using System.Globalization;

namespace CulturalVenue.Converters
{
    public class FilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length < 2)
            {
                return false;
            }

            string chipName = (string)values[0];
            string activeChipName = (string)values[1];

            return chipName == activeChipName;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
