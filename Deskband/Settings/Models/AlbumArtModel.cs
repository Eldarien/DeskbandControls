using Deskband.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Settings.Models
{
    public class AlbumArtModel : ObservableObject<AlbumArtModel>
    {
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

        private String _stubImagePath;

        public String StubImagePath
        {
            get { return _stubImagePath; }
            set { _stubImagePath = value; RaisePropertyChangedEvent(x => x.StubImagePath); }
        }
    }
}