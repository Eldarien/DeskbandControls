using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Deskband.Native
{
    public static class ComTypes
    {
        public const UInt32 S_OK = 0;
        public const UInt32 S_FALSE = 1;
        public const UInt32 E_NOTIMPL = 0x80004001;

        [Flags]
        public enum DBIM : uint
        {
            MINSIZE = 0x0001,
            MAXSIZE = 0x0002,
            INTEGRAL = 0x0004,
            ACTUAL = 0x0008,
            TITLE = 0x0010,
            MODEFLAGS = 0x0020,
            BKCOLOR = 0x0040
        }

        [Flags]
        public enum DBIMF : uint
        {
            NORMAL = 0x0000,
            FIXED = 0x0001,
            FIXEDBMP = 0x0004,
            VARIABLEHEIGHT = 0x0008,
            UNDELETEABLE = 0x0010,
            DEBOSSED = 0x0020,
            BKCOLOR = 0x0040,
            USECHEVRON = 0x0080,
            BREAK = 0x0100,
            ADDTOFRONT = 0x0200,
            TOPALIGN = 0x0400,
            NOGRIPPER = 0x0800,
            ALWAYSGRIPPER = 0x1000,
            NOMARGINS = 0x2000,
            VIEWMODE_NORMAL = 0x0000,
            VIEWMODE_VERTICAL = 0x0001,
            VIEWMODE_FLOATING = 0x0002,
            VIEWMODE_TRANSPARENT = 0x0004
        }

        public struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        public struct MSG
        {
            public IntPtr hwnd;
            public UInt32 message;
            public UInt32 wParam;
            public Int32 lParam;
            public UInt32 time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DESKBANDINFO
        {
            public UInt32 dwMask;
            public Point ptMinSize;
            public Point ptMaxSize;
            public Point ptIntegral;
            public Point ptActual;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public String wszTitle;

            public DBIM dwModeFlags;
            public Int32 crBkgnd;
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")]
        public interface IObjectWithSite
        {
            void SetSite([In, MarshalAs(UnmanagedType.IUnknown)] Object pUnkSite);

            void GetSite(ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out Object ppvSite);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00000114-0000-0000-C000-000000000046")]
        public interface IOleWindow
        {
            void GetWindow(out IntPtr phwnd);

            void ContextSensitiveHelp([In] bool fEnterMode);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("012dd920-7b26-11d0-8ca9-00a0c92dbfe8")]
        public interface IDockingWindow
        {
            void GetWindow(out System.IntPtr phwnd);

            void ContextSensitiveHelp([In] bool fEnterMode);

            void ShowDW([In] bool fShow);

            void CloseDW([In] UInt32 dwReserved);

            void ResizeBorderDW(IntPtr prcBorder, [In, MarshalAs(UnmanagedType.IUnknown)] Object punkToolbarSite, bool fReserved);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EB0FE172-1A3A-11D0-89B3-00A0C90A90AC")]
        public interface IDeskBand
        {
            void GetWindow(out System.IntPtr phwnd);

            void ContextSensitiveHelp([In] bool fEnterMode);

            void ShowDW([In] bool fShow);

            void CloseDW([In] UInt32 dwReserved);

            void ResizeBorderDW(IntPtr prcBorder, [In, MarshalAs(UnmanagedType.IUnknown)] Object punkToolbarSite, bool fReserved);

            void GetBandInfo(UInt32 dwBandID, UInt32 dwViewMode, ref DESKBANDINFO pdbi);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("79D16DE4-ABEE-4021-8D9D-9169B261D657")]
        public interface IDeskBand2 : IDeskBand
        {
            new void GetWindow(out System.IntPtr phwnd);

            new void ContextSensitiveHelp([In] bool fEnterMode);

            new void ShowDW([In] bool fShow);

            new void CloseDW([In] UInt32 dwReserved);

            new void ResizeBorderDW(IntPtr prcBorder, [In, MarshalAs(UnmanagedType.IUnknown)] Object punkToolbarSite, bool fReserved);

            new void GetBandInfo(UInt32 dwBandID, UInt32 dwViewMode, ref DESKBANDINFO pdbi);

            void CanRenderComposited(ref bool pfCanRenderComposited);

            void SetCompositionState([MarshalAs(UnmanagedType.Bool)] bool fCompositionEnabled);

            void GetCompositionState(ref bool pfCompositionEnabled);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("0000010c-0000-0000-C000-000000000046")]
        public interface IPersist
        {
            void GetClassID([Out] out Guid pClassID);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00000109-0000-0000-C000-000000000046")]
        public interface IPersistStream : IPersist
        {
            new void GetClassID([Out] out Guid pClassID);

            [PreserveSig]
            UInt32 IsDirty();

            void Load([In, MarshalAs(UnmanagedType.Interface)] Object pStm);

            void Save([In, MarshalAs(UnmanagedType.Interface)] Object pStm, [In] bool fClearDirty);

            void GetSizeMax([In, Out] ref UInt64 pcbSize);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("68284faa-6a48-11d0-8c78-00c04fd918b4")]
        public interface IInputObject
        {
            void UIActivateIO(Int32 fActivate, ref MSG msg);

            [PreserveSig]
            UInt32 HasFocusIO();

            [PreserveSig]
            UInt32 TranslateAcceleratorIO(ref MSG msg);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("f1db8392-7331-11d0-8c99-00a0c92dbfe8")]
        public interface IInputObjectSite
        {
            [PreserveSig]
            Int32 OnFocusChangeIS([MarshalAs(UnmanagedType.IUnknown)] Object punkObj, Int32 fSetFocus);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OLECMD
        {
            public uint cmdID;
            public uint cmdf;
        }

        public enum OLECMDEXECOPT : uint
        {
            OLECMDEXECOPT_DODEFAULT = 0,
            OLECMDEXECOPT_PROMPTUSER = 1,
            OLECMDEXECOPT_DONTPROMPTUSER = 2,
            OLECMDEXECOPT_SHOWHELP = 3
        }

        public enum DESKBANDCID : uint
        {
            DBID_BANDINFOCHANGED = 0,
            DBID_SHOWONLY = 1,
            DBID_MAXIMIZEBAND = 2,
            DBID_PUSHCHEVRON = 3,
            DBID_DELAYINIT = 4,
            DBID_FINISHINIT = 5,
            DBID_SETWINDOWTHEME = 6,
            DBID_PERMITAUTOHIDE = 7
        }

        public static Guid CGID_DeskBand = new Guid("EB0FE172-1A3A-11D0-89B3-00A0C90A90AC");

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
        public interface IOleCommandTarget
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryStatus(
                [In] IntPtr pguidCmdGroup,
                [In, MarshalAs(UnmanagedType.U4)] uint cCmds,
                [In, Out, MarshalAs(UnmanagedType.Struct)] ref OLECMD prgCmds,
                //This parameter must be IntPtr, as it can be null
                [In, Out] IntPtr pCmdText);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Exec(
                [In] ref Guid pguidCmdGroup,
                //[In] IntPtr pguidCmdGroup,
                [In, MarshalAs(UnmanagedType.U4)] uint nCmdID,
                [In, MarshalAs(UnmanagedType.U4)] uint nCmdexecopt,
                [In] IntPtr pvaIn,
                [In, Out] IntPtr pvaOut);
        }
    }
}