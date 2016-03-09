using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class AlbumArtSettings
    {
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string StubImagePath { get; set; }
        public bool DoNotShowStubImage { get; set; }
        public bool PreserveAspectRatio { get; set; }
    }
}
