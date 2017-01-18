using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.Core.Interfaces
{
    public interface ITooltipProvider
    {
        void ShowTooltip(Guid moduleId, int x, int y, int width, int height, Color backgroundColor, Action<Form> drawAction);
        void HideTooltip();
    }
}
