using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Deskband.Common.ValueConverters
{
    internal class SelectedIndexToEnabledConverter : ValueConverterBase, IValueConverter
    {
        public SelectedIndexToEnabledConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = (int)value;
            return index > -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}