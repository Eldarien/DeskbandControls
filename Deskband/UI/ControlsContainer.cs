using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class ControlsContainer : UserControl
    {
        public ControlsContainer()
        {
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApiTypes.WM_NCHITTEST)
            {
                m.Result = (IntPtr)WinApiTypes.HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
