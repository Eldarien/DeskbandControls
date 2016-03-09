using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Deskband.Core.WinApi;

namespace Deskband.UI
{
    public class ModuleContainer : ControlsContainer, IModuleContainer
    {
        private List<ModuleContainerEntry> _entries = new List<ModuleContainerEntry>();

        private ModuleContainerEntry Entry(Guid moduleId)
        {
            var entry = _entries.FirstOrDefault(x => x.Id == moduleId);
            if (entry == null)
            {
                entry = new ModuleContainerEntry(moduleId);
                _entries.Add(entry);
                Controls.Add(entry.Container);
            }
            return entry;
        }

        public void AddControl(Guid moduleId, Control control, bool isLastChild = false)
        {
            var entry = Entry(moduleId);
            if (!entry.Container.Controls.Contains(control))
            {
                entry.Container.Controls.Add(control);
            }
            if (isLastChild)
            {
                entry.Container.Controls.SetChildIndex(control, entry.Container.Controls.Count - 1);
            }
        }

        public void ClearControls(Guid moduleId)
        {
            Entry(moduleId).Container.Controls.Clear();
        }

        public Size GetSize(Guid moduleId)
        {
            return Entry(moduleId).Container.Size;
        }

        public void Hide(Guid moduleId)
        {
            Entry(moduleId).Container.Visible = false;
            LayoutModules();
        }

        public void Show(Guid moduleId)
        {
            Entry(moduleId).Container.Visible = true;
            LayoutModules();
        }

        public Control AsControl()
        {
            return this;
        }

        public void PositionModules(Guid moduleId, Size size, Point location)
        {
            var entry = Entry(moduleId);
            entry.Container.Size = size;
            entry.Container.Location = location;
            LayoutModules();
        }

        private void LayoutModules()
        {
            //TODO: layout controls, then calculate bounding box for visible controls and call OnResize event
            var width = _entries.Where(x => x.Container.Visible).Sum(x => x.Container.Size.Width);
            var height = _entries.Where(x => x.Container.Visible).Max(x => x.Container.Size.Height);
            this.Size = new Size(width, height);
            //this.OnResize(EventArgs.Empty);
            this.Refresh();
        }

        public Guid? LocateModuleAtPoint(Point location)
        {
            var container = GetChildAtPoint(location);
            var entry = _entries.FirstOrDefault(e => e.Container == container);
            return entry != null ? entry.Id : (Guid?)null;
        }
    }
}
