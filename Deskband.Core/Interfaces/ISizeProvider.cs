using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Deskband.Core.Interfaces
{
    public interface ISizeProvider
    {
        int DPI { get; }
        float Scale { get; }

        Point MakePoint(int x, int y);
        Size MakeSize(int width, int height);
    }
}
