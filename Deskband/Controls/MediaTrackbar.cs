using Deskband.Common;
using Deskband.Common.Extensions;
using Deskband.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Controls
{
    public class MediaTrackbarPositionEventArgs
    {
        public int Position { get; private set; }

        public MediaTrackbarPositionEventArgs(int position)
        {
            this.Position = position;
        }
    }

    public delegate void MediaTrackbarPositionEventHandler(object sender, MediaTrackbarPositionEventArgs e);

    public partial class MediaTrackbar : UserControl
    {
        public event MediaTrackbarPositionEventHandler OnPositionChanged;

        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        public Enums.TrackbarKindType Kind { get; set; }

        public bool DrawOutline { get; set; }

        private int position = 0;

        private bool mousePressed;

        public int Position
        {
            get { return position; }
            set
            {
                position = value;
                WinApi.InvalidateRect(this.Handle, IntPtr.Zero, false);
            }
        }

        public int Range { get; set; }

        public MediaTrackbar()
        {
            InitializeComponent();

            this.Cursor = Cursors.Hand;

            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var color = new WinApi.COLORREF(this.ForeColor);
            var rc = new WinApi.RECT(ClientRectangle);
            var hdc = e.Graphics.GetHdc();

            var memdc = WinApi.CreateCompatibleDC(hdc);

            var dib = new WinApi.BITMAPINFO();
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(WinApi.BITMAPINFOHEADER));
            dib.bmiHeader.biHeight = -(rc.bottom - rc.top); // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = rc.right - rc.left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = WinApi.BI_RGB;

            var bitmap = WinApi.CreateDIBSection(memdc, ref dib, WinApi.DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldBitmap = WinApi.SelectObject(memdc, bitmap);

            //
            InternalOnPaint(memdc, rc, color);

            // Fix alpha channel
            var rgbColor = (color.ColorDWORD & 0x000000FF) << 16 | (color.ColorDWORD & 0x0000FF00) | (color.ColorDWORD & 0x00FF0000) >> 16;
            var pixels = new WinApi.COLORREF[Width * Height];
            WinApi.GetDIBits(memdc, bitmap, 0, (uint)Height, pixels, ref dib, 0);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].ColorDWORD == rgbColor)
                    pixels[i].ColorDWORD |= 0xFF000000;
            }
            WinApi.SetDIBits(memdc, bitmap, 0, (uint)Height, pixels, ref dib, 0);

            WinApi.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, WinApi.SRCCOPY);

            WinApi.SelectObject(memdc, oldBitmap);
            WinApi.DeleteObject(bitmap);

            WinApi.ReleaseDC(memdc, -1);
            WinApi.DeleteDC(memdc);

            e.Graphics.ReleaseHdc(hdc);
        }

        private void InternalOnPaint(IntPtr hdc, WinApi.RECT rc, WinApi.COLORREF color)
        {
            WinApi.DrawThemeParentBackground(Handle, hdc, ref rc);
            if (this.DrawOutline)
            {
                WinApi.SelectObject(hdc, WinApi.GetStockObject(WinApi.StockObjects.HOLLOW_BRUSH));
                WinApi.SelectObject(hdc, WinApi.GetStockObject(WinApi.StockObjects.WHITE_PEN));
                WinApi.Rectangle(hdc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
            }

            var pen = WinApi.CreatePen(WinApi.PenStyle.PS_SOLID, 0, color);
            var oldPen = WinApi.SelectObject(hdc, pen);

            WinApi.SelectObject(hdc, WinApi.GetStockObject(WinApi.StockObjects.HOLLOW_BRUSH));
            WinApi.RoundRect(hdc, rc.left, rc.top, rc.right, rc.bottom, 3, 3);

            var brush = WinApi.CreateSolidBrush(color);
            var oldBrush = WinApi.SelectObject(hdc, brush);

            if (this.Range > 0) // Range can be 0 for radio streams
            {
                int wx = (rc.right - 4) * this.Position / this.Range;

                WinApi.Rectangle(hdc, rc.left + 2, rc.top + 2, wx + 2, rc.bottom - 2);
            }

            WinApi.SelectObject(hdc, oldBrush);
            WinApi.DeleteObject(brush);

            WinApi.SelectObject(hdc, oldPen);
            WinApi.DeleteObject(pen);
        }

        private void SetPositionByMouseX(int x)
        {
            int p = x * this.Range / (this.Width - 4);
            if (p > this.Range) p = this.Range;

            if (p != this.Position)
            {
                this.Position = p;

                if (this.OnPositionChanged != null)
                    OnPositionChanged(this, new MediaTrackbarPositionEventArgs(p));
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
                    OnPositionChanged(this, new MediaTrackbarPositionEventArgs(p));
            }
        }

        public static MediaTrackbar Create(Settings.Models.TrackbarModel x, bool outline)
        {
            var tb = new MediaTrackbar();
            tb.DrawOutline = outline;
            tb.Kind = x.Kind;

            tb.Visible = x.Visible;
            tb.Location = new Point(x.X, x.Y); //x.BoundRect.Location;
            tb.Size = new Size(x.Width, x.Height); //x.BoundRect.Size;
            tb.ForeColor = x.Color.AsDrawingColor(); //ColorHelpers.GetThemedColor(x.FgColor);

            tb.Range = 100;
            tb.Position = 0;

            return tb;
        }
    }
}