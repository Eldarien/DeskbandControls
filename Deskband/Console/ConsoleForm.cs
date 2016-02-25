using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Console
{
    public partial class ConsoleForm : Form
    {
        public ConsoleForm()
        {
            InitializeComponent();

            tbLines.GotFocus += (s, e) => User32.HideCaret(tbLines.Handle);
        }

        public void AddLine(string line)
        {
            AddLines(new[] { line });
        }

        public void AddLines(IEnumerable<string> lines)
        {
            if (tbLines.Text.Length > 0)
            {
                tbLines.AppendText("\r\n");
            }
            tbLines.AppendText(String.Join("\r\n", lines));
        }

        public event EventHandler OnClear;

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbLines.Clear();
            if (OnClear != null)
                OnClear(this, EventArgs.Empty);
        }
    }
}
