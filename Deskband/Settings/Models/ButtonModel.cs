using Deskband.Common;
using Deskband.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Settings.Models
{
    public class ButtonModel : ObservableObject<ButtonModel>
    {
        public ButtonModel()
        {
        }

        public ButtonModel(Enums.ButtonKindType kind, int x, int y, int width = 16, int height = 16, bool visible = true)
        {
            Kind = kind;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Visible = visible;
        }

        public Enums.ButtonKindType Kind { get; set; }

        public override string ToString()
        {
            return Kind.ToDescription();
        }

        private Int32 _x;

        public Int32 X
        {
            get { return _x; }
            set { _x = value; RaisePropertyChangedEvent(x => x.X); }
        }

        private Int32 _y;

        public Int32 Y
        {
            get { return _y; }
            set { _y = value; RaisePropertyChangedEvent(x => x.Y); }
        }

        private Int32 _width;

        public Int32 Width
        {
            get { return _width; }
            set { _width = value; RaisePropertyChangedEvent(x => x.Width); }
        }

        private Int32 _height;

        public Int32 Height
        {
            get { return _height; }
            set { _height = value; RaisePropertyChangedEvent(x => x.Height); }
        }

        private Boolean _visible;

        public Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; RaisePropertyChangedEvent(x => x.Visible); }
        }

        private String _iconPath;

        public String IconPath
        {
            get { return _iconPath; }
            set { _iconPath = value; RaisePropertyChangedEvent(x => x.IconPath); }
        }

        private String _additionalIconPath;

        public String AdditionalIconPath
        {
            get { return _additionalIconPath; }
            set { _additionalIconPath = value; RaisePropertyChangedEvent(x => x.AdditionalIconPath); }
        }
    }
}