using Deskband.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Deskband.Core.Configuration;
using Deskband.Extensions;

namespace Deskband.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private string _configDir;
        private string _configFilePath;
        private JArray _data;
        private JsonSerializerSettings _serializerSettings;
        private JsonSerializer _serializer;
        private FileSystemWatcher _watcher;

        public event EventHandler ConfigurationFileChanged;

        public ConfigurationProvider()
        {
            _configDir = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "DeskbandControls");
            if (!Directory.Exists(_configDir))
            {
                Directory.CreateDirectory(_configDir);
            }
            var options = GetOptions(_configDir);
            if (!String.IsNullOrWhiteSpace(options.ConfigurationDirectory))
            {
                _configDir = options.ConfigurationDirectory;
            }

            _configFilePath = Path.Combine(_configDir, "DeskbandControls.json");
            _data = new JArray();
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter());
            _serializer = JsonSerializer.Create(_serializerSettings);

            _watcher = new FileSystemWatcher();
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            _watcher.Path = Path.GetDirectoryName(_configFilePath);
            _watcher.Filter = Path.GetFileName(_configFilePath);
            _watcher.Changed += OnFileChanged;
            _watcher.EnableRaisingEvents = true;
        }

        private static ConfigurationOptions GetOptions(string configDir)
        {
            var configOptionsPath = Path.Combine(configDir, "Options.json");
            var options = ConfigurationOptions.GetDefault();
            if (File.Exists(configOptionsPath))
            {
                var json = File.ReadAllText(configOptionsPath);
                options = JObject.Parse(json).ToObject<ConfigurationOptions>();
            }
            else
            {
                var json = JsonConvert.SerializeObject(options, Formatting.Indented);
                File.WriteAllText(configOptionsPath, json);
            }
            return options;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            ConfigurationFileChanged?.Invoke(null, EventArgs.Empty);
        }

        private string GetConfigFilePath(string profileName)
        {
            if (profileName == null) return _configFilePath;
            return Path.Combine(_configDir, "_" + profileName.SanitizeFileName() + ".json");
        }

        public void DisableWatcher()
        {
            _watcher.EnableRaisingEvents = false;
        }

        public IEnumerable<string> GetProfiles()
        {
            return Directory.GetFiles(_configDir, "_*.json", SearchOption.TopDirectoryOnly)
                .Select(x => Path.GetFileNameWithoutExtension(x).Substring(1));
        }

        public bool ProfileExists(string profileName)
        {
            var filePath = GetConfigFilePath(profileName);
            return File.Exists(filePath);
        }

        public void Load(string profileName = null)
        {
            var filePath = GetConfigFilePath(profileName);
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _data = JArray.Parse(json);
            }
        }

        public void Save(string profileName = null)
        {
            var watcherEnabled = _watcher.EnableRaisingEvents;
            _watcher.EnableRaisingEvents = false;

            var filePath = GetConfigFilePath(profileName);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(_data, Formatting.Indented));

            if (watcherEnabled) _watcher.EnableRaisingEvents = true;
        }

        public T GetConfiguration<T>(Guid moduleId, T defaultConfiguration) where T : ConfigurationObjectBase
        {
            var index = _data.ToObject<List<ConfigurationObjectBase>>().FindIndex(x => x.ModuleId == moduleId);
            if (index == -1)
                return defaultConfiguration;

            return _data.ElementAt(index).ToObject<T>();
        }

        public void UpdateConfiguration<T>(T configurationObject) where T : ConfigurationObjectBase
        {
            var jtoken = JToken.FromObject(configurationObject, _serializer);
            var index = _data.ToObject<List<ConfigurationObjectBase>>().FindIndex(x => x.ModuleId == configurationObject.ModuleId);
            if (index == -1)
                _data.Add(jtoken);
            else
                _data.ElementAt(index).Replace(jtoken);
        }

        public object GetAllConfiguration()
        {
            return _data.ToObject<List<ConfigurationObjectBase>>().ToArray();
        }
    }
}
