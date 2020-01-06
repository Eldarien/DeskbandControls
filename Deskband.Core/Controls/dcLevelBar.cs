using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Deskband.Core.WinApi;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.Controls
{
    public partial class dcLevelbar : UserControl
    {
        public bool DrawOutline { get; set; }
        public int Position
        {
            get { return _position; }
            set { SetPosition(value); }
        }

        public int SegmentsCount { get; set; }
        public int TransitionPoint { get; set; }
        public bool StripedSegments { get; set; }
        public int SegmentSpaceRatio { get; set; }

        public Color BackgroundColor { get; set; }
        public Color PrimarySegmentColor { get; set; }
        public Color SecondarySegmentColor { get; set; }
        public Color InactiveSegmentColor { get; set; }

        public int PaddingTop { get; set; }
        public int PaddingBottom { get; set; }

        public int Range { get; set; }


        private int _position = 0;
        

        public dcLevelbar()
        {
            BackColor = Color.Transparent;
        }

        private void SetPosition(int position)
        {
            _position = position;
            User32.InvalidateRect(Handle, IntPtr.Zero, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            BackgroundColor = Gdi32.FixBlackAlpha(BackgroundColor);
            PrimarySegmentColor = Gdi32.FixBlackAlpha(PrimarySegmentColor);
            SecondarySegmentColor = Gdi32.FixBlackAlpha(SecondarySegmentColor);
            InactiveSegmentColor = Gdi32.FixBlackAlpha(InactiveSegmentColor);

            var primarySegmentColor = new COLORREF(PrimarySegmentColor);
            var secondarySegmentColor = new COLORREF(SecondarySegmentColor);
            var inactiveSegmentColor = new COLORREF(InactiveSegmentColor);

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
                Gdi32.FixAlphaChannel(alphadc, alphabitmap, (uint)Height, ref pixels, ref dib, new[] { backgroundColor });
                Gdi32.DoAlphaBlendToMemdc(memdc, ref rc, alphadc, BackgroundColor.A);

                // clear alphadc
                Gdi32.FillAlphadc(alphadc, ref rc, StockObjects.BLACK_BRUSH, StockObjects.BLACK_PEN);
            }

            Internal_PaintContent(alphadc, rc, primarySegmentColor, secondarySegmentColor, inactiveSegmentColor);
            Gdi32.FixAlphaChannel(alphadc, alphabitmap, (uint)Height, ref pixels, ref dib, new[] { primarySegmentColor, secondarySegmentColor, inactiveSegmentColor });
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
            int offset = 0; // HideBorders ? 0 : 2;

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

        private void Internal_PaintContent(IntPtr hdc, RECT rc, COLORREF primaryColor, COLORREF secondaryColor, COLORREF inactiveSegmentColor)
        {
            var primaryBrush = Gdi32.CreateSolidBrush(primaryColor);
            var secondaryBrush = Gdi32.CreateSolidBrush(secondaryColor);
            var inactiveBrush = Gdi32.CreateSolidBrush(inactiveSegmentColor);

            var oldBrush = Gdi32.SelectObject(hdc, primaryBrush);

            if (Range > 0)
            {
                int placeWidth = rc.Width / SegmentsCount;
                int segmentWidth = (int)Math.Ceiling((placeWidth / 100d) * SegmentSpaceRatio) + 1;
                int stripeX = (int)Math.Ceiling(segmentWidth / 3d);
                int signalX = rc.Right * _position / Range;
                int transitionX = rc.Right * TransitionPoint / Range;

                for (int i = 0; i < SegmentsCount; i++)
                {
                    int segmentX = placeWidth * i;

                    var brush = segmentX + segmentWidth <= signalX
                        ? _position >= TransitionPoint && segmentX >= transitionX ? secondaryBrush : primaryBrush
                        : inactiveBrush;

                    Gdi32.SelectObject(hdc, brush);
                    if (StripedSegments)
                    {
                        int x1 = segmentX; int w1 = segmentX + stripeX;
                        int x2 = x1 + stripeX; int w2 = x2 + stripeX;
                        int x3 = x2 + stripeX; int w3 = x3 + stripeX;

                        Gdi32.Rectangle(hdc, x1, rc.Top + PaddingTop, w1, rc.Bottom - PaddingBottom);
                        Gdi32.Rectangle(hdc, x2, rc.Top + PaddingTop, w2, rc.Bottom - PaddingBottom);
                        Gdi32.Rectangle(hdc, x3, rc.Top + PaddingTop, w3, rc.Bottom - PaddingBottom);
                    }
                    else
                    {
                        Gdi32.Rectangle(hdc, segmentX, rc.Top + PaddingTop, segmentX + segmentWidth, rc.Bottom - PaddingBottom);
                    }
                }
            }

            Gdi32.SelectObject(hdc, oldBrush);

            Gdi32.DeleteObject(inactiveBrush);
            Gdi32.DeleteObject(secondaryBrush);
            Gdi32.DeleteObject(primaryBrush);
        }
    }
}
