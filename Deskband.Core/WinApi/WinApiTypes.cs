using System;
using System.Runtime.InteropServices;

namespace Deskband.Core.WinApi
{
    public static class WinApiTypes
    {
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

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

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

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;

            public RECT(System.Drawing.Rectangle rect)
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

            public System.Drawing.Point AsPoint()
            {
                return new System.Drawing.Point(X, Y);
            }
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
        public const int USER_DEFAULT_SCREEN_DPI = 96;

        // HitTest

        public const int HT_CAPTION = 0x2;
        public const int HTTRANSPARENT = (-1);

        // Window pos

        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;

        // blend operation
        public const int AC_SRC_OVER = 0x00;

        // alpha format
        public const int AC_SRC_ALPHA = 0x01;

        // SystemParametersInfo
        public const int SPI_SETWORKAREA = 0x002F;

        // ShowWindow

        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;

        // Messages

        public const int WM_ACTIVATE = 0x0006;
        public const int WM_SETCURSOR = 0x0020;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_COMMAND = 0x111;
        public const int WM_PAINT = 0xf;
        public const int WM_GETFONT = 0x0031;
        public const int WM_THEMECHANGED = 0x031A;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_COPYDATA = 0x4A;
        public const int WM_DPICHANGED = 0x02E0;
        public const int WM_SETTINGCHANGE = 0x001A;

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        // Hooks

        public const int WH_MOUSE_LL = 14;

        public delegate IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Explicit)]
        public struct HookMouseStruct
        {
            [FieldOffset(0x00)]
            public POINT Point;

            /// <summary>
            ///     Specifies information associated with the message.
            /// </summary>
            /// <remarks>
            ///     The possible values are:
            ///     <list type="bullet">
            ///         <item>
            ///             <description>0 - No Information</description>
            ///         </item>
            ///         <item>
            ///             <description>1 - X-Button1 Click</description>
            ///         </item>
            ///         <item>
            ///             <description>2 - X-Button2 Click</description>
            ///         </item>
            ///         <item>
            ///             <description>120 - Mouse Scroll Away from User</description>
            ///         </item>
            ///         <item>
            ///             <description>-120 - Mouse Scroll Toward User</description>
            ///         </item>
            ///     </list>
            /// </remarks>
            [FieldOffset(0x0A)]
            public Int16 MouseData;

            /// <summary>
            ///     Returns a Timestamp associated with the input, in System Ticks.
            /// </summary>
            [FieldOffset(0x10)]
            public Int32 Timestamp;
        }
    }
}
