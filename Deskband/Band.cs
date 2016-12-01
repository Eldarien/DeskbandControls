using Deskband.BandIntegration;
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
using Deskband.Core.EventArguments;
using Deskband.UI;
using Deskband.Core.Common;

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
        public event EventHandler<ValueEventArgs<int>> DPIChanged;
        public event EventHandler TaskbarResized;

        private IntPtr _taskbarWindowHandle;
        private IKernel _kernel;

        public Band() // Entry Point
        {
            Title = Band.ClassTitle;
            BackColor = Color.Transparent;

            _taskbarWindowHandle = User32.FindWindow("Shell_TrayWnd", null);

            try
            {
                //AssemblyResolver.Initialize();

                _kernel = AppConfig.InitializeKernel(this);

                using (var g = CreateGraphics())
                {
                    DPIChanged?.Invoke(this, new ValueEventArgs<int>((int)g.DpiX));
                }

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
                    var tsi = GetTaskbarSizeInfo();
                    if (tsi.Mode == LayoutMode.Horizontal && point.Y == 0 || tsi.Mode == LayoutMode.Vertical && point.X == 0)
                    {
                        m.Result = (IntPtr)WinApiTypes.HTTRANSPARENT;
                        return;
                    }
                }
            }

            if (m.Msg == WinApiTypes.WM_DPICHANGED)
            {
                int dpi = ((int)m.WParam).LowWord();
                DPIChanged?.Invoke(this, new ValueEventArgs<int>(dpi));
            }

            if (m.Msg == WinApiTypes.WM_SETTINGCHANGE)
            {
                if ((int)m.WParam == WinApiTypes.SPI_SETWORKAREA)
                {
                    TaskbarResized?.Invoke(this, EventArgs.Empty);
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnClose()
        {
            Close?.Invoke(this, EventArgs.Empty);
            base.OnClose();
        }

        protected override void Dispose(bool disposing)
        {
            _kernel.Dispose();
            base.Dispose(disposing);
        }

        public TaskbarSizeInfo GetTaskbarSizeInfo()
        {
            WinApiTypes.RECT r;
            User32.GetWindowRect(_taskbarWindowHandle, out r);
            return new TaskbarSizeInfo { Rect = r };
        }
    }
}