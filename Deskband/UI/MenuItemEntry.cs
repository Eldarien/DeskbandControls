using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class MenuItemEntry
    {
        public Guid Id { get; set; }
        public Guid ModuleId { get; set; }
        public MenuItem MenuItem { get; set; }
        public Menu.MenuItemCollection Collection { get; set; }
    }
}
