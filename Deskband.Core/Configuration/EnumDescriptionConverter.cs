using Deskband.Core.Extensions;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Deskband.Core.Configuration
{
    public class EnumDescriptionConverter<T> : EnumConverter where T: struct
    {
        public EnumDescriptionConverter()
            : base (typeof(T))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Enum ve)
            {
                return ve.ToDescription();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var item = EnumExtensions.EnumToList(typeof(T)).Select(x => new { x.Key, x.Value }).FirstOrDefault(x => x.Value == value.ToString());
            if (item != null)
            {
                return item.Key;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
