using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Communication
{
    public class ValueEventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public ValueEventArgs(T value)
        {
            Value = value;
        }
    }

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