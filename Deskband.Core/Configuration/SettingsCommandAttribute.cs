using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class SettingsCommandAttribute : Attribute
    {
        public string Name { get; private set; }
        private readonly String _method;

        public SettingsCommandAttribute(string name, string method)
        {
            Name = name;
            _method = method;
        }

        public void ExecuteCommand(object instance, object argument)
        {
            var m = instance.GetType().GetMethod(_method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (m != null)
            {
                var p = m.GetParameters();

                m.Invoke(null, p.Length == 1 ? new[] { instance } : new[] { instance, argument });
            }
        }
    }
}