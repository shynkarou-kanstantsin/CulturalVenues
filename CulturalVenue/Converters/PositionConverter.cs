using System;
using System.Collections.Generic;
using System.Text;
using Maui.GoogleMaps;
using System.Globalization;

namespace CulturalVenue.Converters
{
    public class PositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CulturalVenue.Models.Venue venue)
            {
                return new Position(venue.Latitude, venue.Longitude);
            }
            return new Position(0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
