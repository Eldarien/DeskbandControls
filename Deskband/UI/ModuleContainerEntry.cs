using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class ModuleContainerEntry
    {
        public Guid Id { get; private set; }
        public ControlsContainer Container { get; private set; }

        public ModuleContainerEntry(Guid id)
        {
            Id = id;
            Container = new ControlsContainer();
            Container.Dock = DockStyle.None;
            //Container.BorderStyle = BorderStyle.FixedSingle;
        }

        public int Order { get; set; }
    }
}
