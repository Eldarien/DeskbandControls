using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Deskband.Core.Interfaces
{
    public interface ITooltipProvider
    {
        void ShowTooltip(Guid moduleId, int x, int y, string text);
        void HideTooltip();
    }
}
