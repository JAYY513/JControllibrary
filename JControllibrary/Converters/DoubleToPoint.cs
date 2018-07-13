using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace JControllibrary.Converters
{
    public class DoubleToPoint : IMultiValueConverter
    {
        public static readonly DoubleToPoint Instance = new DoubleToPoint();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var point = double.IsNaN((double)values[2]) ? new Point() : new Point((double)values[0] / (double)values[2], (double)values[1] / (double)values[3]);
            return point;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OppDoubleToPoint : IMultiValueConverter
    {
        public static readonly OppDoubleToPoint Instance = new OppDoubleToPoint();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var point = double.IsNaN((double)values[2]) ? new Point() : new Point(1 - (double)values[0] / (double)values[2], 1 - (double)values[1] / (double)values[3]);
            return point;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
