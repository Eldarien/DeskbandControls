using Deskband.Core.Common;
using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.Controls
{
    public partial class dcLabel : UserControl
    {
        private Timer _timer;

        private IntPtr _hFont;
        private int _dpi;
        private int _scrollPos;

        public dcLabel(int dpi, FontConfiguration fontConfiguration)
        {
            _dpi = dpi;
            _scrollPos = 0;

            BackColor = Color.Transparent;
            FontConfiguration = fontConfiguration;

            _timer = new Timer();
            _timer.Tick += (ts, te) => { _scrollPos += ScrollStep; Refresh(); };

            ScrollSpeed = 100;
            ScrollStep = 5;
            ScrollSeparator = " **** ";

            ShadowOffset = 2;
            BackgroundColor = Color.Transparent;
        }

        // properties

        private FontConfiguration _fontConfiguration;
        public FontConfiguration FontConfiguration
        {
            get { return _fontConfiguration; }
            set { _fontConfiguration = value; InitializeFont(); }
        }

        private bool _isTextRtl;
        [Bindable(false), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get { return base.Text; }
            set { if (base.Text != value) { _isTextRtl = WinApiHelpers.IsTextRtl(value); base.Text = value; Refresh(); } }
        }

        public HorizontalAlign TextAlign { get; set; }

        public bool DisplayShadow { get; set; }
        public Color ShadowColor { get; set; }
        public int ShadowOffset { get; set; }

        public Color BackgroundColor { get; set; }

        public bool DrawOutline { get; set; }

        public bool EnableScrolling
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; if (!value) { ResetScrollPosition(); } }
        }

        public int ScrollSpeed
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        public int ScrollStep { get; set; }

        public string ScrollSeparator { get; set; }

        public void ResetScrollPosition()
        {
            if (_scrollPos != 0)
            {
                _scrollPos = 0;
                Refresh();
            }
        }

        private Image _bkImage;
        private int _bkImageX;
        private int _bkImageY;
        public void SetBkImage(Image bkImage, int x, int y, int width, int height, bool preserveAR)
        {
            if (_bkImage != null)
            {
                _bkImage.Dispose();
                _bkImage = null;
            }
            if (bkImage != null)
            {
                Bitmap bmp = bkImage.Width != width || bkImage.Height != height
                    ? (Bitmap)ImageHelpers.HQResize(bkImage, width, height, preserveAR)
                    : new Bitmap(bkImage);

                _bkImage = bmp;
                _bkImageX = x;
                _bkImageY = y;
            }
        }

        // private methods

        private void InitializeFont()
        {
            int logPixelsY = _dpi;
            int logFontSize = -(int)Math.Round((_fontConfiguration.Size * logPixelsY) / 72.0);

            bool isBold = (_fontConfiguration.Styles & FontStyles.Bold) != 0;
            bool isItalic = (_fontConfiguration.Styles & FontStyles.Italic) != 0;

            _hFont = Gdi32.CreateFont(logFontSize, 0, 0, 0, isBold ? 700 : 400, isItalic ? 1u : 0u, 0, 0, 0, 0, 0, 0, 0, _fontConfiguration.Name);
        }

        // protected / public methods

        public override void Refresh()
        {
            User32.InvalidateRect(Handle, IntPtr.Zero, false);
        }

        protected override void Dispose(bool disposing)
        {
            Gdi32.DeleteObject(_hFont);
            _timer.Dispose();

            if (_bkImage != null && _bkImage != ImageHelpers.Empty)
            {
                _bkImage.Dispose();
                _bkImage = null;
            }

            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ForeColor = Gdi32.FixBlackAlpha(ForeColor);
            BackgroundColor = Gdi32.FixBlackAlpha(BackgroundColor);

            var hdc = e.Graphics.GetHdc();
            var memdc = Gdi32.CreateCompatibleDC(hdc);
            var hTheme = UxTheme.OpenThemeData(IntPtr.Zero, "BUTTON");
            var oldFont = Gdi32.SelectObject(memdc, _hFont);
            var rc = new RECT(ClientRectangle);
            var textColor = new COLORREF(ForeColor);

            var textFlags = DT_NOPREFIX;
            if (TextAlign == HorizontalAlign.Right)
                textFlags |= DT_RIGHT;
            else if (TextAlign == HorizontalAlign.Center)
                textFlags |= DT_CENTER;
            if (_isTextRtl)
                textFlags |= DT_RTLREADING;

            var dib = new BITMAPINFO();
            Gdi32.FillBitmapInfo(ref dib, rc.Width, rc.Height);

            var alphadc = Gdi32.CreateCompatibleDC(hdc);
            Gdi32.SelectObject(alphadc, _hFont);

            var alphabitmap = Gdi32.CreateDIBSection(alphadc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldalphaBitmap = Gdi32.SelectObject(alphadc, alphabitmap);
            Gdi32.FillAlphadc(alphadc, ref rc, StockObjects.HOLLOW_BRUSH, StockObjects.NULL_PEN);

            if (DwmApi.DwmIsCompositionEnabled())
            {
                var bitmap = Gdi32.CreateDIBSection(memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

                PaintBackground(memdc, rc, ref dib);
                if (DrawOutline) Gdi32.DrawOutline(memdc, ref rc);

                DTTOPTS opts = new DTTOPTS();
                opts.dwSize = (UInt32)Marshal.SizeOf(typeof(DTTOPTS));
                opts.dwFlags = DTT_COMPOSITED | DTT_TEXTCOLOR;

                var t = PrepareScrollText(alphadc, rc, textFlags);

                if (DisplayShadow)
                {
                    opts.crText = new COLORREF(ShadowColor);
                    UxTheme.DrawThemeTextEx(hTheme, alphadc, 0, 0, t.Text, t.Text.Length, t.TextFlags, ref t.ShadowRect, ref opts);
                }

                opts.crText = textColor;
                UxTheme.DrawThemeTextEx(hTheme, alphadc, 0, 0, t.Text, t.Text.Length, t.TextFlags, ref t.Rect, ref opts);

                Gdi32.DoAlphaBlendToMemdc(memdc, ref rc, alphadc, ForeColor.A);

                Gdi32.BitBlt(hdc, rc.Left, rc.Top, rc.Width, rc.Height, memdc, 0, 0, SRCCOPY);

                Gdi32.SelectObject(memdc, oldBitmap);
                Gdi32.DeleteObject(bitmap);
            }
            else
            {
                var bitmap = Gdi32.CreateCompatibleBitmap(hdc, rc.Right, rc.Bottom);
                var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

                PaintBackground(memdc, rc, ref dib);
                if (DrawOutline) Gdi32.DrawOutline(memdc, ref rc);

                var dtp = new DRAWTEXTPARAMS();
                dtp.cbSize = (UInt32)Marshal.SizeOf(typeof(DRAWTEXTPARAMS));

                var t = PrepareScrollText(memdc, rc, textFlags);
                Gdi32.SetBkMode(memdc, TRANSPARENT);

                if (DisplayShadow)
                {
                    Gdi32.SetTextColor(memdc, new COLORREF(ShadowColor));
                    User32.DrawTextEx(memdc, t.Text, t.Text.Length, ref t.ShadowRect, t.TextFlags, ref dtp);
                }

                Gdi32.SetTextColor(memdc, textColor);
                User32.DrawTextEx(memdc, t.Text, t.Text.Length, ref t.Rect, t.TextFlags, ref dtp);

                Gdi32.BitBlt(hdc, rc.Left, rc.Top, rc.Width, rc.Height, memdc, 0, 0, SRCCOPY);

                Gdi32.SelectObject(memdc, oldBitmap);
                Gdi32.DeleteObject(bitmap);
            }

            // Cleanup

            Gdi32.SelectObject(alphadc, oldalphaBitmap);
            Gdi32.DeleteObject(alphabitmap);
            Gdi32.ReleaseDC(alphadc, -1);
            Gdi32.DeleteDC(alphadc);

            Gdi32.SelectObject(memdc, oldFont);

            UxTheme.CloseThemeData(hTheme);

            Gdi32.ReleaseDC(memdc, -1);
            Gdi32.DeleteDC(memdc);

            e.Graphics.ReleaseHdc(hdc);
        }

        private void PaintBackground(IntPtr dc, RECT rc, ref BITMAPINFO dib)
        {
            if (_bkImage == null)
            {
                UxTheme.DrawThemeParentBackground(Handle, dc, ref rc);
            }
            else
            {
                var b = ((Bitmap)_bkImage).GetHbitmap(Color.Red);
                var bdc = Gdi32.CreateCompatibleDC(dc);
                Gdi32.SelectObject(bdc, b);
                Gdi32.BitBlt(dc, 0, 0, rc.Width, rc.Height, bdc, Left - _bkImageX, Top - _bkImageY, SRCCOPY);
                Gdi32.DeleteDC(bdc);
                Gdi32.DeleteObject(b);
            }
            if (BackgroundColor != Color.Transparent)
            {
                var bdc = Gdi32.CreateCompatibleDC(dc);
                var bitmap = Gdi32.CreateCompatibleBitmap(dc, rc.Right, rc.Bottom);
                Gdi32.SelectObject(bdc, bitmap);

                var color = new COLORREF(BackgroundColor);
                var brush = Gdi32.CreateSolidBrush(color);
                var pen = Gdi32.CreatePen(PenStyle.PS_SOLID, 1, color);
                Gdi32.SelectObject(bdc, brush);
                Gdi32.SelectObject(bdc, pen);
                Gdi32.Rectangle(bdc, 0, 0, rc.Width, rc.Height);

                var pixels = new COLORREF[Width * Height];
                Gdi32.FixAlphaChannel(bdc, bitmap, (uint)rc.Height, ref pixels, ref dib, color);
                Gdi32.DoAlphaBlendToMemdc(dc, ref rc, bdc, BackgroundColor.A);

                Gdi32.DeleteObject(pen);
                Gdi32.DeleteObject(brush);
                Gdi32.DeleteObject(bitmap);
                Gdi32.DeleteDC(bdc);
            }
        }

        public struct TextWithRect
        {
            public string Text;
            public RECT Rect;
            public uint TextFlags;
            public RECT ShadowRect;
        }

        private TextWithRect PrepareScrollText(IntPtr dc, RECT rc, uint textFlags)
        {
            var text = Text;
            if (_timer.Enabled)
            {
                var len = text.Length;
                var fullTextSize = Size.Empty;
                Gdi32.GetTextExtentPoint32(dc, text, len, out fullTextSize);
                if (fullTextSize.Width > rc.Width)
                {
                    var tsb = new StringBuilder();
                    var textSize = Size.Empty;
                    do
                    {
                        tsb.Append(Text);
                        tsb.Append(ScrollSeparator);
                        tsb.Append(Text);
                        tsb.Append(ScrollSeparator);
                        text = tsb.ToString();

                        Gdi32.GetTextExtentPoint32(dc, text, text.Length, out textSize);
                    } while (textSize.Width < Width * 2);
                    if (_scrollPos >= textSize.Width / 2) _scrollPos = 0;
                    rc = new RECT(rc.Left - _scrollPos, rc.Top, rc.Right, rc.Bottom);
                    textFlags &= ~(DT_CENTER | DT_RIGHT); // Always align to left when scrolling
                }
            }
            var shOffset = ShadowOffset;
            var shRect = new RECT(rc.Left + shOffset, rc.Top + shOffset, rc.Right + shOffset, rc.Bottom + shOffset);
            return new TextWithRect { Text = text, Rect = rc, TextFlags = textFlags, ShadowRect = shRect };
        }
    }
}