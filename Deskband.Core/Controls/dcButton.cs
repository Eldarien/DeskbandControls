using Deskband.Core.Common;
using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.Controls
{
    public partial class dcButton : Button
    {
        private enum ButtonStateType { Normal = 0, Pressed = 1 };

        private ButtonStateType buttonState = ButtonStateType.Normal;

        public bool DrawOutline { get; set; }

        public bool ShowAdditionalImage { get; set; }

        public new Image Image
        {
            get { return base.Image; }
            private set { base.Image = value; }
        }

        public Image Image2 { get; private set; }

        public void SetImage(Image image)
        {
            var oldImage = Image;
            Image = image != null
                  ? ImageHelpers.HQResize(image, Width, Height, true)
                  : ImageHelpers.Empty;

            if (oldImage != null && oldImage != ImageHelpers.Empty)
            {
                oldImage.Dispose();
                oldImage = null;
            }
        }

        public void SetImage2(Image image)
        {
            var oldImage = Image2;
            Image2 = image != null
                  ? ImageHelpers.HQResize(image, Width, Height, true)
                  : ImageHelpers.Empty;

            if (oldImage != null && oldImage != ImageHelpers.Empty)
            {
                oldImage.Dispose();
                oldImage = null;
            }
        }

        public override void Refresh()
        {
            User32.InvalidateRect(this.Handle, IntPtr.Zero, false);
        }

        public dcButton()
        {
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rc = new RECT(ClientRectangle);
            var hdc = e.Graphics.GetHdc();
            UxTheme.DrawThemeParentBackground(Handle, hdc, ref rc);
            if (this.DrawOutline)
            {
                Gdi32.SelectObject(hdc, Gdi32.GetStockObject(StockObjects.HOLLOW_BRUSH));
                Gdi32.SelectObject(hdc, Gdi32.GetStockObject(StockObjects.WHITE_PEN));
                Gdi32.Rectangle(hdc, 0, 0, rc.Right - rc.Left, rc.Bottom - rc.Top);
            }
            e.Graphics.ReleaseHdc(hdc);

            var image = ShowAdditionalImage
                ? (Image2 != null ? Image2 : Image)
                : Image;

            if (image != null)
            {
                int x = this.Width / 2 - image.Width / 2;
                int y = this.Height / 2 - image.Height / 2;

                if (buttonState == ButtonStateType.Pressed)
                {
                    x++; y++;
                }
                e.Graphics.DrawImage(image, x, y, image.Width, image.Height);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                buttonState = ButtonStateType.Pressed;
                User32.InvalidateRect(this.Handle, IntPtr.Zero, false);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                buttonState = ButtonStateType.Normal;
                User32.InvalidateRect(this.Handle, IntPtr.Zero, false);
            }
        }

        protected override void Dispose(bool disposing)
        {
            var oldImage1 = Image;
            Image = ImageHelpers.Empty;
            if (oldImage1 != null && oldImage1 != ImageHelpers.Empty)
            {
                oldImage1.Dispose();
                oldImage1 = null;
            }

            var oldImage2 = Image2;
            Image2 = ImageHelpers.Empty;
            if (oldImage2 != null && oldImage2 != ImageHelpers.Empty)
            {
                oldImage2.Dispose();
                oldImage2 = null;
            }

            base.Dispose(disposing);
        }
    }
}