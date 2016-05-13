using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Deskband.Core.Configuration
{
    public class ExpandableCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string))
            {
                return "(Collection)";
            }
            return base.ConvertTo(context, culture, value, destType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IList collection = value as IList;
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < collection.Count; i++)
            {
                ExpandableCollectionPropertyDescriptor pd = new ExpandableCollectionPropertyDescriptor(collection, i);
                pds.Add(pd);
            }
            return pds;
        }
    }
}
