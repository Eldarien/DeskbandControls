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
    }
}
