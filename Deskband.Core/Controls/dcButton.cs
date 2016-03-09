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

        //public Enums.ButtonKindType Kind { get; set; }

        public bool ShowAdditionalImage { get; set; }

        public Image AdditionalImage { get; set; }

        public override void Refresh()
        {
            User32.InvalidateRect(this.Handle, IntPtr.Zero, false);
        }

        public dcButton()
        {
            InitializeComponent();

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
                Gdi32.Rectangle(hdc, 0, 0, rc.right - rc.left, rc.bottom - rc.top);
            }
            e.Graphics.ReleaseHdc(hdc);

            var image = ShowAdditionalImage
                ? (AdditionalImage != null ? AdditionalImage : Image)
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

        //public static AeroButton Create(ButtonModel model, bool outline)
        //{
        //    var button = new AeroButton();
        //    button.DrawOutline = outline;
        //    button.Kind = model.Kind;

        //    var image = ImageHelpers.GetImageFromFile(model.IconPath);
        //    var additionalImage = ImageHelpers.GetImageFromFile(model.AdditionalIconPath);
        //    if (image == ImageHelpers.Empty)
        //    {
        //        switch (model.Kind)
        //        {
        //            case Enums.ButtonKindType.Stop:
        //                image = Resources.Icon_Stop.ToBitmap();
        //                break;

        //            case Enums.ButtonKindType.PlayPause:
        //                image = Resources.Icon_Play.ToBitmap();
        //                additionalImage = Resources.Icon_Pause.ToBitmap();
        //                break;

        //            case Enums.ButtonKindType.Previous:
        //                image = Resources.Icon_Prev.ToBitmap();
        //                break;

        //            case Enums.ButtonKindType.Next:
        //                image = Resources.Icon_Next.ToBitmap();
        //                break;

        //            case Enums.ButtonKindType.Random:
        //                image = Resources.Icon_Random.ToBitmap();
        //                break;

        //            case Enums.ButtonKindType.StopAfterCurrent:
        //                image = Resources.Icon_StopAfterCurrentOff.ToBitmap();
        //                additionalImage = Resources.Icon_StopAfterCurrentOn.ToBitmap();
        //                break;
        //        }
        //    }

        //    var colorizeColor = model.ColorizeColor.AsDrawingColor();
        //    button.Image = ImageHelpers.Colorize(image, colorizeColor);
        //    button.AdditionalImage = ImageHelpers.Colorize(additionalImage, colorizeColor);

        //    button.Location = new Point(model.X, model.Y);
        //    button.Size = new Size(model.Width, model.Height);
        //    button.Visible = model.Visible;

        //    return button;
        //}
    }
}