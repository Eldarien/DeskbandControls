using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.WinApi
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hdc, int state);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern int SaveDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
        public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
            int nWidthDest, int nHeightDest,
            IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
            BLENDFUNCTION blendFunction);

        [DllImport("gdi32.dll")]
        public static extern uint SetTextColor(IntPtr hdc, COLORREF crColor);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, int iBkMode);

        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(StockObjects fnObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out System.Drawing.Size lpSize);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(PenStyle fnPenStyle, int nWidth, COLORREF crColor);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(COLORREF crColor);

        [DllImport("gdi32.dll")]
        public static extern bool RoundRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan,
           uint cScanLines, [Out] COLORREF[] lpvBits, ref BITMAPINFO lpbmi, uint uUsage);

        [DllImport("gdi32.dll")]
        public static extern int SetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint
           cScanLines, COLORREF[] lpvBits, [In] ref BITMAPINFO lpbmi, uint fuColorUse);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement,
           int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline,
           uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision,
           uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public static System.Drawing.Color FixBlackAlpha(System.Drawing.Color color)
        {
            if (color.R == 0 && color.G == 0 && color.B == 0)
                color = System.Drawing.Color.FromArgb(color.A, 1, 1, 1);
            return color;
        }

        public static void FillBitmapInfo(ref BITMAPINFO dib, int width, int height)
        {
            dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            dib.bmiHeader.biHeight = -height; // negative because GDI functions use a top-down DIB
            dib.bmiHeader.biWidth = width;
            dib.bmiHeader.biPlanes = 1;
            dib.bmiHeader.biBitCount = 32;
            dib.bmiHeader.biCompression = BI_RGB;
        }

        public static void FixAlphaChannel(IntPtr alphadc, IntPtr alphabitmap, uint lines, ref COLORREF[] pixels, ref BITMAPINFO dib, COLORREF[] colorsToFix)
        {
            var rgbColorsToFix = colorsToFix
                .Select(x => (x.ColorDWORD & 0x000000FF) << 16 | (x.ColorDWORD & 0x0000FF00) | (x.ColorDWORD & 0x00FF0000) >> 16)
                .ToArray();

            Gdi32.GetDIBits(alphadc, alphabitmap, 0, lines, pixels, ref dib, 0);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (rgbColorsToFix.Contains(pixels[i].ColorDWORD))
                    pixels[i].ColorDWORD |= 0xFF000000;
            }
            Gdi32.SetDIBits(alphadc, alphabitmap, 0, lines, pixels, ref dib, 0);
        }

        public static void FillAlphadc(IntPtr alphadc, ref RECT rc, StockObjects brush, StockObjects pen)
        {
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(brush));
            Gdi32.SelectObject(alphadc, Gdi32.GetStockObject(pen));
            Gdi32.Rectangle(alphadc, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top);
        }

        public static void DoAlphaBlendToMemdc(IntPtr memdc, ref RECT rc, IntPtr alphadc, byte alpha)
        {
            var blendFunc = new BLENDFUNCTION(AC_SRC_OVER, 0, alpha, AC_SRC_ALPHA);
            Gdi32.AlphaBlend(memdc, rc.Left, rc.Top, rc.Width, rc.Height,
                alphadc, 0, 0, rc.Width, rc.Height,
                blendFunc);
        }

        public static void DrawOutline(IntPtr memdc, ref RECT rc)
        {
            Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
            Gdi32.SelectObject(memdc, Gdi32.GetStockObject(StockObjects.WHITE_PEN));
            Gdi32.Rectangle(memdc, 0, 0, rc.Width, rc.Height);
        }
    }
}
