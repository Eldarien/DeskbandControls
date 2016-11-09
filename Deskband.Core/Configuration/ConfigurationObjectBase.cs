using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Core.Configuration
{
    // See Deskband.Settings.SettingsModel
    [SettingsCommand("Move Up", "MoveUp", "IsMoveAvailable")]
    [SettingsCommand("Move Down", "MoveDown", "IsMoveAvailable")]
    public class ConfigurationObjectBase
    {
        [Browsable(false)]
        public Guid ModuleId { get; set; }

        [Category("Position")]
        public virtual int Offset { get; set; }

        [Browsable(false)]
        public virtual int Order { get; set; }

        [Category("Size")]
        public virtual int Width { get; set; }

        [Category("Size")]
        public virtual int Height { get; set; }
    }
}