using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    }
}