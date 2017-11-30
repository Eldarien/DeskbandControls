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
using Deskband.Core.Extensions;
using Deskband.Console;
using Deskband.Common;

namespace Deskband.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        readonly IConsole _console;
        private string _configDir;
        private string _configFilePath;
        private JArray _data;
        private JsonSerializerSettings _serializerSettings;
        private JsonSerializer _serializer;
        private FileSystemWatcher _watcher;

        public event EventHandler ConfigurationFileChanged;

        public ConfigurationProvider(IConsole console)
        {
            _console = console;

            //var optionsDir = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "DeskbandControls");
            //if (!Directory.Exists(optionsDir))
            //{
            //    Directory.CreateDirectory(optionsDir);
            //}
            //var options = GetOptions(optionsDir);
            //if (!String.IsNullOrWhiteSpace(options.ConfigurationDirectory))
            //{
            //    _configDir = options.ConfigurationDirectory;
            //}
            //else
            //{
            //    _configDir = optionsDir;
            //}

            _configDir = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "DeskbandControls");
            _console.AddLine($"Configuration directory: {_configDir}");
            if (!Directory.Exists(_configDir))
            {
                Directory.CreateDirectory(_configDir);
            }

            _configFilePath = Path.Combine(_configDir, "DeskbandControls.json");
            _data = new JArray();
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter());
            _serializerSettings.Converters.Add(new JsonColorHexConverter());
            _serializer = JsonSerializer.Create(_serializerSettings);

            _watcher = new FileSystemWatcher();
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            _watcher.Path = Path.GetDirectoryName(_configFilePath);
            _watcher.Filter = Path.GetFileName(_configFilePath);
            _watcher.Changed += OnFileChanged;
            _watcher.EnableRaisingEvents = true;
        }

        //private static ConfigurationOptions GetOptions(string configDir)
        //{
        //    var configOptionsPath = Path.Combine(configDir, "Options.json");
        //    var options = ConfigurationOptions.GetDefault();
        //    if (File.Exists(configOptionsPath))
        //    {
        //        var json = File.ReadAllText(configOptionsPath);
        //        options = JObject.Parse(json).ToObject<ConfigurationOptions>();
        //    }
        //    else
        //    {
        //        var json = JsonConvert.SerializeObject(options, Formatting.Indented);
        //        File.WriteAllText(configOptionsPath, json);
        //    }
        //    return options;
        //}

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            _console.AddLine($"Configuration file chnaged, reason: {e.ChangeType}");
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

        private IEnumerable<JToken> AllTokens(JArray obj)
        {
            var toSearch = new Stack<JToken>(obj.Children());
            while (toSearch.Count > 0)
            {
                var inspected = toSearch.Pop();
                yield return inspected;
                foreach (var child in inspected)
                {
                    toSearch.Push(child);
                }
            }
        }

        public void Delete(string profileName)
        {
            if (profileName == null)
                return;

            var filePath = GetConfigFilePath(profileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _console.AddLine($"Profile \"{profileName}\" was deleted");
            }
        }

        public void Load(string profileName = null)
        {
            var filePath = GetConfigFilePath(profileName);
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _data = JArray.Parse(json);

                // fix possible null entries in config
                foreach (var token in AllTokens(_data))
                {
                    if (token.Type == JTokenType.Null)
                    {
                        token.Replace(new JValue(""));
                    }
                }

                _console.AddLine($"Profile \"{profileName ?? "Default"}\" loaded");
            }
        }

        public void Save(string profileName = null)
        {
            var watcherEnabled = _watcher.EnableRaisingEvents;
            _watcher.EnableRaisingEvents = false;

            var filePath = GetConfigFilePath(profileName);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(_data, Formatting.Indented));
            _console.AddLine($"Profile \"{profileName ?? "Default"}\" saved");

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
    }
}
