using Deskband.Common.Extensions;
using Deskband.Native;
using Deskband.Settings.Models;
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

namespace Deskband.Controls
{
    public partial class AeroLabel : UserControl
    {
        private int scrollPos;
        private bool isRtlText;

        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text != value)
                {
                    isRtlText = value.IsRtlText();
                    base.Text = value;

                    _lastRefresh = DateTime.MinValue; // force refresh
                    Refresh();
                }
            }
        }

        public string StoppedText { get; set; }

        public string Format { get; set; }

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

            WinApi.InvalidateRect(this.Handle, IntPtr.Zero, false);
        }

        private IntPtr _hFont;

        public AeroLabel(String fontName, int fontSize, bool italic, bool bold)
        {
            this.BackColor = Color.Transparent;
            scrollPos = 0;

            int logPixelsY = WinApi.GetDeviceCaps(WinApi.GetDC(IntPtr.Zero), WinApi.LOGPIXELSY);
            int logFontSize = -(int)Math.Round((fontSize * logPixelsY) / 72.0);
            _hFont = WinApi.CreateFont(logFontSize, 0, 0, 0, bold ? 700 : 400, italic ? 1u : 0u, 0, 0, 0, 0, 0, 0, 0, fontName);
        }

        protected override void Dispose(bool disposing)
        {
            WinApi.DeleteObject(_hFont);

            base.Dispose(disposing);
        }

        public void ScrollTick()
        {
            if (!EnableScroll)
            {
                scrollPos = 0;
                return;
            }
            scrollPos++;
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
            WinApi.GetTextExtentPoint32(hdc, Text, len, out fullTextSize);
            if (fullTextSize.Width > this.Size.Width)
            {
                const string scrollSeparator = " **** ";
                int scrollSeparatorLen = scrollSeparator.Length;
                if (scrollPos >= len + scrollSeparatorLen)
                    scrollPos = 0;

                var textBuffer = new StringBuilder(Text.Length * 2 + scrollSeparatorLen * 2);
                textBuffer.Append(scrollSeparator);
                textBuffer.Append(Text);
                textBuffer.Append(scrollSeparator);
                textBuffer.Append(Text);

                bool rmode = AlignTextToRight && !isRtlText || !AlignTextToRight && isRtlText;
                int xlen = (len + scrollSeparatorLen) * 2 -
                    (rmode ? len + scrollSeparatorLen - scrollPos : scrollPos);

                return textBuffer.ToString(rmode ? 0 : scrollPos, xlen);
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
            var memdc = WinApi.CreateCompatibleDC(hdc);

            var hTheme = WinApi.OpenThemeData(IntPtr.Zero, "BUTTON");

            //var hFont = this.Font.ToHfont();

            var oldFont = WinApi.SelectObject(memdc, _hFont);

            var rc = new WinApi.RECT(ClientRectangle);

            string text = PrepareScrolledText(memdc);

            var textColor = new WinApi.COLORREF(this.ForeColor);

            var textFlags = WinApi.DT_NOPREFIX;
            if (this.AlignTextToRight)
                textFlags |= WinApi.DT_RIGHT;
            if (isRtlText)
                textFlags |= WinApi.DT_RTLREADING;

            var dib = new WinApi.BITMAPINFO();
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(WinApi.BITMAPINFOHEADER));
            dib.bmiHeader.biHeight = -(rc.bottom - rc.top); // negative because DrawThemeTextEx() uses a top-down DIB
            dib.bmiHeader.biWidth = rc.right - rc.left;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = WinApi.BI_RGB;

            var alphadc = WinApi.CreateCompatibleDC(hdc);
            WinApi.SelectObject(alphadc, _hFont);

            var alphabitmap = WinApi.CreateDIBSection(alphadc, ref dib, WinApi.DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
            var oldalphaBitmap = WinApi.SelectObject(alphadc, alphabitmap);
            WinApi.SelectObject(alphadc, WinApi.GetStockObject(WinApi.StockObjects.HOLLOW_BRUSH));
            WinApi.SelectObject(alphadc, WinApi.GetStockObject(WinApi.StockObjects.NULL_PEN));
            WinApi.Rectangle(alphadc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);

            if (WinApi.DwmIsCompositionEnabled())
            {
                var bitmap = WinApi.CreateDIBSection(memdc, ref dib, WinApi.DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                var oldBitmap = WinApi.SelectObject(memdc, bitmap);

                WinApi.DTTOPTS opts = new WinApi.DTTOPTS();
                opts.dwSize = (UInt32)Marshal.SizeOf(typeof(WinApi.DTTOPTS));
                opts.dwFlags = WinApi.DTT_COMPOSITED | WinApi.DTT_TEXTCOLOR;
                opts.crText = textColor;

                WinApi.DrawThemeParentBackground(Handle, memdc, ref rc);

                if (this.DrawOutline)
                {
                    WinApi.SelectObject(memdc, WinApi.GetStockObject(WinApi.StockObjects.HOLLOW_BRUSH));
                    WinApi.SelectObject(memdc, WinApi.GetStockObject(WinApi.StockObjects.WHITE_PEN));
                    WinApi.Rectangle(memdc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
                }

                WinApi.DrawThemeTextEx(hTheme, alphadc, 0, 0, text, text.Length, textFlags, ref rc, ref opts);

                var blendFunc = new WinApi.BLENDFUNCTION(WinApi.AC_SRC_OVER, 0, ForeColor.A, WinApi.AC_SRC_ALPHA);
                WinApi.AlphaBlend(memdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, alphadc,
                    0, 0, rc.right - rc.left, rc.bottom - rc.top,
                    blendFunc);

                WinApi.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, WinApi.SRCCOPY);

                WinApi.SelectObject(memdc, oldBitmap);
                WinApi.DeleteObject(bitmap);
            }
            else
            {
                var bitmap = WinApi.CreateCompatibleBitmap(hdc, rc.right, rc.bottom);
                var oldBitmap = WinApi.SelectObject(memdc, bitmap);

                var dtp = new WinApi.DRAWTEXTPARAMS();
                dtp.cbSize = (UInt32)Marshal.SizeOf(typeof(WinApi.DRAWTEXTPARAMS));

                WinApi.DrawThemeParentBackground(Handle, memdc, ref rc);

                if (this.DrawOutline)
                {
                    WinApi.SelectObject(memdc, WinApi.GetStockObject(WinApi.StockObjects.HOLLOW_BRUSH));
                    WinApi.SelectObject(memdc, WinApi.GetStockObject(WinApi.StockObjects.WHITE_PEN));
                    WinApi.Rectangle(memdc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
                }

                //WinApi.SetTextColor(alphadc, textColor);
                //WinApi.SetBkMode(alphadc, WinApi.TRANSPARENT);

                //WinApi.DrawTextEx(alphadc, text, text.Length, ref rc, textFlags, ref dtp);

                //var blendFunc = new WinApi.BLENDFUNCTION(WinApi.AC_SRC_OVER, 0, ForeColor.A, WinApi.AC_SRC_ALPHA);
                //WinApi.AlphaBlend(memdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, alphadc,
                //    0, 0, rc.right - rc.left, rc.bottom - rc.top,
                //    blendFunc);

                WinApi.SetTextColor(memdc, textColor);
                WinApi.SetBkMode(memdc, WinApi.TRANSPARENT);
                WinApi.DrawTextEx(memdc, text, text.Length, ref rc, textFlags, ref dtp);

                WinApi.BitBlt(hdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, memdc, 0, 0, WinApi.SRCCOPY);

                WinApi.SelectObject(memdc, oldBitmap);
                WinApi.DeleteObject(bitmap);
            }

            // Cleanup

            WinApi.SelectObject(alphadc, oldalphaBitmap);
            WinApi.DeleteObject(alphabitmap);
            WinApi.ReleaseDC(alphadc, -1);
            WinApi.DeleteDC(alphadc);

            WinApi.SelectObject(memdc, oldFont);
            //WinApi.DeleteObject(hFont);

            WinApi.CloseThemeData(hTheme);

            WinApi.ReleaseDC(memdc, -1);
            WinApi.DeleteDC(memdc);

            e.Graphics.ReleaseHdc(hdc);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_NCHITTEST)
            {
                m.Result = (IntPtr)WinApi.HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        public static AeroLabel Create(TextBlockModel model, bool outline)
        {
            var label = new AeroLabel(model.FontName, (int)model.FontSize, model.Italic, model.Bold);
            label.DrawOutline = outline;
            label.Text = "";
            label.StoppedText = model.StoppedText ?? "";
            label.Format = model.Format;
            label.ForeColor = model.FontColor.AsDrawingColor(); //ColorHelpers.GetThemedColor(x.FontColor);
            label.Location = new Point(model.X, model.Y);
            label.Size = new Size(model.Width, model.Height);
            label.EnableScroll = model.Scroll;
            label.AlignTextToRight = model.AlignToRight;
            label.Visible = model.Visible;
            return label;
        }
    }
}