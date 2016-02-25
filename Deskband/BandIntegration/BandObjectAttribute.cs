using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.BandIntegration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BandObjectAttribute : Attribute
    {
        public BandObjectAttribute()
        {
        }

        public BandObjectAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}