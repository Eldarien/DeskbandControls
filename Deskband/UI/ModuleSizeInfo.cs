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

        public ModuleSizeInfo(Guid id, Size size)
        {
            Id = id;
            Size = size;
        }
    }
}
