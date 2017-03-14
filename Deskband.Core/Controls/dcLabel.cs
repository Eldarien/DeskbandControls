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
            base.OnPaint(e);

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
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biHeight = -(rc.Bottom - rc.Top); // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = rc.Right - rc.Left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;

            var alphadc = Gdi32.CreateCompatibleDC(hdc);
            Gdi32.SelectObject(alphadc, _hFont);

            var alphabitmap = Gdi32.CreateDIBSection(alphadc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldalphaBitmap = Gdi32.SelectObject(alphadc, alphabitmap);
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.NULL_PEN));
            Gdi32.Rectangle(alphadc, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top);

            if (DwmApi.DwmIsCompositionEnabled())
            {
                var bitmap = Gdi32.CreateDIBSection(memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

                DTTOPTS opts = new DTTOPTS();
                opts.dwSize = (UInt32)Marshal.SizeOf(typeof(DTTOPTS));
                opts.dwFlags = DTT_COMPOSITED | DTT_TEXTCOLOR;
                opts.crText = textColor;

                UxTheme.DrawThemeParentBackground(Handle, memdc, ref rc);

                PaintOutline(memdc, rc);

                var t = PrepareScrollText(alphadc, rc, textFlags);
                UxTheme.DrawThemeTextEx(hTheme, alphadc, 0, 0, t.Text, t.Text.Length, t.TextFlags, ref t.Rect, ref opts);

                var blendFunc = new BLENDFUNCTION(AC_SRC_OVER, 0, ForeColor.A, AC_SRC_ALPHA);
                Gdi32.AlphaBlend(memdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, alphadc,
                    0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top,
                    blendFunc);

                Gdi32.BitBlt(hdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, memdc, 0, 0, SRCCOPY);

                Gdi32.SelectObject(memdc, oldBitmap);
                Gdi32.DeleteObject(bitmap);
            }
            else
            {
                var bitmap = Gdi32.CreateCompatibleBitmap(hdc, rc.Right, rc.Bottom);
                var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

                var dtp = new DRAWTEXTPARAMS();
                dtp.cbSize = (UInt32)Marshal.SizeOf(typeof(DRAWTEXTPARAMS));

                UxTheme.DrawThemeParentBackground(Handle, memdc, ref rc);

                PaintOutline(memdc, rc);

                Gdi32.SetTextColor(memdc, textColor);
                Gdi32.SetBkMode(memdc, TRANSPARENT);

                var t = PrepareScrollText(memdc, rc, textFlags);
                User32.DrawTextEx(memdc, t.Text, t.Text.Length, ref t.Rect, t.TextFlags, ref dtp);

                Gdi32.BitBlt(hdc, rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top, memdc, 0, 0, SRCCOPY);

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

        private void PaintOutline(IntPtr dc, RECT rc)
        {
            if (DrawOutline)
            {
                Gdi32.SelectObject(dc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
                Gdi32.SelectObject(dc, Gdi32.GetStockObject(StockObjects.WHITE_PEN));
                Gdi32.Rectangle(dc, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top);
            }
        }

        public struct TextWithRect
        {
            public string Text;
            public RECT Rect;
            public uint TextFlags;
        }

        private TextWithRect PrepareScrollText(IntPtr dc, RECT rc, uint textFlags)
        {
            var text = Text;
            if (_timer.Enabled)
            {
                var len = text.Length;
                var fullTextSize = Size.Empty;
                Gdi32.GetTextExtentPoint32(dc, text, len, out fullTextSize);
                if (fullTextSize.Width > rc.Right - rc.Left)
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
            return new TextWithRect { Text = text, Rect = rc, TextFlags = textFlags };
        }

        /*
        // old

        private bool isRtlText;

        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text != value)
                {
                    isRtlText = WinApiHelpers.IsTextRtl(value);
                    base.Text = value;

                    _lastRefresh = DateTime.MinValue; // force refresh
                    Refresh();
                }
            }
        }

        public bool AlignTextToRight { get; set; }

        public bool EnableScroll { get; set; }

        public bool DrawOutline { get; set; }

        private DateTime _lastRefresh;

        public override void Refresh()
        {
            var now = DateTime.Now;
            if ((now - _lastRefresh).TotalMilliseconds < 100)
                return;

            _lastRefresh = now;

            User32.InvalidateRect(this.Handle, IntPtr.Zero, false);
        }

        

        protected override void Dispose(bool disposing)
        {
            Gdi32.DeleteObject(_hFont);

            base.Dispose(disposing);
        }

        public void ScrollTick()
        {
            if (!EnableScroll)
            {
                _scrollPos = 0;
                return;
            }
            _scrollPos++;
            Refresh();
        }

        private string PrepareScrolledText(IntPtr hdc)
        {
            if (!EnableScroll)
            {
                return this.Text;
            }

            var len = Text.Length;
            var fullTextSize = Size.Empty;
            Gdi32.GetTextExtentPoint32(hdc, Text, len, out fullTextSize);
            if (fullTextSize.Width > this.Size.Width)
            {
                const string scrollSeparator = " **** ";
                int scrollSeparatorLen = scrollSeparator.Length;
                if (_scrollPos >= len + scrollSeparatorLen)
                    _scrollPos = 0;

                var textBuffer = new StringBuilder(Text.Length * 2 + scrollSeparatorLen * 2);
                textBuffer.Append(scrollSeparator);
                textBuffer.Append(Text);
                textBuffer.Append(scrollSeparator);
                textBuffer.Append(Text);

                bool rmode = AlignTextToRight && !isRtlText || !AlignTextToRight && isRtlText;
                int xlen = (len + scrollSeparatorLen) * 2 -
                    (rmode ? len + scrollSeparatorLen - _scrollPos : _scrollPos);

                return textBuffer.ToString(rmode ? 0 : _scrollPos, xlen);
            }
            else
            {
                return this.Text;
            }
        }

        //private IntPtr GetTextFont()
        //{
        //    const string rebarWindowClass = "ReBarWindow32";
        //    var hParent = this.Handle;
        //    var parentWindowClass = new StringBuilder(256);
        //    while (parentWindowClass.ToString() != rebarWindowClass)
        //    {
        //        hParent = WinApi.GetParent(hParent);
        //        if (hParent == IntPtr.Zero)
        //        {
        //            break;
        //        }
        //        parentWindowClass.Clear();
        //        WinApi.RealGetWindowClass(hParent, parentWindowClass, (uint)parentWindowClass.Capacity);
        //    }

        //    var hFont = Environment.OSVersion.Version.Major < 6 || hParent == IntPtr.Zero
        //        ? WinApi.GetStockObject(WinApi.StockObjects.DEFAULT_GUI_FONT)
        //        : (IntPtr)WinApi.SendMessage(hParent, WinApi.WM_GETFONT, IntPtr.Zero, IntPtr.Zero);

        //    return hFont;
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var hdc = e.Graphics.GetHdc();
            var memdc = Gdi32.CreateCompatibleDC(hdc);

            var hTheme = UxTheme.OpenThemeData(IntPtr.Zero, "BUTTON");

            //var hFont = this.Font.ToHfont();

            var oldFont = Gdi32.SelectObject(memdc, _hFont);

            var rc = new RECT(ClientRectangle);

            string text = PrepareScrolledText(memdc);

            var textColor = new COLORREF(this.ForeColor);

            var textFlags = DT_NOPREFIX;
            if (this.AlignTextToRight)
                textFlags |= DT_RIGHT;
            if (isRtlText)
                textFlags |= DT_RTLREADING;

            var dib = new BITMAPINFO();
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biHeight = -(rc.bottom - rc.top); // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = rc.right - rc.left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;

            var alphadc = Gdi32.CreateCompatibleDC(hdc);
            Gdi32.SelectObject(alphadc, _hFont);

            var alphabitmap = Gdi32.CreateDIBSection(alphadc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldalphaBitmap = Gdi32.SelectObject(alphadc, alphabitmap);
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(StockObjects.NULL_PEN));
            Gdi32.Rectangle(alphadc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);

            if (DwmApi.DwmIsCompositionEnabled())
            {
                var bitmap = Gdi32.CreateDIBSection(memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

                var opts = new DTTOPTS();
                opts.dwSize = (UInt32)Marshal.SizeOf(typeof(DTTOPTS));
                opts.dwFlags = DTT_COMPOSITED | DTT_TEXTCOLOR;
                opts.crText = textColor;

                UxTheme.DrawThemeParentBackground(Handle, memdc, ref rc);

                if (this.DrawOutline)
                {
                    Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
                    Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.WHITE_PEN));
                    Gdi32.Rectangle(memdc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
                }

                UxTheme.DrawThemeTextEx(hTheme, alphadc, 0, 0, text, text.Length, textFlags, ref rc, ref opts);

                var blendFunc = new BLENDFUNCTION(AC_SRC_OVER, 0, ForeColor.A, AC_SRC_ALPHA);
                Gdi32.AlphaBlend(memdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, alphadc,
                    0, 0, rc.right - rc.left, rc.bottom - rc.top,
                    blendFunc);

                Gdi32.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, SRCCOPY);

                Gdi32.SelectObject(memdc, oldBitmap);
                Gdi32.DeleteObject(bitmap);
            }
            else
            {
                var bitmap = Gdi32.CreateCompatibleBitmap(hdc, rc.right, rc.bottom);
                var oldBitmap = Gdi32.SelectObject(memdc, bitmap);

                var dtp = new DRAWTEXTPARAMS();
                dtp.cbSize = (UInt32)Marshal.SizeOf(typeof(DRAWTEXTPARAMS));

                UxTheme.DrawThemeParentBackground(Handle, memdc, ref rc);

                if (this.DrawOutline)
                {
                    Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
                    Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.WHITE_PEN));
                    Gdi32.Rectangle(memdc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
                }

                //WinApi.SetTextColor(alphadc, textColor);
                //WinApi.SetBkMode(alphadc, WinApi.TRANSPARENT);

                //WinApi.DrawTextEx(alphadc, text, text.Length, ref rc, textFlags, ref dtp);

                //var blendFunc = new WinApi.BLENDFUNCTION(WinApi.AC_SRC_OVER, 0, ForeColor.A, WinApi.AC_SRC_ALPHA);
                //WinApi.AlphaBlend(memdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, alphadc,
                //    0, 0, rc.right - rc.left, rc.bottom - rc.top,
                //    blendFunc);

                Gdi32.SetTextColor(memdc, textColor);
                Gdi32.SetBkMode(memdc, TRANSPARENT);
                User32.DrawTextEx(memdc, text, text.Length, ref rc, textFlags, ref dtp);

                Gdi32.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, SRCCOPY);

                Gdi32.SelectObject(memdc, oldBitmap);
                Gdi32.DeleteObject(bitmap);
            }

            // Cleanup

            Gdi32.SelectObject(alphadc, oldalphaBitmap);
            Gdi32.DeleteObject(alphabitmap);
            Gdi32.ReleaseDC(alphadc, -1);
            Gdi32.DeleteDC(alphadc);

            Gdi32.SelectObject(memdc, oldFont);
            //WinApi.DeleteObject(hFont);

            UxTheme.CloseThemeData(hTheme);

            Gdi32.ReleaseDC(memdc, -1);
            Gdi32.DeleteDC(memdc);

            e.Graphics.ReleaseHdc(hdc);
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
        */
    }
}