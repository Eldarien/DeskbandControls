using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Deskband.Core.Common;
using Newtonsoft.Json;

namespace Deskband.Core.Configuration
{
    [TypeConverter(typeof(EditableObjectConverter<SettingsObject>))]
    [JsonConverter(typeof(NoTypeConverterJsonConverter<SettingsObject>))]
    public class SettingsObject
    {
        [Browsable(false)]
        public virtual string Name { get; set; }

        public static object AddCollectionItemCommandHelper<T>(List<T> collection, string newItemName, Func<string, T> newItemFunc) where T: class
        {
            int i = 1;
            string newName;
            do
            {
                newName = $"{newItemName} #{i}";
                i++;
            } while (collection.Any(x => x.ToString() == newName));

            var ts = newItemFunc(newName);
            collection.Add(ts);
            return ts;
        }

        public static object RemoveCollectionItemCommandHelper<T>(List<T> collection, T item) where T: class
        {
            collection.Remove(item);
            return collection.LastOrDefault() ?? (object)collection;
        }
    }
}