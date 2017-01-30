using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.Core.Interfaces
{
    public interface ITooltipProvider
    {
        void ShowTooltip(Guid moduleId, TooltipInfo ti);
        void RequestHideTooltip();
    }

    public class TooltipInfo
    {
        public Rectangle Rect { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color BackgroundColor { get; set; }
        public bool UseBorderlessWindow { get; set; }
        public Action<Form> CreateAction { get; set; }
        public Action DestroyAction { get; set; }
    }
}
