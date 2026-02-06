using System;
using System.Collections.Generic;
using System.Text;

namespace CulturalVenue.Models
{
    public record ScreenDetails(
        double CenterLatitude,
        double CenterLongitude,
        double LatitudeDelta,
        double LongitudeDelta
    );
}
