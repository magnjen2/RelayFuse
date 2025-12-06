using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RelaySettingToolView
{
    public class SelectedCellBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSelected = value is bool b && b;
            return isSelected ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}