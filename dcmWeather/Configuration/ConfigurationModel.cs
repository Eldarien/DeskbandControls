using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dcmWeather.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public override string ToString()
        {
            return WeatherModule.ModuleName;
        }

        public static readonly ConfigurationModel Default = new ConfigurationModel
        {
            ModuleId = WeatherModule.ModuleId,
            Width = 100,
            Height = 30
        };
    }
}