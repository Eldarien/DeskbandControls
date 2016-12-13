using Deskband.Core.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class TooltipProvider : ITooltipProvider
    {
        private TooltipForm _form;

        public void ShowTooltip(Guid moduleId, int x, int y, Action<Form> drawAction)
        {
            _form = new TooltipForm();
            _form.TopMost = true;
            _form.MinimizeBox = false;
            _form.MaximizeBox = false;
            _form.ControlBox = false;
            _form.ShowInTaskbar = false;
            _form.Text = null;
            _form.BackColor = Color.FromKnownColor(KnownColor.Info);
            _form.Width = 400;
            _form.Height = 100;

            var screen = Screen.FromControl(_form);
            _form.Left = x - _form.Width / 2;
            _form.Top = screen.WorkingArea.Height - _form.Height;

            drawAction(_form);

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
