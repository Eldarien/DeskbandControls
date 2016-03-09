using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Code
{
    public class TrackTextEventArgs : EventArgs
    {
        public int Index { get; private set; }

        public string Text { get; private set; }

        public TrackTextEventArgs(int index, string text)
        {
            Index = index;
            Text = text;
        }
    }
}
