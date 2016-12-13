using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Deskband.Core.Configuration
{
    public class ColorHexConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                ? true
                : base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Color)
                ? true
                : base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value.GetType() == typeof(Color))
            {
                var c = (Color)value;
                return "#" + c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string input = (string)value;
                return ColorTranslator.FromHtml(input);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
