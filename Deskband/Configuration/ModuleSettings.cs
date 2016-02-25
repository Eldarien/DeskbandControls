using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class ModuleSettings
    {
        public Guid Id { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
