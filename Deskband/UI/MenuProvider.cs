using Deskband.Console;
using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class MenuProvider : IDisposable, IMenuProvider
    {
        private Dictionary<string, MenuItem> _items = new Dictionary<string, MenuItem>();
        private Dictionary<string, MenuItem> _groupItems = new Dictionary<string, MenuItem>();

        private Band _band;
        private FloatingForm _floatingForm;
        private ConsoleHandler _console;
        private ContextMenu _contextMenu;

        public MenuProvider(Band band, FloatingForm floatingForm, ConsoleHandler console)
        {
            _band = band;
            _floatingForm = floatingForm;
            _console = console;
            _contextMenu = new ContextMenu();
            _band.ContextMenu = _contextMenu;
            _floatingForm.ContextMenu = _contextMenu;
        }

        public void Dispose()
        {
            _contextMenu.Dispose();
        }

        public string AddItem(string group, string name, Action handler)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can not be null or empty", "name");

            var key = String.Format("{0}/{1}", group, name);
            if (name != "-" && _items.ContainsKey(key))
                throw new InvalidOperationException(String.Format("Menu item \"{0}\" is already registered", key));

            Menu.MenuItemCollection menuItems;
            if (!String.IsNullOrEmpty(group))
            {
                MenuItem groupItem;
                if (!_groupItems.TryGetValue(group, out groupItem))
                {
                    groupItem = _contextMenu.MenuItems.Add(group);
                    _groupItems.Add(group, groupItem);
                }
                menuItems = groupItem.MenuItems;
            }
            else
            {
                menuItems = _contextMenu.MenuItems;
            }

            var item = menuItems.Add(name, (s, ea) => { if (handler != null) { handler(); } });
            if (name != "-")
            {
                _items.Add(key, item);
            }

            return key;
        }

        public void AddSeparator(string group)
        {
            AddItem(group, "-", null);
        }

        public void ClearGroup(string group)
        {
            MenuItem item;
            if (_items.TryGetValue(group, out item))
            {
                _contextMenu.MenuItems.Remove(item);
                foreach (var i in _items.Where(x => x.Key == group || x.Key.StartsWith(group + "/")).ToList())
                {
                    _items.Remove(i.Key);
                }
            }
        }

        public void SetItemEnabledState(string key, bool isEnabled)
        {
            MenuItem item;
            if (_items.TryGetValue(key, out item))
                item.Enabled = isEnabled;
        }

        public void SetItemCheckedState(string key, bool isChecked)
        {
            MenuItem item;
            if (_items.TryGetValue(key, out item))
                item.Checked = isChecked;
        }
    }
}
