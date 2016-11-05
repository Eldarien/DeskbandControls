using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Settings
{
    public class ModelDataWithParentData
    {
        public object Data { get; private set; }
        public object ParentData { get; private set; }

        public ModelDataWithParentData(object data, object parentData)
        {
            Data = data;
            ParentData = parentData;
        }
    }
}
