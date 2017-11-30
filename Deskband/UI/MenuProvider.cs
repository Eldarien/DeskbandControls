using Deskband.Console;
using Deskband.Core.Extensions;
using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class MenuProvider : IDisposable, IMenuProvider
    {
        private Dictionary<Guid, MenuItemEntry> _entries = new Dictionary<Guid, MenuItemEntry>();

        private readonly Band _band;
        private readonly FloatingForm _floatingForm;
        private readonly ConsoleHandler _console;
        private readonly ContextMenu _contextMenu;
        private readonly ModuleContainer _mcontainer;
        private readonly TooltipProvider _tooltipProvider;

        public MenuProvider(Band band, FloatingForm floatingForm, ConsoleHandler console, ModuleContainer mcontainer, TooltipProvider tooltipProvider)
        {
            _band = band;
            _floatingForm = floatingForm;
            _console = console;
            _mcontainer = mcontainer;
            _tooltipProvider = tooltipProvider;

            _contextMenu = new ContextMenu();
            _band.ContextMenu = _contextMenu;
            _floatingForm.ContextMenu = _contextMenu;

            _contextMenu.Popup += _contextMenu_Popup;
            _contextMenu.Collapse += _contextMenu_Collapse;
        }



        public void Dispose()
        {
            _contextMenu.Dispose();
            foreach (var e in _entries)
            {
                e.Value.MenuItem.Dispose();
            }
            _entries.Clear();
        }

        private MenuItemEntry FindItemEntry(Guid id)
        {
            MenuItemEntry entry;
            if (_entries.TryGetValue(id, out entry))
                return entry;
            else
                return null;
        }

        private void _contextMenu_Popup(object sender, EventArgs e)
        {
            _tooltipProvider.DisableTooltip();
            var location = _mcontainer.PointToClient(Cursor.Position);
            var moduleId = _mcontainer.LocateModuleAtPoint(location);
            SetItemsVisibility(moduleId ?? Guid.Empty);
        }

        private void _contextMenu_Collapse(object sender, EventArgs e)
        {
            _tooltipProvider.EnableTooltip();
        }

        public void SetItemsVisibility(Guid moduleId)
        {
            foreach (var e in _entries.Where(x => x.Value.ModuleId != Guid.Empty))
                e.Value.MenuItem.Visible = false;

            foreach (var e in _entries.Where(x => x.Value.ModuleId == moduleId))
                e.Value.MenuItem.Visible = true;
        }

        public Guid AddItem(Guid moduleId, Guid? parentId, string text, Action handler)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text can not be null or empty", "text");

            var preparedText = text.Shorten(70).Replace("&", "&&");
            var item = new MenuItem(preparedText, (s, e) => { handler?.Invoke(); });
            var entry = new MenuItemEntry { Id = Guid.NewGuid(), ModuleId = moduleId, MenuItem = item };
            item.Tag = entry;

            var parent = parentId != null ? FindItemEntry(parentId.Value) : null;
            var menuItemsCollection = parent != null ? parent.MenuItem.MenuItems : _contextMenu.MenuItems;

            int? index = null;
            if (moduleId != Guid.Empty && parentId == null)
            {
                var firstGlobal = _entries.Where(x => x.Value.ModuleId == Guid.Empty).Select(x => x.Value).FirstOrDefault();
                if (firstGlobal != null)
                {
                    index = firstGlobal.Collection.IndexOf(firstGlobal.MenuItem);
                }
            }

            if (index != null)
                menuItemsCollection.Add(index.Value, item);
            else
                menuItemsCollection.Add(item);

            entry.Collection = menuItemsCollection;

            _entries.Add(entry.Id, entry);
            return entry.Id;
        }

        public void RemoveItem(Guid id)
        {
            var entry = FindItemEntry(id);
            if (entry != null)
            {
                entry.Collection.Remove(entry.MenuItem);
                entry.MenuItem.Dispose();
                _entries.Remove(id);
            }
        }

        public void ClearByModule(Guid moduleId)
        {
            var entries = _entries.Where(x => x.Value.ModuleId == moduleId).Select(x => x.Value).ToList();
            foreach (var entry in entries)
            {
                entry.Collection.Remove(entry.MenuItem);
                entry.MenuItem.Dispose();
                _entries.Remove(entry.Id);
            }
        }

        public void SetItemEnabledState(Guid id, bool isEnabled)
        {
            var entry = FindItemEntry(id);
            if (entry != null)
            {
                entry.MenuItem.Enabled = isEnabled;
            }
        }

        public void SetItemCheckedState(Guid id, bool isChecked)
        {
            var entry = FindItemEntry(id);
            if (entry != null)
            {
                entry.MenuItem.Checked = isChecked;
            }
        }
    }
}
