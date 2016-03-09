using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Deskband.Core.EventArguments;

namespace Deskband.Console
{
    public partial class ConsoleForm : Form
    {
        public ConsoleForm()
        {
            InitializeComponent();

            tbLines.GotFocus += (s, e) => User32.HideCaret(tbLines.Handle);
        }

        public void AddLine(string line, bool debug)
        {
            if (!debug || chkDebug.Checked)
            {
                AddLines(new[] { line });
            }
        }

        private void AddLines(IEnumerable<string> lines)
        {
            if (tbLines.Text.Length > 0)
            {
                tbLines.AppendText("\r\n");
            }
            tbLines.AppendText(String.Join("\r\n", lines));
        }

        public void Clear()
        {
            tbLines.Clear();
        }

        public event EventHandler OnClear;
        public event EventHandler<ValueEventArgs<bool>> OnShowDebugChanged;

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            if (OnClear != null)
                OnClear(this, EventArgs.Empty);
        }

        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (OnShowDebugChanged != null)
                OnShowDebugChanged(this, new ValueEventArgs<bool>(chkDebug.Checked));
        }
    }
}
