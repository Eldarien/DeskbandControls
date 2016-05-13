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

namespace Deskband.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private string _configDir;
        private string _configFilePath;
        private JArray _data;
        private JsonSerializerSettings _serializerSettings;
        private JsonSerializer _serializer;

        public ConfigurationProvider()
        {
            _configDir = Path.Combine(Environment.GetEnvironmentVariable("AppData"), "DeskbandControls");
            if (!Directory.Exists(_configDir))
            {
                Directory.CreateDirectory(_configDir);
            }

            _configFilePath = Path.Combine(_configDir, "configuration.js");
            _data = new JArray();

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter());

            _serializer = JsonSerializer.Create(_serializerSettings);
        }

        public void Load()
        {
            if (File.Exists(_configFilePath))
            {
                var json = File.ReadAllText(_configFilePath);
                _data = JArray.Parse(json);
            }
        }

        public void Save()
        {
            File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(_data, Formatting.Indented));
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
