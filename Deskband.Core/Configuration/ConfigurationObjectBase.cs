using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Core.Configuration
{
    public class ConfigurationObjectBase
    {
        [Browsable(false)]
        public Guid ModuleId { get; set; }

        //[Category("Position")]
        //public int Left { get; set; }

        //[Category("Position")]
        //public int Top { get; set; }

        [Category("Position")]
        public virtual int Order { get; set; }

        [Category("Size")]
        public virtual int Width { get; set; }

        [Category("Size")]
        public virtual int Height { get; set; }
    }
}