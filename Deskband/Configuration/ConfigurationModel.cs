using Deskband.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Deskband.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public override string ToString() => "Deskband Controls";

        [Browsable(false), JsonIgnore]
        public override bool Disabled { get; set; }

        [Browsable(false), JsonIgnore]
        public override int Offset { get; set; }

        [Browsable(false), JsonIgnore]
        public override int Order { get; set; }

        [Browsable(false), JsonIgnore]
        public override int Width { get; set; }

        [Browsable(false), JsonIgnore]
        public override int Height { get; set; }

        [Browsable(false), JsonIgnore]
        public override string BackgroundImagePath { get; set; }

        [Browsable(false), JsonIgnore]
        public override bool StretchBackgroundImage { get; set; }

        [Browsable(false)]
        [SettingsNode("General Settings")]
        public GeneralSettings GeneralSettings { get; set; } = GeneralSettings.Default;

        [Browsable(false)]
        [SettingsNode("Floating Window")]
        public FloatingWindowSettings FloatingWindowSettings { get; set; } = FloatingWindowSettings.Default;

        public static ConfigurationModel Default => new ConfigurationModel { ModuleId = Guid.Empty, Width = 10, Height = 40 };
    }
}