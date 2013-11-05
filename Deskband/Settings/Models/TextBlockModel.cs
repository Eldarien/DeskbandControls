using Deskband.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Deskband.Settings.Models
{
    public class TextBlockModel : ObservableObject<TextBlockModel>
    {
        public TextBlockModel()
        {
        }

        public TextBlockModel(string name, string format, string fontName, int fontSize, Color fontColor, int x, int y, int width, int height, bool scroll = true, bool visible = true)
        {
            Name = name;
            Format = format;
            FontName = fontName;
            FontSize = fontSize;
            FontColor = fontColor;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Scroll = scroll;
            Visible = visible;
        }

        //public override String ToString()
        //{
        //    return String.IsNullOrWhiteSpace(Name) ? "Text Block" : Name;
        //}

        private String _name;

        public String Name
        {
            get { return String.IsNullOrEmpty(_name) ? "Text Block" : _name; }
            set { _name = value; RaisePropertyChangedEvent(x => x.Name); }
        }

        private String _format;

        public String Format
        {
            get { return _format; }
            set { _format = value; RaisePropertyChangedEvent(x => x.Format); }
        }

        private String _fontName;

        public String FontName
        {
            get { return _fontName; }
            set { _fontName = value; RaisePropertyChangedEvent(x => x.FontName); }
        }

        private Double _fontSize;

        public Double FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; RaisePropertyChangedEvent(x => x.FontSize); }
        }

        private bool _italic;

        public bool Italic
        {
            get { return _italic; }
            set { _italic = value; RaisePropertyChangedEvent(x => x.Italic); }
        }

        private bool _bold;

        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; RaisePropertyChangedEvent(x => x.Bold); }
        }

        private Color _fontColor;

        public Color FontColor
        {
            get { return _fontColor; }
            set { _fontColor = value; RaisePropertyChangedEvent(x => x.FontColor); }
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

        private Boolean _scroll;

        public Boolean Scroll
        {
            get { return _scroll; }
            set { _scroll = value; RaisePropertyChangedEvent(x => x.Scroll); }
        }

        private Boolean _alignToRight;

        public Boolean AlignToRight
        {
            get { return _alignToRight; }
            set { _alignToRight = value; RaisePropertyChangedEvent(x => x.AlignToRight); }
        }

        private Boolean _visible;

        public Boolean Visible
        {
            get { return _visible; }
            set { _visible = value; RaisePropertyChangedEvent(x => x.Visible); }
        }

        private String _stoppedText;

        public String StoppedText
        {
            get { return _stoppedText; }
            set { _stoppedText = value; RaisePropertyChangedEvent(x => x.StoppedText); }
        }
    }
}