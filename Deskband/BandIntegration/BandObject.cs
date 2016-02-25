using Deskband.Core.WinApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.BandIntegration
{
    public class BandObject : UserControl,
        ComTypes.IOleWindow, ComTypes.IDockingWindow,
        ComTypes.IDeskBand, ComTypes.IDeskBand2,
        ComTypes.IPersist, ComTypes.IPersistStream,
        ComTypes.IObjectWithSite, ComTypes.IInputObject
    {
        private ComTypes.IInputObjectSite InputObjectSite { get; set; }

        private uint BandId { get; set; }

        protected bool IsObjectDirty { get; set; }

        protected bool IsCompositionEnabled { get; set; }

        public BandObject()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Name = "BandObject";
            this.MinSize = new Size(-1, -1);
            this.MaxSize = new Size(-1, -1);
            this.IntegralSize = new Size(1, 1);
        }

        protected virtual string GetClassGuidString()
        {
            return Guid.Empty.ToString();
        }

        protected virtual void OnClose()
        {
        }

        [Browsable(true)]
        [DefaultValue(false)]
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Title of band object. Displayed at the left or on top of the band object.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("")]
        public String Title { get; set; }

        /// <summary>
        /// Minimum size of the band object. Default value of -1 sets no minimum constraint.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "-1,-1")]
        public Size MinSize { get; set; }

        /// <summary>
        /// Maximum size of the band object. Default value of -1 sets no maximum constraint.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "-1,-1")]
        public Size MaxSize { get; set; }

        /// <summary>
        /// Says that band object's size must be multiple of this size. Defauilt value of -1 does not set this constraint.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(typeof(Size), "1,1")]
        public Size IntegralSize { get; set; }

        /////////////////////////////////////////////////
        // Interfaces

        // IOleWindow

        public void GetWindow(out IntPtr phwnd)
        {
            phwnd = this.Handle;
        }

        public void ContextSensitiveHelp(bool fEnterMode)
        {
        }

        // IDockingWindow

        public void ShowDW(bool fShow)
        {
            if (fShow)
                Show();
            else
                Hide();
        }

        public void CloseDW(uint dwReserved)
        {
            OnClose();

            Dispose(true);
        }

        public void ResizeBorderDW(IntPtr prcBorder, object punkToolbarSite, bool fReserved)
        {
        }

        // IDeskBand

        public virtual void GetBandInfo(UInt32 dwBandID, UInt32 dwViewMode, ref ComTypes.DESKBANDINFO dbi)
        {
            this.BandId = dwBandID;

            if ((dbi.dwMask & (uint)ComTypes.DBIM.MINSIZE) > 0)
            {
                dbi.ptMinSize.X = this.MinSize.Width;
                dbi.ptMinSize.Y = this.MinSize.Height;
            }
            if ((dbi.dwMask & (uint)ComTypes.DBIM.MAXSIZE) > 0)
            {
                dbi.ptMaxSize.X = this.MaxSize.Width;
                dbi.ptMaxSize.Y = this.MaxSize.Height;
            }
            if ((dbi.dwMask & (uint)ComTypes.DBIM.INTEGRAL) > 0)
            {
                dbi.ptIntegral.X = this.IntegralSize.Width;
                dbi.ptIntegral.Y = this.IntegralSize.Height;
            }
            if ((dbi.dwMask & (uint)ComTypes.DBIM.ACTUAL) > 0)
            {
                dbi.ptActual.X = this.Size.Width;
                dbi.ptActual.Y = this.Size.Height;
            }
            if ((dbi.dwMask & (uint)ComTypes.DBIM.TITLE) > 0)
            {
                dbi.wszTitle = this.Title;
                if (!this.ShowTitle)
                {
                    dbi.dwMask &= ~(uint)ComTypes.DBIM.TITLE;
                }
            }
            if ((dbi.dwMask & (uint)ComTypes.DBIM.MODEFLAGS) > 0)
            {
                dbi.dwModeFlags = (ComTypes.DBIM)(ComTypes.DBIMF.NORMAL | ComTypes.DBIMF.FIXED | ComTypes.DBIMF.NOGRIPPER | ComTypes.DBIMF.NOMARGINS);
            }
            if ((dbi.dwMask & (uint)ComTypes.DBIM.BKCOLOR) > 0)
            {
                // Use the default background color by removing this flag.
                dbi.dwMask &= ~(uint)ComTypes.DBIM.BKCOLOR;
            }
        }

        // IDeskBand2

        public void CanRenderComposited(ref bool pfCanRenderComposited)
        {
            pfCanRenderComposited = true;
        }

        public void SetCompositionState(bool fCompositionEnabled)
        {
            this.IsCompositionEnabled = fCompositionEnabled;

            //this.Invalidate(true);
            //this.Update();
        }

        public void GetCompositionState(ref bool pfCompositionEnabled)
        {
            pfCompositionEnabled = this.IsCompositionEnabled;
        }

        // IPersist

        public void GetClassID(out Guid pClassID)
        {
            pClassID = Guid.Parse(GetClassGuidString());
        }

        // IPersistStream

        public UInt32 IsDirty()
        {
            return this.IsObjectDirty ? ComTypes.S_OK : ComTypes.S_FALSE;
        }

        public new void Load(object pStm)
        {
            Marshal.ReleaseComObject(pStm);
        }

        public void Save(object pStm, bool fClearDirty)
        {
            if (fClearDirty)
            {
                this.IsObjectDirty = false;
            }

            Marshal.ReleaseComObject(pStm);
        }

        public void GetSizeMax(ref ulong pcbSize)
        {
        }

        // IObjectWithSite

        public virtual void SetSite(Object pUnkSite)
        {
            if (this.InputObjectSite != null)
            {
                Marshal.ReleaseComObject(this.InputObjectSite);
            }

            this.InputObjectSite = (ComTypes.IInputObjectSite)pUnkSite;
        }

        public virtual void GetSite(ref Guid riid, out Object ppvSite)
        {
            ppvSite = this.InputObjectSite;
        }

        // IInputObject

        public virtual void UIActivateIO(Int32 fActivate, ref ComTypes.MSG Msg)
        {
            if (fActivate != 0)
            {
                Control ctrl = GetNextControl(this, true); //first
                if (ModifierKeys == Keys.Shift)
                {
                    ctrl = GetNextControl(ctrl, false); //last
                }
                if (ctrl != null)
                {
                    ctrl.Select();
                }
                this.Focus();
            }
        }

        public virtual UInt32 HasFocusIO()
        {
            return this.ContainsFocus ? ComTypes.S_OK : ComTypes.S_FALSE;
        }

        public virtual UInt32 TranslateAcceleratorIO(ref ComTypes.MSG msg)
        {
            if (msg.message == 0x100) //WM_KEYDOWN
            {
                if (msg.wParam == (uint)Keys.Tab || msg.wParam == (uint)Keys.F6) //keys used by explorer to navigate from control to control
                {
                    bool direction = ModifierKeys == Keys.Shift ? false : true;
                    if (SelectNextControl(ActiveControl, direction, true, true, false))
                    {
                        return ComTypes.S_OK;
                    }
                }
            }
            return ComTypes.S_FALSE;
        }

        /////////////////////////////////////////////

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);

            //this.InputObjectSite.OnFocusChangeIS(this as ComTypes.IInputObject, 1);
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);

            //if (ActiveControl == null)
            //{
            //    this.InputObjectSite.OnFocusChangeIS(this as ComTypes.IInputObject, 0);
            //}
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackColor == Color.Transparent)
            {
                IntPtr hdc = e.Graphics.GetHdc();
                RECT rc = new RECT(e.ClipRectangle);
                UxTheme.DrawThemeParentBackground(this.Handle, hdc, ref rc);
                e.Graphics.ReleaseHdc(hdc);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        public void ExecBandInfoChangedCommand()
        {
            var container = (ComTypes.IOleCommandTarget)this.InputObjectSite;
            if (container != null)
            {
                container.Exec(ref ComTypes.CGID_DeskBand,
                    (uint)ComTypes.DESKBANDCID.DBID_BANDINFOCHANGED,
                    (uint)ComTypes.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT,
                    IntPtr.Zero, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Called when derived class is registered as a COM server.
        /// </summary>
        [ComRegisterFunction]
        public static void Register(Type t)
        {
            string guid = t.GUID.ToString("B");

            using (var rkClass = Registry.ClassesRoot.CreateSubKey(@"CLSID\" + guid))
            using (var rkCat = rkClass.CreateSubKey("Implemented Categories"))
            {
                var boa = (BandObjectAttribute[])t.GetCustomAttributes(typeof(BandObjectAttribute), false);

                string name = t.Name;

                if (boa.Length == 1)
                {
                    if (boa[0].Name != null)
                        name = boa[0].Name;
                }

                rkClass.SetValue(null, name);

                var k = rkCat.CreateSubKey("{00021492-0000-0000-C000-000000000046}"); // Deskband
                k.Close();
            }
        }

        /// <summary>
        /// Called when derived class is unregistered as a COM server.
        /// </summary>
        [ComUnregisterFunction]
        public static void Unregister(Type t)
        {
            string guid = t.GUID.ToString("B");

            using (var rkCLSID = Registry.ClassesRoot.CreateSubKey(@"CLSID"))
            {
                rkCLSID.DeleteSubKeyTree(guid, false);
            }
        }
    }
}