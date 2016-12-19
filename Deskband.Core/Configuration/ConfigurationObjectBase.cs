using Deskband.Core.Common;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

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

        [Category("Background"), DisplayName("Image Path"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public virtual string BackgroundImagePath { get { return _backgroundImagePath; } set { _backgroundImagePath = PathHelpers.TryPlaceEnvVars(value); } }
        private string _backgroundImagePath;

        [Category("Background"), DisplayName("Stretch Image"), TypeConverter(typeof(YesNoBooleanConverter))]
        public virtual bool StretchBackgroundImage { get; set; }
    }
}