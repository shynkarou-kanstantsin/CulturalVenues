using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Maui.GoogleMaps;

namespace CulturalVenue.Converters
{
    public class BitmapIconsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string type)
            {
                return type switch
                {
                    "Music" => BitmapDescriptorFactory.FromBundle("music_pin"),
                    "Sports" => BitmapDescriptorFactory.FromBundle("sport_pin"),
                    "Film" => BitmapDescriptorFactory.FromBundle("film_pin"),
                    "Arts & Theater" => BitmapDescriptorFactory.FromBundle("art_pin")
                };
            }
            return "art_pin";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
