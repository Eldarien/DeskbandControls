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
        //public bool UseBackgroundColor { get; set; }
        public bool DrawOutline { get; set; }
        public bool HideBorders { get; set; }
        public int Position
        {
            get { return _position; }
            set { if (!_mousePressed) { SetPosition(value); } }
        }
        public int PaddingTop { get; set; }
        public int PaddingBottom { get; set; }
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
            ForeColor = Gdi32.FixBlackAlpha(ForeColor);
            BackgroundColor = Gdi32.FixBlackAlpha(BackgroundColor);

            var color = new COLORREF(ForeColor);
            
            var rc = new RECT(ClientRectangle);
            var hdc = e.Graphics.GetHdc();

            var memdc = Gdi32.CreateCompatibleDC(hdc);

            var dib = new BITMAPINFO();
            Gdi32.FillBitmapInfo(ref dib, rc.Width, rc.Height);

            var bitmap = Gdi32.CreateDIBSection(memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

            // background & outline
            UxTheme.DrawThemeParentBackground(Handle, memdc, ref rc);
            if (DrawOutline) Gdi32.DrawOutline(memdc, ref rc);

            var alphadc = Gdi32.CreateCompatibleDC(hdc);
            var alphabitmap = Gdi32.CreateDIBSection(alphadc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldalphaBitmap = Gdi32.SelectObject(alphadc, alphabitmap);
            var pixels = new COLORREF[Width * Height];

            Gdi32.FillAlphadc(alphadc, ref rc, StockObjects.HOLLOW_BRUSH, StockObjects.NULL_PEN);
          
            if (BackgroundColor != Color.Transparent)
            {
                var backgroundColor = new COLORREF(BackgroundColor);
                Internal_PaintBackground(alphadc, rc, backgroundColor);
                Gdi32.FixAlphaChannel(alphadc, alphabitmap, (uint)Height, ref pixels, ref dib, backgroundColor);
                Gdi32.DoAlphaBlendToMemdc(memdc, ref rc, alphadc, BackgroundColor.A);

                // clear alphadc
                Gdi32.FillAlphadc(alphadc, ref rc, StockObjects.BLACK_BRUSH, StockObjects.BLACK_PEN);
            }

            Internal_PaintContent(alphadc, rc, color);
            Gdi32.FixAlphaChannel(alphadc, alphabitmap, (uint)Height, ref pixels, ref dib, color);
            Gdi32.DoAlphaBlendToMemdc(memdc, ref rc, alphadc, ForeColor.A);

            Gdi32.BitBlt(hdc, rc.Left, rc.Top, rc.Width, rc.Height, memdc, 0, 0, SRCCOPY);

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

        private void Internal_PaintBackground(IntPtr hdc, RECT rc, COLORREF backgroundColor)
        {
            int offset = HideBorders ? 0 : 2;

            var backgroundPen = Gdi32.CreatePen(PenStyle.PS_SOLID, 0, backgroundColor);
            var backgroundOldPen = Gdi32.SelectObject(hdc, backgroundPen);

            var backgroundBrush = Gdi32.CreateSolidBrush(backgroundColor);
            var backgroundOldBrush = Gdi32.SelectObject(hdc, backgroundBrush);

            Gdi32.Rectangle(hdc, rc.Left + offset, rc.Top + offset + PaddingTop, rc.Right - offset, rc.Bottom - offset - PaddingBottom);

            Gdi32.SelectObject(hdc, backgroundOldPen);
            Gdi32.DeleteObject(backgroundPen);

            Gdi32.SelectObject(hdc, backgroundOldBrush);
            Gdi32.DeleteObject(backgroundBrush);
        }

        private void Internal_PaintContent(IntPtr hdc, RECT rc, COLORREF color)
        {
            var pen = Gdi32.CreatePen(PenStyle.PS_SOLID, 0, color);
            var oldPen = Gdi32.SelectObject(hdc, pen);

            if (!HideBorders)
            {
                Gdi32.SelectObject(hdc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
                Gdi32.RoundRect(hdc, rc.Left, rc.Top + PaddingTop, rc.Right, rc.Bottom - PaddingBottom, 3, 3);
            }

            var brush = Gdi32.CreateSolidBrush(color);
            var oldBrush = Gdi32.SelectObject(hdc, brush);

            if (Range > 0) // Range can be 0 for radio streams
            {
                int offset = HideBorders ? 0 : 2;
                int wx = (rc.Right - offset * 2) * _position / Range;

                Gdi32.Rectangle(hdc, rc.Left + offset, rc.Top + offset + PaddingTop, wx + offset, rc.Bottom - offset - PaddingBottom);
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
            if (p < 0) p = 0;

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