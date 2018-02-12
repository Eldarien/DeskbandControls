using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public enum DisplayMode
    {
        [Description("Docked Deskband")]
        Deskband,

        [Description("Floating Deskband")]
        FloatingDeskband,

        [Description("Floating Window")]
        FloatingWindow
    }
}
