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
        public Size Size { get; private set; }
        public Point Offset { get; private set; }

        public ModuleSizeInfo(Guid id, Size size, Point offset)
        {
            Id = id;
            Size = size;
            Offset = offset;
        }
    }
}
