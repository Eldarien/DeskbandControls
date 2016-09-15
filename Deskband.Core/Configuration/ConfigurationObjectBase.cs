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
    }
}