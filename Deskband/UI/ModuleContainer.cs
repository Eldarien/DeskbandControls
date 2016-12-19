using Deskband.Core.Common;
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
        //private readonly Band _band;
        private List<ModuleContainerEntry> _entries = new List<ModuleContainerEntry>();
        private LayoutMode _layoutMode;
        //public ModuleContainer(Band band)
        //{
        //    _band = band;
        //}

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
            Entry(moduleId).Hidden = true;
            LayoutModules();
        }

        public void Show(Guid moduleId)
        {
            var entry = Entry(moduleId);
            if (!entry.Disabled)
            {
                Entry(moduleId).Hidden = false;
                LayoutModules();
            }
        }

        public Control AsControl()
        {
            return this;
        }

        public void UpdateModules(IEnumerable<ModuleSizeInfo> moduleSizeInfo, bool drawBorders, LayoutMode layoutMode)
        {
            _layoutMode = layoutMode;
            BorderStyle = drawBorders ? BorderStyle.FixedSingle : BorderStyle.None;

            int index = 0;
            foreach (var m in moduleSizeInfo)
            {
                var entry = Entry(m.Id);
                entry.Disabled = m.Disabled;
                entry.Container.Size = m.Size;
                entry.Container.Offset = m.Offset.X;
                entry.Order = index;
                if (!ImageHelpers.IsNullOrEmpty(entry.Container.BackgroundImage))
                {
                    entry.Container.BackgroundImage.Dispose();
                    entry.Container.BackgroundImage = null;
                }
                if (m.BackgroundImagePath != null)
                {
                    var image = ImageHelpers.GetImageFromFile(Environment.ExpandEnvironmentVariables(m.BackgroundImagePath));
                    if (image != null)
                    {
                        entry.Container.BackgroundImage = image;
                        entry.Container.BackgroundImageLayout = m.StretchBackgroundImage ? ImageLayout.Stretch : ImageLayout.None;
                    }
                }
                index++;
            }
            LayoutModules();
        }

        private void LayoutModules()
        {
            _entries.ForEach(x => x.Container.Visible = !x.Hidden && !x.Disabled);

            //var tsi = _band.GetTaskbarSizeInfo();
            var resultSize = new Size(10, this.Size.Height);
            var visibleEntries = _entries.Where(x => x.Container.Visible);
            if (visibleEntries.Any())
            {
                // place controls in a row or column depending on taskbar position
                int coord = 0;
                foreach (var e in visibleEntries.OrderBy(x => x.Order))
                {
                    if (_layoutMode == LayoutMode.Horizontal)
                    {
                        e.Container.Left = coord;
                        e.Container.Top = e.Container.Offset;
                        coord += e.Container.Width;
                    }
                    else
                    {
                        e.Container.Left = e.Container.Offset;
                        e.Container.Top = coord;
                        coord += e.Container.Height;
                    }
                }

                // calculate bounding box
                int xMin = visibleEntries.Min(x => x.Container.Left - (_layoutMode == LayoutMode.Horizontal ? 0 : x.Container.Offset));
                int yMin = visibleEntries.Min(x => x.Container.Top - (_layoutMode == LayoutMode.Vertical ? 0 : x.Container.Offset));
                int xMax = visibleEntries.Max(x => x.Container.Right);
                int yMax = visibleEntries.Max(x => x.Container.Bottom);
                resultSize = new Size(xMax - xMin, yMax - yMin);
            }
            Size = resultSize;
            Refresh();
        }

        public Guid? LocateModuleAtPoint(Point location)
        {
            var container = GetChildAtPoint(location, GetChildAtPointSkip.Invisible);
            var entry = _entries.FirstOrDefault(e => e.Container == container);
            return entry != null ? entry.Id : (Guid?)null;
        }

        public Rectangle GetModuleScreenRectangle(Guid id)
        {
            var entry = _entries.FirstOrDefault(x => x.Id == id);
            var rect = entry.Container.ClientRectangle;
            return RectangleToScreen(rect);
        }
    }
}