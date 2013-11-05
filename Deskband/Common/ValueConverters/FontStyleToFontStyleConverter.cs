using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Deskband.Common.ValueConverters
{
    internal class FontStyleToFontStyleConverter : ValueConverterBase, IValueConverter
    {
        public FontStyleToFontStyleConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fontStyle = (System.Drawing.FontStyle)value;

            var result = FontStyles.Normal;

            if ((fontStyle & System.Drawing.FontStyle.Italic) != 0)
                result = FontStyles.Italic;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}