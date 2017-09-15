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
using Deskband.Common;
using Deskband.Configuration;
using static Deskband.Core.WinApi.WinApiTypes;
using System.IO;
using System.Reflection;
using Deskband.Console;

namespace Deskband
{
    [ComVisible(true)]
    [Guid(Band.ClassGuid)]
    [BandObject(Band.ClassTitle)]
    public class Band : BandObject
    {
        public const String ClassGuid = DeskbandBridge.FB2KConstants.DeskbandGuid;
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
        private ConsoleHandler _console;

        public Band() // Entry Point
        {
            Title = Band.ClassTitle;
            BackColor = Color.Transparent;

            _taskbarWindowHandle = User32.FindWindow("Shell_TrayWnd", null);

            try
            {
                AssemblyResolver.Initialize();

                _kernel = AppConfig.InitializeKernel(this);
                _console = _kernel.Get<ConsoleHandler>();

                var installationDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _console.AddLine($"Installation directory: {installationDir}");

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
                var taskbarInfo = GetTaskbarSizeInfo();
                var taskbarHeight = taskbarInfo.Rect.Height;
                var taskbarWidth = taskbarInfo.Rect.Width;

                // 2 points at each edge should be NCHITTEST transparent for auto-hide of taskbar to work correctly
                var horizontalPoints = new[] { 0, 1, taskbarHeight - 1, taskbarHeight - 2 };
                var verticalPoints = new[] { 0, 1, taskbarWidth - 1, taskbarWidth - 2 };

                var point = new WinApiTypes.POINT { X = x, Y = y };
                if (User32.ScreenToClient(_taskbarWindowHandle, ref point))
                {
                    var tsi = GetTaskbarSizeInfo();
                    if (tsi.Mode == LayoutMode.Horizontal && horizontalPoints.Contains(point.Y)
                        ||
                        tsi.Mode == LayoutMode.Vertical && verticalPoints.Contains(point.X))
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