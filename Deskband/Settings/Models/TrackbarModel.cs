using Deskband.Common;
using Deskband.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Deskband.Settings.Models
{
    public class TrackbarModel : ObservableObject<TrackbarModel>
    {
        public TrackbarModel()
        {
        }

        public TrackbarModel(Enums.TrackbarKindType kind, Color color, int x, int y, int width, int height, bool visible = true)
        {
            Kind = kind;
            Color = color;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Visible = visible;
        }

        public Enums.TrackbarKindType Kind { get; set; }

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

        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; RaisePropertyChangedEvent(x => x.Color); }
        }

        private Color _backgroundColor;

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; RaisePropertyChangedEvent(x => x.BackgroundColor); }
        }

        private bool _useBackgroundColor;

        public bool UseBackgroundColor
        {
            get { return _useBackgroundColor; }
            set { _useBackgroundColor = value; RaisePropertyChangedEvent(x => x.UseBackgroundColor); }
        }

        private Boolean _visible;

        public Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; RaisePropertyChangedEvent(x => x.Visible); }
        }

        private bool _hideBorders;

        public bool HideBorders
        {
            get { return _hideBorders; }
            set { _hideBorders = value; RaisePropertyChangedEvent(x => x.HideBorders); }
        }
    }
}