using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Deskband.Core.WinApi.WinApiTypes;
using Deskband.Core.EventArguments;

namespace Deskband.Core.Controls
{
    public partial class dcTrackbar : UserControl
    {
        public event EventHandler<ValueEventArgs<int>> OnPositionChanged;
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }
        public Color BackgroundColor { get; set; }
        public bool UseBackgroundColor { get; set; }
        public bool DrawOutline { get; set; }
        public bool HideBorders { get; set; }
        public int Position
        {
            get { return _position; }
            set { if (!_mousePressed) { SetPosition(value); } }
        }
        public bool ChangeOnMouseUp { get; set; }
        public int Range { get; set; }

        private int _position = 0;
        private bool _mousePressed;

        public dcTrackbar()
        {
            InitializeComponent();

            Cursor = Cursors.Hand;
            BackColor = Color.Transparent;
        }

        private void SetPosition(int position)
        {
            _position = position;
            User32.InvalidateRect(Handle, IntPtr.Zero, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Fix for black color alpha blend issue
            if (ForeColor.R == 0 && ForeColor.G == 0 && ForeColor.B == 0)
                ForeColor = Color.FromArgb(ForeColor.A, 1, 1, 1);

            if (BackgroundColor.R == 0 && BackgroundColor.G == 0 && BackgroundColor.B == 0)
                BackgroundColor = Color.FromArgb(BackgroundColor.A, 1, 1, 1);

            var color = new COLORREF(ForeColor);
            var backgroundColor = new COLORREF(BackgroundColor);
            var rc = new RECT(ClientRectangle);
            var hdc = e.Graphics.GetHdc();

            var memdc = Gdi32.CreateCompatibleDC(hdc);

            var dib = new BITMAPINFO();
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biHeight = -(rc.Bottom - rc.Top); // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = rc.Right - rc.Left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;

            var bitmap = Gdi32.CreateDIBSection(memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

            // background & outline
            UxTheme.DrawThemeParentBackground(Handle, memdc, ref rc);
            if (this.DrawOutline)
            {
                Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
                Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.WHITE_PEN));
                Gdi32.Rectangle(memdc, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top);
            }

            // Redraw background from memdc to dc
            //WinApi.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, WinApi.SRCCOPY);

            // draw trackbar
            var alphadc = Gdi32.CreateCompatibleDC(hdc);
            var alphabitmap = Gdi32.CreateDIBSection(alphadc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldalphaBitmap = Gdi32.SelectObject(alphadc, alphabitmap);
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.NULL_PEN));
            Gdi32.Rectangle(alphadc, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top);
            InternalOnPaint(alphadc, rc, color, backgroundColor);

            // Fix alpha channel
            var rgbColor = (color.ColorDWORD & 0x000000FF) << 16 | (color.ColorDWORD & 0x0000FF00) | (color.ColorDWORD & 0x00FF0000) >> 16;
            var rgbBackgroundColor = (backgroundColor.ColorDWORD & 0x000000FF) << 16 | (backgroundColor.ColorDWORD & 0x0000FF00) | (backgroundColor.ColorDWORD & 0x00FF0000) >> 16;
            var pixels = new COLORREF[Width * Height];
            Gdi32.GetDIBits(alphadc, alphabitmap, 0, (uint)Height, pixels, ref dib, 0);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].ColorDWORD == rgbColor || pixels[i].ColorDWORD == rgbBackgroundColor && UseBackgroundColor)
                    pixels[i].ColorDWORD |= 0xFF000000;
            }
            Gdi32.SetDIBits(alphadc, alphabitmap, 0, (uint)Height, pixels, ref dib, 0);

            var blendFunc = new BLENDFUNCTION(AC_SRC_OVER, 0, ForeColor.A, AC_SRC_ALPHA);
            Gdi32.AlphaBlend(memdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, alphadc,
                0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top,
                blendFunc);

            Gdi32.BitBlt(hdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, memdc, 0, 0, SRCCOPY);

            Gdi32.SelectObject(memdc, oldBitmap);
            Gdi32.DeleteObject(bitmap);

            Gdi32.SelectObject(alphadc, oldalphaBitmap);
            Gdi32.DeleteObject(alphabitmap);
            Gdi32.ReleaseDC(alphadc, -1);
            Gdi32.DeleteDC(alphadc);

            Gdi32.ReleaseDC(memdc, -1);
            Gdi32.DeleteDC(memdc);

            e.Graphics.ReleaseHdc(hdc);
        }

        private void InternalOnPaint(IntPtr hdc, RECT rc, COLORREF color, COLORREF backgroundColor)
        {
            var pen = Gdi32.CreatePen(PenStyle.PS_SOLID, 0, color);
            var oldPen = Gdi32.SelectObject(hdc, pen);

            if (!HideBorders)
            {
                Gdi32.SelectObject(hdc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
                Gdi32.RoundRect(hdc, rc.Left, rc.Top, rc.Right, rc.Bottom, 3, 3);
            }

            int offset = HideBorders ? 0 : 2;

            if (UseBackgroundColor)
            {
                var backgroundPen = Gdi32.CreatePen(PenStyle.PS_SOLID, 0, backgroundColor);
                var backgroundOldPen = Gdi32.SelectObject(hdc, backgroundPen);

                var backgroundBrush = Gdi32.CreateSolidBrush(backgroundColor);
                var backgroundOldBrush = Gdi32.SelectObject(hdc, backgroundBrush);

                Gdi32.Rectangle(hdc, rc.Left + offset, rc.Top + offset, rc.Right - offset, rc.Bottom - offset);

                Gdi32.SelectObject(hdc, backgroundOldPen);
                Gdi32.DeleteObject(backgroundPen);

                Gdi32.SelectObject(hdc, backgroundOldBrush);
                Gdi32.DeleteObject(backgroundBrush);
            }

            var brush = Gdi32.CreateSolidBrush(color);
            var oldBrush = Gdi32.SelectObject(hdc, brush);

            if (Range > 0) // Range can be 0 for radio streams
            {
                int wx = (rc.Right - offset * 2) * _position / Range;

                Gdi32.Rectangle(hdc, rc.Left + offset, rc.Top + offset, wx + offset, rc.Bottom - offset);
            }

            Gdi32.SelectObject(hdc, oldBrush);
            Gdi32.DeleteObject(brush);

            Gdi32.SelectObject(hdc, oldPen);
            Gdi32.DeleteObject(pen);
        }

        private void SetPositionByMouseX(int x, bool raisePositionChanged)
        {
            int clientWidth = Width - 4;
            if (clientWidth <= 0)
                return;

            int p = x * Range / clientWidth;
            if (p > Range) p = Range;

            SetPosition(p);

            if (raisePositionChanged)
            {
                OnPositionChanged?.Invoke(this, new ValueEventArgs<int>(p));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left || Range == 0)
                return;

            _mousePressed = true;

            if (!ChangeOnMouseUp)
            {
                SetPositionByMouseX(e.X, true);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _mousePressed = false;

            if (e.Button != MouseButtons.Left || Range == 0)
                return;

            if (ChangeOnMouseUp)
            {
                SetPositionByMouseX(e.X, true);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_mousePressed)
                return;

            SetPositionByMouseX(e.X, !ChangeOnMouseUp);
        }

        public void SetDelta(int delta)
        {
            var p = Position + delta;
            if (p > Range) p = Range;
            if (p < 0) p = 0;
            if (p != Position)
            {
                Position = p;

                if (this.OnPositionChanged != null)
                    OnPositionChanged(this, new ValueEventArgs<int>(p));
            }
        }
    }
}