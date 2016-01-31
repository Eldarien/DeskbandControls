using Deskband.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Console
{
    public class ConsoleHandler : IDisposable
    {
        private List<String> _lines;
        private ConsoleForm _form;

        public event EventHandler<ValueEventArgs<bool>> OnConsoleToggle;

        public ConsoleHandler()
        {
            _lines = new List<String>();
        }

        private void InitializeForm()
        {
            _form = new ConsoleForm();
            _form.FormClosed += OnFormClosed;
            _form.OnClear += (s, e) => _lines.Clear();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            _form = null;

            if (OnConsoleToggle != null)
                OnConsoleToggle(this, new ValueEventArgs<bool>(false));
        }

        private void SetFormSizeAndPosition()
        {
            _form.Width = 600;
            _form.Height = 350;

            var screen = Screen.FromControl(_form);
            _form.Left = screen.WorkingArea.Width - _form.Width;
            _form.Top = screen.WorkingArea.Height - _form.Height;
        }

        public void ToggleConsole()
        {
            if (_form == null)
            {
                InitializeForm();
                SetFormSizeAndPosition();
            }

            if (_form.Visible)
            {
                _form.Close();
            }
            else
            {
                _form.Show();
                _form.AddLines(_lines);

                if (OnConsoleToggle != null)
                    OnConsoleToggle(this, new ValueEventArgs<bool>(true));
            }
        }

        public void AddLine(string line)
        {
            _lines.Add(line);

            if (_form != null)
                _form.AddLine(line);
        }

        public void Dispose()
        {
            if (_form != null)
                _form.Dispose();
        }
    }
}