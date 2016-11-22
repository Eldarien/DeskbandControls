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
        public LayoutMode Mode => (Rect.right - Rect.left) > (Rect.bottom - Rect.top) ? LayoutMode.Horizontal : LayoutMode.Vertical;
    }
}
