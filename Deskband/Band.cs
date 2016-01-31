using Deskband.Common;
using Deskband.Common.Extensions;
using Deskband.Controls;
using Deskband.Native;
using Ninject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Deskband
{
    [ComVisible(true)]
    [Guid(Band.ClassGuid)]
    [BandObject(Band.ClassTitle)]
    public class Band : BandObject
    {
        public const String ClassGuid = "9690ED28-CD24-4534-B380-77103A4E7774";
        public const String ClassTitle = "Deskband Controls";

        protected override string GetClassGuidString()
        {
            return ClassGuid;
        }

        public event EventHandler Close;

        private IntPtr _taskbarWindowHandle;
        private IKernel _kernel;

        public Band() // Entry Point
        {
            Title = Band.ClassTitle;
            BackColor = Color.Transparent;

            _taskbarWindowHandle = WinApi.FindWindow("Shell_TrayWnd", null);

            try
            {
                AssemblyResolver.Initialize();

                _kernel = AppConfig.InitializeKernel(this);
                var app = _kernel.Get<App>();
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Deskband Controls Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_NCHITTEST)
            {
                int x = ((int)m.LParam).LowWord();
                int y = ((int)m.LParam).HighWord();

                var point = new WinApi.POINT { X = x, Y = y };
                if (WinApi.ScreenToClient(_taskbarWindowHandle, ref point))
                {
                    WinApi.RECT r;
                    WinApi.GetWindowRect(_taskbarWindowHandle, out r);
                    bool isHorizontal = (r.right - r.left) > (r.bottom - r.top);

                    if (isHorizontal && point.Y == 0 || !isHorizontal && point.X == 0)
                    {
                        m.Result = (IntPtr)WinApi.HTTRANSPARENT;
                        return;
                    }
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnClose()
        {
            if (Close != null)
                Close(this, EventArgs.Empty);

            base.OnClose();
        }

        protected override void Dispose(bool disposing)
        {
            _kernel.Dispose();
            base.Dispose(disposing);
        }
    }
}