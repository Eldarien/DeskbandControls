using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public partial class TooltipForm : Form
    {
        public TooltipForm()
        {
            InitializeComponent();
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
    }
}
