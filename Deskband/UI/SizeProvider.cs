using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;

namespace Deskband.UI
{
    public class SizeProvider : ISizeProvider
    {
        private readonly IConsole _console;
        private int _dpi;
        private float _scale;

        public SizeProvider(Band band, IConsole console)
        {
            _console = console;

            band.DPIChanged += Band_DPIChanged;
        }

        private void Band_DPIChanged(object sender, Core.EventArguments.ValueEventArgs<int> e)
        {
            _dpi = e.Value;
            _scale = (float)_dpi / WinApiTypes.USER_DEFAULT_SCREEN_DPI;
            _console.AddDebugLine(String.Format("DPI changed: {0}", _dpi));
        }

        public int DPI { get { return _dpi; } }
        public float Scale { get { return _scale; } }

        public Point MakePoint(int x, int y)
        {
            return new Point((int)(x * _scale), (int)(y * _scale));
        }

        public Size MakeSize(int width, int height)
        {
            return new Size((int)(width * _scale), (int)(height * _scale));
        }
    }
}
