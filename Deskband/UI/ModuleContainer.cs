using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class ModuleContainer : ControlsContainer, IModuleContainer
    {
        private readonly Band _band;
        private List<ModuleContainerEntry> _entries = new List<ModuleContainerEntry>();

        public ModuleContainer(Band band)
        {
            _band = band;
        }

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

        public void PositionModules(IEnumerable<ModuleSizeInfo> moduleSizeInfo)
        {
            int index = 0;
            foreach (var m in moduleSizeInfo)
            {
                var entry = Entry(m.Id);
                entry.Container.Size = m.Size;
                entry.Order = index;
                index++;
            }
            LayoutModules();
        }

        private void LayoutModules()
        {
            var tsi = _band.GetTaskbarSizeInfo();
            var resultSize = new Size(10, this.Size.Height);
            var visibleEntries = _entries.Where(x => x.Container.Visible);
            if (visibleEntries.Any())
            {
                // place controls in a row or column depending on taskbar position
                int coord = 0;
                foreach (var e in visibleEntries.OrderBy(x => x.Order))
                {
                    if (tsi.IsHorizontal)
                    {
                        e.Container.Left = coord;
                        e.Container.Top = 0;
                        coord += e.Container.Width;
                    }
                    else
                    {
                        e.Container.Left = 0;
                        e.Container.Top = coord;
                        coord += e.Container.Height;
                    }
                }

                // calculate bounding box
                int xMin = visibleEntries.Min(x => x.Container.Left);
                int yMin = visibleEntries.Min(x => x.Container.Top);
                int xMax = visibleEntries.Max(x => x.Container.Right);
                int yMax = visibleEntries.Max(x => x.Container.Bottom);
                resultSize = new Size(xMax - xMin, yMax - yMin);
            }

            this.Size = resultSize;
            this.BorderStyle = BorderStyle.FixedSingle;
            
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