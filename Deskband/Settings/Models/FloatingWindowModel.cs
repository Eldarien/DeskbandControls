using Deskband.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Deskband.Settings.Models
{
    public class FloatingWindowModel : ObservableObject<FloatingWindowModel>
    {
        private double _opacity;

        public double Opacity
        {
            get { return _opacity; }
            set { _opacity = value; RaisePropertyChangedEvent(x => x.Opacity); }
        }

        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; RaisePropertyChangedEvent(x => x.Color); }
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

        private string _backgroundImage;

        public string BackgroundImage
        {
            get { return _backgroundImage; }
            set { _backgroundImage = value; RaisePropertyChangedEvent(x => x.BackgroundImage); }
        }

        private bool _useBackgroundImage;

        public bool UseBackgroundImage
        {
            get { return _useBackgroundImage; }
            set { _useBackgroundImage = value; RaisePropertyChangedEvent(x => x.UseBackgroundImage); }
        }

        private bool _useTransparencyKey;

        public bool UseTransparencyKey
        {
            get { return _useTransparencyKey; }
            set { _useTransparencyKey = value; RaisePropertyChangedEvent(x => x.UseTransparencyKey); }
        }

        private bool _stretchBackgroundImage;

        public bool StretchBackgroundImage
        {
            get { return _stretchBackgroundImage; }
            set { _stretchBackgroundImage = value; RaisePropertyChangedEvent(x => x.StretchBackgroundImage); }
        }
    }
}