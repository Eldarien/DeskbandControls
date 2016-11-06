using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Deskband.Settings
{
    public class ModelDataWithPropertyInfo : IModelData
    {
        public object Data { get; private set; }
        public PropertyInfo Info { get; private set; }

        public ModelDataWithPropertyInfo(object data, PropertyInfo info)
        {
            Data = data;
            Info = info;
        }
    }
}
