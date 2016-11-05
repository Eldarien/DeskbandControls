using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Core.Configuration
{
    //[TypeConverter(typeof(ExpandableCollectionConverter))]
    //[Editor(typeof(NoCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class SettingsList<T> : List<T> where T : SettingsObject
    {
    }
}
