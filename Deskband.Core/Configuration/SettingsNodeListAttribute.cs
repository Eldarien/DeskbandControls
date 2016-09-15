using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingsNodeListAttribute : Attribute
    {
        public string Name { get; private set; }

        public SettingsNodeListAttribute(string name)
        {
            Name = name;
        }
    }
}