using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class ModuleSettings
    {
        private string _moduleName = null;

        public void SetName(string name)
        {
            _moduleName = name;
        }

        public override string ToString()
        {
            return _moduleName ?? Id.ToString();
        }

        [Browsable(false)]
        public Guid Id { get; set; }

        [Category("Position")]
        public int Left { get; set; }

        [Category("Position")]
        public int Top { get; set; }

        [Category("Size")]
        public int Width { get; set; }

        [Category("Size")]
        public int Height { get; set; }
    }
}