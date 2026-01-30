using CulturalVenue.Models;
using System.Globalization;
using System.Text.Json;

namespace CulturalVenue.Services
{
    public class TicketmasterService
    {
        private static readonly string[] AllowedTypes = { "Arts & Theatre", "Music", "Film", "Sports" };

        private static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://app.ticketmaster.com/discovery/v2/"),
        };

        private const string ApiKey = Config.TicketmasterKey;

        public static async Task<List<Venue>> GetEventsByMapPosition(ScreenDetails screenDetails, string? activeChipFilterName)
        {
            var latitude = screenDetails.CenterLatitude.ToString("F6", CultureInfo.InvariantCulture);
            var longitude = screenDetails.CenterLongitude.ToString("F6", CultureInfo.InvariantCulture);
            var radius = Math.Max(1, (int)screenDetails.RadiusInKm);

            string startDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string endDateTime = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-ddTHH:mm:ssZ");

            string filter = "";

            if (!string.IsNullOrEmpty(activeChipFilterName))
            {
                filter = $"&segmentName={Uri.EscapeDataString(activeChipFilterName)}";
            }

            var url = $"events?apikey={ApiKey}&latlong={latitude},{longitude}&radius={radius}&unit=km&startDateTime={startDateTime}&endDateTime={endDateTime}{filter}&size=40";

            try
            {
                var response = await httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                var venuesDict = new Dictionary<string, Venue>();

                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("_embedded", out var embedded)) return new List<Venue>();
                var eventsArray = embedded.GetProperty("events");

                foreach (var eventElement in eventsArray.EnumerateArray())
                {
                    if (eventElement.TryGetProperty("_embedded", out var evEmb) &&
                        evEmb.TryGetProperty("venues", out var venuesArray))
                    {
                        if (!eventElement.TryGetProperty("classifications", out var classificationsArray)) continue;

                        var classification = classificationsArray[0];
                        if (!classification.TryGetProperty("segment", out var segment)) continue;

                        var segmentName = segment.GetProperty("name").GetString();
                        if (!AllowedTypes.Contains(segmentName)) continue;

                        var vElement = venuesArray[0];
                        string vId = vElement.GetProperty("id").GetString();

                        if (!venuesDict.ContainsKey(vId))
                        {
                            if (!vElement.TryGetProperty("location", out var vLocation)) continue;

                            if (double.TryParse(vLocation.GetProperty("latitude").GetString(), CultureInfo.InvariantCulture, out double vLatitude) &&
                                double.TryParse(vLocation.GetProperty("longitude").GetString(), CultureInfo.InvariantCulture, out double vLongitude))
                            {
                                var newVenue = new Venue
                                {
                                    Id = vId,
                                    Name = vElement.GetProperty("name").GetString(),
                                    Latitude = vLatitude,
                                    Longitude = vLongitude,
                                    Address = vElement.TryGetProperty("address", out var address) ? address.GetProperty("line1").GetString() : "",
                                    Type = segmentName
                                };
                                venuesDict.Add(vId, newVenue);
                            }
                        }
                    }
                }
                return venuesDict.Values.ToList();
            }
            catch (Exception ex)
            {
                return new List<Venue>();
            }
        }
    }
}