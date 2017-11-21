using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using System.Drawing;

namespace Deskband.UI
{
    public class SizeProvider : ISizeProvider
    {
        private int _dpi;
        private float _scale;
        private float? _initialScale;

        public SizeProvider(Band band)
        {
            band.DPIChanged += Band_DPIChanged;
        }

        private void Band_DPIChanged(object sender, Core.EventArguments.ValueEventArgs<int> e)
        {
            _dpi = e.Value;
            _scale = (float)_dpi / WinApiTypes.USER_DEFAULT_SCREEN_DPI;
            if (_initialScale == null) _initialScale = _scale;
        }

        public int DPI => _dpi;
        public float Scale => _scale;
        public float InitialScale => _initialScale ?? _scale;

        public Point MakePoint(int x, int y)
        {
            return new Point((int)(x * _scale), (int)(y * _scale));
        }

        public Size MakeSize(int width, int height)
        {
            return new Size((int)(width * _scale), (int)(height * _scale));
        }

        public int MakeValue(int value)
        {
            return (int)(value * _scale);
        }

        public int MakeInitialValue(int value)
        {
            return (int)(value * (_initialScale ?? _scale));
        }
    }
}
