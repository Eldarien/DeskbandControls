using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingsNodeAttribute : Attribute
    {
        public string Name { get; private set; }

        public SettingsNodeAttribute(string name)
        {
            Name = name;
        }
    }
}