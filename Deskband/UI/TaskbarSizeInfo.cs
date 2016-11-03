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
        public bool IsHorizontal { get { return (Rect.right - Rect.left) > (Rect.bottom - Rect.top); } }
    }
}
