using System;
using System.ComponentModel;

namespace Deskband.Core.Configuration
{
    // See Deskband.Settings.SettingsModel
    [SettingsCommand("Move Up", "MoveUp", "IsMoveAvailable")]
    [SettingsCommand("Move Down", "MoveDown", "IsMoveAvailable")]
    public class ConfigurationObjectBase
    {
        [Browsable(false)]
        public Guid ModuleId { get; set; }

        [Browsable(false)]
        public virtual int Order { get; set; }

        [Category("General"), TypeConverter(typeof(YesNoBooleanConverter))]
        public virtual bool Disabled { get; set; }

        [Category("Position")]
        public virtual int Offset { get; set; }

        [Category("Size")]
        public virtual int Width { get; set; }

        [Category("Size")]
        public virtual int Height { get; set; }
    }
}