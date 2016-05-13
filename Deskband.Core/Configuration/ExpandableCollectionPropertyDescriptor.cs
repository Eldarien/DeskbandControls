using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Core.Configuration
{
    public class ExpandableCollectionPropertyDescriptor : PropertyDescriptor
    {
        private readonly IList _collection;
        private readonly int _index = -1;

        public ExpandableCollectionPropertyDescriptor(IList coll, int idx)
            : base(GetDisplayName(coll, idx), null)
        {
            _collection = coll;
            _index = idx;
        }

        public override bool SupportsChangeEvents
        {
            get { return true; }
        }

        private static string GetDisplayName(IList list, int index)
        {

            //return "[" + index + "]  " + CSharpName(list[index].GetType());
            var so = list[index] as SettingsObject;
            return so?.Name ?? String.Format("#{0}", index + 1);
        }

        //private static string CSharpName(Type type)
        //{
        //    var sb = new StringBuilder();
        //    var name = type.Name;
        //    if (!type.IsGenericType)
        //        return name;
        //    sb.Append(name.Substring(0, name.IndexOf('`')));
        //    sb.Append("<");
        //    sb.Append(string.Join(", ", type.GetGenericArguments()
        //                                    .Select(CSharpName)));
        //    sb.Append(">");
        //    return sb.ToString();
        //}

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get
            {
                return _collection.GetType();
            }
        }

        public override object GetValue(object component)
        {
            return _collection[_index];
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override string Name
        {
            get { return _index.ToString(); }
        }

        public override Type PropertyType
        {
            get { return _collection[_index].GetType(); }
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            _collection[_index] = value;
        }
    }
}
