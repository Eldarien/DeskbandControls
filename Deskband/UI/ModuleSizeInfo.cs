using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Deskband.UI
{
    public class ModuleSizeInfo
    {
        public Guid Id { get; private set; }
        public bool Disabled { get; private set; }
        public Size Size { get; private set; }
        public Point Offset { get; private set; }

        public ModuleSizeInfo(Guid id, bool disabled, Size size, Point offset)
        {
            Id = id;
            Disabled = disabled;
            Size = size;
            Offset = offset;
        }
    }
}
