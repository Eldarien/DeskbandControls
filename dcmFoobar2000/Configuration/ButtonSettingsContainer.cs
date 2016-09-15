using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class ButtonSettingsContainer
    {
        [Browsable(false), SettingsNode("Stop Button")]
        public ButtonSettings BtnStop { get; set; }

        [Browsable(false), SettingsNode("Play/Pause Button")]
        public ButtonSettings BtnPlayPause { get; set; }

        [Browsable(false), SettingsNode("Previous Button")]
        public ButtonSettings BtnPrev { get; set; }

        [Browsable(false), SettingsNode("Next Button")]
        public ButtonSettings BtnNext { get; set; }

        [Browsable(false), SettingsNode("Random Button")]
        public ButtonSettings BtnRandom { get; set; }

        [Browsable(false), SettingsNode("Stop After Current Button")]
        public ButtonSettings BtnStopAC { get; set; }
    }
}