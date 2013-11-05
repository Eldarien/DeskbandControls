using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Deskband.Common.ValueConverters
{
    internal class FontStyleToFontWeightConverter : ValueConverterBase, IValueConverter
    {
        public FontStyleToFontWeightConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fontStyle = (System.Drawing.FontStyle)value;

            var result = FontWeights.Normal;

            if ((fontStyle & System.Drawing.FontStyle.Bold) != 0)
                result = FontWeights.Bold;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}