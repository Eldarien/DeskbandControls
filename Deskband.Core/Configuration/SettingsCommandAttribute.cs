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
        private readonly string _execMethod;
        private readonly string _checkMethod;

        public SettingsCommandAttribute(string name, string execMethod, string checkMethod = null)
        {
            Name = name;
            _execMethod = execMethod;
            _checkMethod = checkMethod;
        }

        public object ExecuteCommand(object instance, object argument)
        {
            var m = instance.GetType().GetMethod(_execMethod, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var p = m.GetParameters();
            return m.Invoke(null, p.Length == 1 ? new[] { instance } : new[] { instance, argument });
            
        }

        public bool IsAvailable(object instance, object argument)
        {
            if (_checkMethod == null) return true;
            var m = instance.GetType().GetMethod(_checkMethod, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var p = m.GetParameters();
            return (bool)m.Invoke(null, p.Length == 1 ? new[] { instance } : new[] { instance, argument });
        }
    }
}