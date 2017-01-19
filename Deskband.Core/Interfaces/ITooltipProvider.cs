using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.Core.Interfaces
{
    public interface ITooltipProvider
    {
        void ShowTooltip(Guid moduleId, TooltipInfo ti);
        void HideTooltip();
    }

    public class TooltipInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color BackgroundColor { get; set; }
        public bool UseBorderlessWindow { get; set; }
        public Action<Form> DrawAction { get; set; }
    }
}
