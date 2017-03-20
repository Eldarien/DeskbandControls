using Deskband.Core.EventArguments;
using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Console
{
    public class ConsoleHandler : IDisposable, IConsole
    {
        private readonly ISizeProvider _sp;
        private List<Tuple<string, bool>> _lines;
        private ConsoleForm _form;

        public event EventHandler<ValueEventArgs<bool>> OnConsoleToggle;

        public ConsoleHandler(ISizeProvider sizeProvider)
        {
            _sp = sizeProvider;
            _lines = new List<Tuple<string, bool>>();

            AddLine($"{DeskbandBridge.FB2KConstants.DeskbandControlsTitle} {DeskbandBridge.FB2KConstants.DeskbandControlsVersion}");
        }

        private void InitializeForm()
        {
            _form = new ConsoleForm();
            _form.FormClosed += OnFormClosed;
            _form.OnClear += (s, e) => _lines.Clear();
            _form.OnShowDebugChanged += (s, e) => { _form.Clear(); SendLinesToForm(e.Value); };
        }

        private void SendLinesToForm(bool includeDebug)
        {
            foreach (var line in _lines)
            {
                _form.AddLine(line.Item1, line.Item2);
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            _form = null;

            if (OnConsoleToggle != null)
                OnConsoleToggle(this, new ValueEventArgs<bool>(false));
        }

        private void SetFormSizeAndPosition()
        {
            _form.Width = _sp.MakeValue(600);
            _form.Height = _sp.MakeValue(350);

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
                SendLinesToForm(false);

                if (OnConsoleToggle != null)
                    OnConsoleToggle(this, new ValueEventArgs<bool>(true));
            }
        }

        public void AddLine(string line)
        {
            _lines.Add(new Tuple<string, bool>(line, false));

            if (_form != null)
                _form.AddLine(line, false);
        }

        public void AddDebugLine(string line)
        {
            _lines.Add(new Tuple<string, bool>(line, true));

            if (_form != null)
                _form.AddLine(line, true);
        }

        public void Dispose()
        {
            if (_form != null)
                _form.Dispose();
        }
    }
}