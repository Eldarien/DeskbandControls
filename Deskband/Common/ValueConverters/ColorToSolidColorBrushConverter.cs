using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Deskband.Common.ValueConverters
{
    internal class ColorToSolidColorBrushConverter : ValueConverterBase, IValueConverter
    {
        public ColorToSolidColorBrushConverter()
        {
            // When you get the error "No constructor for type '...' has 0 parameters.",
            // you need to add an default constructor to your converter,
            // even it's not needed. Just for the WPF designer.
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var color = (System.Drawing.Color)value;
            //return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            var color = (Color)value;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}