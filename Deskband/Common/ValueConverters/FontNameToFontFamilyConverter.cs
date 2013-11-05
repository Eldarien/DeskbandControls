using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Deskband.Common.ValueConverters
{
    internal class FontNameToFontFamilyConverter : ValueConverterBase, IValueConverter
    {
        public FontNameToFontFamilyConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fontName = (string)value;
            return Fonts.SystemFontFamilies.FirstOrDefault(x => x.ToString() == fontName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fontFamily = (FontFamily)value;
            return fontFamily.ToString();
        }
    }
}