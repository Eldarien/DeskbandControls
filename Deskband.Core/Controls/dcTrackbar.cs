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
    //public class MediaTrackbarPositionEventArgs
    //{
    //    public int Position { get; private set; }

    //    public MediaTrackbarPositionEventArgs(int position)
    //    {
    //        this.Position = position;
    //    }
    //}

    //public delegate void MediaTrackbarPositionEventHandler(object sender, MediaTrackbarPositionEventArgs e);

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

        //public Enums.TrackbarKindType Kind { get; set; }

        public bool DrawOutline { get; set; }

        public bool HideBorders { get; set; }

        private int position = 0;

        private bool mousePressed;

        public int Position
        {
            get { return position; }
            set
            {
                position = value;
                User32.InvalidateRect(this.Handle, IntPtr.Zero, false);
            }
        }

        public int Range { get; set; }

        public dcTrackbar()
        {
            InitializeComponent();

            this.Cursor = Cursors.Hand;

            this.BackColor = Color.Transparent;
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
            dib.bmiHeader.biHeight = -(rc.bottom - rc.top); // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = rc.right - rc.left;
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
                Gdi32.Rectangle(memdc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
            }

            // Redraw background from memdc to dc
            //WinApi.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, WinApi.SRCCOPY);

            // draw trackbar
            var alphadc = Gdi32.CreateCompatibleDC(hdc);
            var alphabitmap = Gdi32.CreateDIBSection(alphadc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldalphaBitmap = Gdi32.SelectObject(alphadc, alphabitmap);
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.NULL_PEN));
            Gdi32.Rectangle(alphadc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
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
            Gdi32.AlphaBlend(memdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, alphadc,
                0, 0, rc.right - rc.left, rc.bottom - rc.top,
                blendFunc);

            Gdi32.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, SRCCOPY);

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
                Gdi32.RoundRect(hdc, rc.left, rc.top, rc.right, rc.bottom, 3, 3);
            }

            int offset = HideBorders ? 0 : 2;

            if (UseBackgroundColor)
            {
                var backgroundPen = Gdi32.CreatePen(PenStyle.PS_SOLID, 0, backgroundColor);
                var backgroundOldPen = Gdi32.SelectObject(hdc, backgroundPen);

                var backgroundBrush = Gdi32.CreateSolidBrush(backgroundColor);
                var backgroundOldBrush = Gdi32.SelectObject(hdc, backgroundBrush);

                Gdi32.Rectangle(hdc, rc.left + offset, rc.top + offset, rc.right - offset, rc.bottom - offset);

                Gdi32.SelectObject(hdc, backgroundOldPen);
                Gdi32.DeleteObject(backgroundPen);

                Gdi32.SelectObject(hdc, backgroundOldBrush);
                Gdi32.DeleteObject(backgroundBrush);
            }

            var brush = Gdi32.CreateSolidBrush(color);
            var oldBrush = Gdi32.SelectObject(hdc, brush);

            if (this.Range > 0) // Range can be 0 for radio streams
            {
                int wx = (rc.right - offset * 2) * this.Position / this.Range;

                Gdi32.Rectangle(hdc, rc.left + offset, rc.top + offset, wx + offset, rc.bottom - offset);
            }

            Gdi32.SelectObject(hdc, oldBrush);
            Gdi32.DeleteObject(brush);

            Gdi32.SelectObject(hdc, oldPen);
            Gdi32.DeleteObject(pen);
        }

        private void SetPositionByMouseX(int x)
        {
            int clientWidth = this.Width - 4;
            if (clientWidth <= 0)
                return;

            int p = x * this.Range / clientWidth;
            if (p > this.Range) p = this.Range;

            if (p != this.Position)
            {
                this.Position = p;

                if (this.OnPositionChanged != null)
                    OnPositionChanged(this, new ValueEventArgs<int>(p));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left || this.Range == 0)
                return;

            mousePressed = true;

            SetPositionByMouseX(e.X);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            mousePressed = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!mousePressed)
                return;

            SetPositionByMouseX(e.X);
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

        //public static dcTrackbar Create(Settings.Models.TrackbarModel x, bool outline)
        //{
        //    var tb = new dcTrackbar();
        //    tb.DrawOutline = outline;
        //    tb.Kind = x.Kind;

        //    tb.Visible = x.Visible;
        //    tb.Location = new Point(x.X, x.Y); //x.BoundRect.Location;
        //    tb.Size = new Size(x.Width, x.Height); //x.BoundRect.Size;
        //    tb.ForeColor = x.Color.AsDrawingColor(); //ColorHelpers.GetThemedColor(x.FgColor);
        //    tb.BackgroundColor = x.BackgroundColor.AsDrawingColor();
        //    tb.UseBackgroundColor = x.UseBackgroundColor;
        //    tb.HideBorders = x.HideBorders;

        //    tb.Range = 100;
        //    tb.Position = 0;

        //    return tb;
        //}
    }
}