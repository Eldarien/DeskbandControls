using System;
using System.Windows.Forms;

namespace Deskband.Core.Interfaces
{
    public interface ITooltipProvider
    {
        void ShowTooltip(Guid moduleId, int x, int y, Action<Form> drawAction);
        void HideTooltip();
    }
}
