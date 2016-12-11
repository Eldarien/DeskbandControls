using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class TooltipProvider : ITooltipProvider
    {
        private TooltipForm _form;

        public void ShowTooltip(Guid moduleId, int x, int y, string text)
        {
            _form = new TooltipForm();
            _form.TopMost = true;
            _form.Width = 300;
            _form.Height = 100;

            var screen = Screen.FromControl(_form);
            _form.Left = x - _form.Width / 2;
            _form.Top = screen.WorkingArea.Height - _form.Height;


            _form.Show();
        }

        public void HideTooltip()
        {
            _form.Hide();
            _form.Dispose();
            _form = null;
        }
    }
}
