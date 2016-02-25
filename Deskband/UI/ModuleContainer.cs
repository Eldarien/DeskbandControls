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
            entry.Container.Controls.Add(control);
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
            UpdateSize();
        }

        public void Show(Guid moduleId)
        {
            Entry(moduleId).Container.Visible = true;
            UpdateSize();
        }

        public Control AsControl()
        {
            return this;
        }

        public void SetModuleSize(Guid moduleId, Size size)
        {
            Entry(moduleId).Container.Size = size;
            UpdateSize();
        }

        private void UpdateSize()
        {
            //TODO: layout controls, then calculate bounding box for visible controls and call OnResize event
            var entry = _entries.FirstOrDefault(x => x.Container.Visible && x.Container.Size.Width > 0);
            if (entry != null)
            {
                this.Size = entry.Container.Size;
            }
            this.OnResize(EventArgs.Empty);
            this.Refresh();
        }
    }
}
