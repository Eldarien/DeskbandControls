using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Deskband.Settings
{
    public class NodeData
    {
        public object ItemData { get; private set; }

        public object ParentData { get; private set; }
        public PropertyInfo ListPropertyInfo { get; private set; }

        public NodeDataType DataType { get; private set; }

        public NodeData(object itemData, object parentData)
        {
            DataType = NodeDataType.Item;
            ItemData = itemData;
            ParentData = parentData;
        }

        public NodeData(object itemData, PropertyInfo listPropertyInfo)
        {
            DataType = NodeDataType.List;
            ItemData = itemData;
            ListPropertyInfo = listPropertyInfo;
        }
    }

    public enum NodeDataType
    {
        List, Item
    }
}
