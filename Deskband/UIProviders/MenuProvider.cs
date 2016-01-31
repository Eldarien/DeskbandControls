using Deskband.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UIProviders
{
    public class MenuProvider : IDisposable
    {
        private Band _band;
        private ConsoleHandler _console;
        private Dictionary<string, MenuItem> _items;
        private ContextMenu _menu;

        public MenuProvider(Band band, ConsoleHandler console)
        {
            _band = band;
            _console = console;
            _items = new Dictionary<string, MenuItem>();
            _menu = new ContextMenu();
            _band.ContextMenu = _menu;
            // TODO: _floatingForm.ContextMenu = contextMenu;
        }

        public void Dispose()
        {
            _menu.Dispose();
        }

        public string AddItem(string group, string name, Action handler)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can not be null or empty", "name");

            var key = String.Format("{0}/{1}", group, name);
            if (_items.ContainsKey(key))
                throw new InvalidOperationException(String.Format("Menu item \"{0}\" is already registered", key));

            // TODO: Implement menu groups
            var item = _menu.MenuItems.Add(name, (s, ea) => { if (handler != null) { handler(); } });
            _items.Add(key, item);

            return key;
        }

        public void AddSeparator(string group)
        {
            // TODO: Implement menu groups
            _menu.MenuItems.Add("-");
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
