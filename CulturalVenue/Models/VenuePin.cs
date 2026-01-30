using Maui.GoogleMaps;
using System;
using System.Collections.Generic;
using System.Text;

namespace CulturalVenue.Models
{
    internal class VenuePin : Pin
    {
        public string VenueId { get; set; }

        private static readonly Dictionary<string, string> EventTypeToIcon = new()
        {
            { "Music", "music.png" },
            { "Film", "film.png" },
            { "Sports", "sport.png" },
            { "Arts & Theatre", "art.png" }
        };
    }
}
