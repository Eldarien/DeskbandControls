using System;

namespace dcmFoobar2000.Code
{
    public class TrackTextEventArgs : EventArgs
    {
        public Guid Id { get; private set; }
        public string Text { get; private set; }

        public TrackTextEventArgs(Guid id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}
