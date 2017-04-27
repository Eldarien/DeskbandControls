using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Code
{
    public class PlaylistEventArgs : EventArgs
    {
        public int CurrentIndex { get; private set; }
        public List<string> Playlist { get; private set; }

        public PlaylistEventArgs(int currentIndex, List<string> playlist)
        {
            CurrentIndex = currentIndex;
            Playlist = playlist;
        }
    }
}
