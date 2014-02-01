using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Deskband.Native
{
    public static class WinApi
    {
        private static class InternalApi
        {
            [DllImport("dwmapi.dll", PreserveSig = false)]
            public static extern bool DwmIsCompositionEnabled();

            [DllImport("dwmapi.dll")]
            public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margin);
        }

        // User32

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint RealGetWindowClass(IntPtr hwnd, StringBuilder pszType, uint cchType);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        //For use with WM_COPYDATA and COPYDATASTRUCT
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int DrawTextEx(IntPtr hdc, String lpchText, int cchText,
           ref RECT lprc, uint dwDTFormat, ref DRAWTEXTPARAMS lpDTParams);

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        // DWM

        public static bool DwmIsCompositionEnabled()
        {
            if (Environment.OSVersion.Version.Major < 6)
                return false;
            else
                return InternalApi.DwmIsCompositionEnabled();
        }

        public static int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margin)
        {
            if (Environment.OSVersion.Version.Major < 6)
                return 0;
            else
                return InternalApi.DwmExtendFrameIntoClientArea(hwnd, ref margin);
        }

        // UxTheme

        [DllImport("uxtheme.dll", ExactSpelling = true)]
        public static extern Int32 DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref RECT pRect);

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenThemeData(IntPtr hWnd, String classList);

        [DllImport("uxtheme.dll", ExactSpelling = true)]
        public static extern Int32 CloseThemeData(IntPtr hTheme);

        [DllImport("uxtheme.dll", ExactSpelling = true)]
        public static extern Int32 GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out COLORREF pColor);

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern Int32 DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, String text, int length, UInt32 flags, ref RECT rect, ref DTTOPTS poptions);

        // GDI

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
        public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);

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

        // Kernel

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetStringTypeW(CharacterTypes dwInfoType, string lpSrcStr, int cchSrc, [Out] CharacterTypeFlags[] lpCharType);

        // Shell

        [DllImport("Shell32.dll")]
        public static extern int ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirecotry, int nShowCmd);

        // Structs and Enums

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;

            public RECT(Rectangle rect)
            {
                this.left = rect.Left;
                this.top = rect.Top;
                this.right = rect.Right;
                this.bottom = rect.Bottom;
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int Length;

            public int Flags;

            public int ShowCmd;

            public POINT MinPosition;

            public POINT MaxPosition;

            public RECT NormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DTTOPTS
        {
            public UInt32 dwSize;
            public UInt32 dwFlags;
            public COLORREF crText;
            public COLORREF crBorder;
            public COLORREF crShadow;
            public int iTextShadowType;
            public int ptShadowOffsetX;
            public int ptShadowOffsetY;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public bool fApplyOverlay;
            public int iGlowSize;
            public IntPtr pfnDrawTextCallback;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COLORREF
        {
            public uint ColorDWORD;

            public COLORREF(uint colorDword)
            {
                ColorDWORD = colorDword;
            }

            public COLORREF(System.Drawing.Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }

            public System.Drawing.Color GetColor()
            {
                return System.Drawing.Color.FromArgb((int)(0x000000FFU & ColorDWORD),
               (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
            }

            public void SetColor(System.Drawing.Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DRAWTEXTPARAMS
        {
            public uint cbSize;
            public int iTabLength;
            public int iLeftMargin;
            public int iRightMargin;
            public uint uiLengthDrawn;
        }

        // DrawText() Format Flags

        public const UInt32 DT_TOP = 0x00000000;
        public const UInt32 DT_LEFT = 0x00000000;
        public const UInt32 DT_CENTER = 0x00000001;
        public const UInt32 DT_RIGHT = 0x00000002;
        public const UInt32 DT_VCENTER = 0x00000004;
        public const UInt32 DT_BOTTOM = 0x00000008;
        public const UInt32 DT_WORDBREAK = 0x00000010;
        public const UInt32 DT_SINGLELINE = 0x00000020;
        public const UInt32 DT_EXPANDTABS = 0x00000040;
        public const UInt32 DT_TABSTOP = 0x00000080;
        public const UInt32 DT_NOCLIP = 0x00000100;
        public const UInt32 DT_EXTERNALLEADING = 0x00000200;
        public const UInt32 DT_CALCRECT = 0x00000400;
        public const UInt32 DT_NOPREFIX = 0x00000800;
        public const UInt32 DT_INTERNAL = 0x00001000;
        public const UInt32 DT_EDITCONTROL = 0x00002000;
        public const UInt32 DT_PATH_ELLIPSIS = 0x00004000;
        public const UInt32 DT_END_ELLIPSIS = 0x00008000;
        public const UInt32 DT_MODIFYSTRING = 0x00010000;
        public const UInt32 DT_RTLREADING = 0x00020000;
        public const UInt32 DT_WORD_ELLIPSIS = 0x00040000;

        //---- bits used in dwFlags of DTTOPTS ----

        public const UInt32 DTT_TEXTCOLOR = (1U << 0);      // crText has been specified
        public const UInt32 DTT_BORDERCOLOR = (1U << 1);      // crBorder has been specified
        public const UInt32 DTT_SHADOWCOLOR = (1U << 2);      // crShadow has been specified
        public const UInt32 DTT_SHADOWTYPE = (1U << 3);      // iTextShadowType has been specified
        public const UInt32 DTT_SHADOWOFFSET = (1U << 4);      // ptShadowOffset has been specified
        public const UInt32 DTT_BORDERSIZE = (1U << 5);      // iBorderSize has been specified
        public const UInt32 DTT_FONTPROP = (1U << 6);      // iFontPropId has been specified
        public const UInt32 DTT_COLORPROP = (1U << 7);      // iColorPropId has been specified
        public const UInt32 DTT_STATEID = (1U << 8);      // IStateId has been specified
        public const UInt32 DTT_CALCRECT = (1U << 9);      // Use pRect as and in/out parameter
        public const UInt32 DTT_APPLYOVERLAY = (1U << 10);     // fApplyOverlay has been specified
        public const UInt32 DTT_GLOWSIZE = (1U << 11);     // iGlowSize has been specified
        public const UInt32 DTT_CALLBACK = (1U << 12);     // pfnDrawTextCallback has been specified
        public const UInt32 DTT_COMPOSITED = (1U << 13);     // Draws text with antialiased alpha (needs a DIB section)

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        };

        // Consts for CreateDIBSection

        public const int BI_RGB = 0;
        public const int DIB_RGB_COLORS = 0; //color table in RGBs

        // Const for BitBlt

        public const int SRCCOPY = 0x00CC0020;

        // SetBkMode consts

        public const int TRANSPARENT = 1;
        public const int OPAQUE = 2;

        // Taskband Parts

        public const int TDP_GROUPCOUNT = 1;
        public const int TDP_FLASHBUTTON = 2;
        public const int TDP_FLASHBUTTONGROUPMENU = 3;

        // Theme Parts

        public const int TMT_TEXTCOLOR = 3803;

        // Device Caps

        public const int LOGPIXELSY = 90;

        // Stock Objects

        public enum StockObjects
        {
            WHITE_BRUSH = 0,
            LTGRAY_BRUSH = 1,
            GRAY_BRUSH = 2,
            DKGRAY_BRUSH = 3,
            BLACK_BRUSH = 4,
            NULL_BRUSH = 5,
            HOLLOW_BRUSH = NULL_BRUSH,
            WHITE_PEN = 6,
            BLACK_PEN = 7,
            NULL_PEN = 8,
            OEM_FIXED_FONT = 10,
            ANSI_FIXED_FONT = 11,
            ANSI_VAR_FONT = 12,
            SYSTEM_FONT = 13,
            DEVICE_DEFAULT_FONT = 14,
            DEFAULT_PALETTE = 15,
            SYSTEM_FIXED_FONT = 16,
            DEFAULT_GUI_FONT = 17,
            DC_BRUSH = 18,
            DC_PEN = 19,
        }

        //  Character Type Flags
        [Flags]
        public enum CharacterTypes : uint
        {
            CT_CTYPE1 = 0x00000001,  // ctype 1 information
            CT_CTYPE2 = 0x00000002,  // ctype 2 information
            CT_CTYPE3 = 0x00000004  // ctype 3 information
        }

        //  CType 2 Flag Bits.
        [Flags]
        public enum CharacterTypeFlags : ushort
        {
            C2_LEFTTORIGHT = 0x0001,      // left to right
            C2_RIGHTTOLEFT = 0x0002,      // right to left
            C2_EUROPENUMBER = 0x0003,      // European number, digit
            C2_EUROPESEPARATOR = 0x0004,      // European numeric separator
            C2_EUROPETERMINATOR = 0x0005,      // European numeric terminator
            C2_ARABICNUMBER = 0x0006,      // Arabic number
            C2_COMMONSEPARATOR = 0x0007,      // common numeric separator
            C2_BLOCKSEPARATOR = 0x0008,      // block separator
            C2_SEGMENTSEPARATOR = 0x0009,      // segment separator
            C2_WHITESPACE = 0x000A,      // white space
            C2_OTHERNEUTRAL = 0x000B,      // other neutrals
            C2_NOTAPPLICABLE = 0x0000      // no implicit directionality
        }

        public enum PenStyle : int
        {
            PS_SOLID = 0, //The pen is solid.
            PS_DASH = 1, //The pen is dashed.
            PS_DOT = 2, //The pen is dotted.
            PS_DASHDOT = 3, //The pen has alternating dashes and dots.
            PS_DASHDOTDOT = 4, //The pen has alternating dashes and double dots.
            PS_NULL = 5, //The pen is invisible.
            PS_INSIDEFRAME = 6,// Normally when the edge is drawn, it’s centred on the outer edge meaning that half the width of the pen is drawn

            // outside the shape’s edge, half is inside the shape’s edge. When PS_INSIDEFRAME is specified the edge is drawn
            //completely inside the outer edge of the shape.
            PS_USERSTYLE = 7,

            PS_ALTERNATE = 8,
            PS_STYLE_MASK = 0x0000000F,

            PS_ENDCAP_ROUND = 0x00000000,
            PS_ENDCAP_SQUARE = 0x00000100,
            PS_ENDCAP_FLAT = 0x00000200,
            PS_ENDCAP_MASK = 0x00000F00,

            PS_JOIN_ROUND = 0x00000000,
            PS_JOIN_BEVEL = 0x00001000,
            PS_JOIN_MITER = 0x00002000,
            PS_JOIN_MASK = 0x0000F000,

            PS_COSMETIC = 0x00000000,
            PS_GEOMETRIC = 0x00010000,
            PS_TYPE_MASK = 0x000F0000
        }

        // HitTest

        public const int HT_CAPTION = 0x2;
        public const int HTTRANSPARENT = (-1);

        // Window pos

        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            //[MarshalAs(UnmanagedType.LPStr)]
            //public string lpData;
            public IntPtr lpData;
        }

        public struct MARGINS
        {
            public int m_Left;
            public int m_Right;
            public int m_Top;
            public int m_Buttom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            private byte BlendOp;
            private byte BlendFlags;
            private byte SourceConstantAlpha;
            private byte AlphaFormat;

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        // blend operation
        public const int AC_SRC_OVER = 0x00;

        // alpha format
        public const int AC_SRC_ALPHA = 0x01;

        // ShowWindow

        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;

        // Messages

        public const int WM_SETCURSOR = 0x0020;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_COMMAND = 0x111;
        public const int WM_PAINT = 0xf;
        public const int WM_GETFONT = 0x0031;
        public const int WM_THEMECHANGED = 0x031A;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_COPYDATA = 0x4A;
    }
}