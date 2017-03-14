using Deskband.Core.Common;
using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.UI
{
    public struct TaskbarSizeInfo
    {
        public WinApiTypes.RECT Rect;
        public LayoutMode Mode => (Rect.Width > Rect.Height) ? LayoutMode.Horizontal : LayoutMode.Vertical;
    }
}
