using Deskband.Configuration;
using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Settings
{
    public class SettingsModel
    {
        [SettingsNodeList("Settings")]
        public List<object> SettingsModels { get; set; }
    }
}