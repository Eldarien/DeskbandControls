using Deskband.BandIntegration;
using Deskband.Common;
using Deskband.Controls;
using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using Deskband.Extensions;
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
        public static readonly Guid ModuleId = Guid.Parse("{2A5AF4C8-25AE-4276-8E29-D9E2198E7114}");
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

            _taskbarWindowHandle = User32.FindWindow("Shell_TrayWnd", null);

            try
            {
                AssemblyResolver.Initialize();

                _kernel = AppConfig.InitializeKernel(this);
                foreach (var m in _kernel.GetAll<IModule>())
                {
                    m.Initialize(_kernel);
                }
                _kernel.Get<App>().Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Deskband Controls Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApiTypes.WM_NCHITTEST)
            {
                int x = ((int)m.LParam).LowWord();
                int y = ((int)m.LParam).HighWord();

                var point = new WinApiTypes.POINT { X = x, Y = y };
                if (User32.ScreenToClient(_taskbarWindowHandle, ref point))
                {
                    WinApiTypes.RECT r;
                    User32.GetWindowRect(_taskbarWindowHandle, out r);
                    bool isHorizontal = (r.right - r.left) > (r.bottom - r.top);

                    if (isHorizontal && point.Y == 0 || !isHorizontal && point.X == 0)
                    {
                        m.Result = (IntPtr)WinApiTypes.HTTRANSPARENT;
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