using CulturalVenue.Models;
using System.Globalization;
using System.Text.Json;

namespace CulturalVenue.Services
{
    public class TicketmasterService
    {
        private static readonly string[] AllowedTypes = { "Arts & Theatre", "Music", "Film", "Sports" };

        private static readonly Dictionary<string, ImageSource> Types = new()
        {
            { "Arts & Theatre", "art" },
            { "Music", "music" },
            { "Film", "film" },
            { "Sports", "sport" },
        };

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

        public static async Task<List<SearchResult>> SearchEventsAsync(string query, CancellationToken token)
        {
            var result = new List<SearchResult>();
            
            var url = $"events?apikey={ApiKey}&keyword={Uri.EscapeDataString(query)}&size=20";
            
            try
            {
                var response = await httpClient.GetAsync(url, token);
                var json = await response.Content.ReadAsStringAsync(token);

                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("_embedded", out var embedded)) return new List<SearchResult>();
                var eventsArray = embedded.GetProperty("events");

                foreach (var eventElement in eventsArray.EnumerateArray())
                {
                    string name = eventElement.GetProperty("name").GetString();

                    if (!name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    token.ThrowIfCancellationRequested();

                    if (eventElement.TryGetProperty("classifications", out var classification) && classification.GetArrayLength() > 0)
                    {
                        var firstClass = classification[0];
                        var segmentName = string.Empty;
                        var subGenreName = string.Empty;
                        var venueName = string.Empty;
                        var venueCity = string.Empty;

                        if (firstClass.TryGetProperty("segment", out var segment))
                        {
                            segmentName = segment.GetProperty("name").GetString();
                            if (!Types.ContainsKey(segmentName)) continue;
                        }

                        if (firstClass.TryGetProperty("subGenre", out var subGengre))
                        {
                            subGenreName = subGengre.GetProperty("name").GetString();
                        }

                        if (eventElement.TryGetProperty("_embedded", out var eventEmbedded) && eventEmbedded.TryGetProperty("venues", out var venues) && venues.GetArrayLength() > 0)
                        {
                            venueName = venues[0].GetProperty("name").GetString();

                            if (venues[0].TryGetProperty("city", out var city))
                            {
                                venueCity = city.GetProperty("name").GetString();
                            }
                        }

                        var newEvent = new SearchResult
                        {
                            Name = name,
                            Id = eventElement.GetProperty("id").ToString(),
                            Type = segmentName,
                            Icon = Types[segmentName],
                            Description = venueName + subGenreName == string.Empty ? "" : $"{venueName}, {venueCity} • {subGenreName}"
                        };

                        if (result.Any(ev => ev.Name == newEvent.Name && ev.Description == newEvent.Description))
                        {
                            continue;
                        }
                        result.Add(newEvent);

                        if (result.Count >= 3)
                        {
                            break;
                        }
                    } 
                }
                return result;
            }
            catch (Exception ex)
            {
                return new List<SearchResult>();
            }
        }

        public static async Task<List<SearchResult>> SearchVenuesAsync(string query, CancellationToken token)
        {
            var result = new List<SearchResult>();

            var url = $"venues?apikey={ApiKey}&keyword={Uri.EscapeDataString(query)}&size=20";

            try
            {
                var response = await httpClient.GetAsync(url, token);
                var json = await response.Content.ReadAsStringAsync(token);

                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("_embedded", out var embedded)) return new List<SearchResult>();
                var venuesArray = embedded.GetProperty("venues");

                foreach (var venueElement in venuesArray.EnumerateArray())
                {
                    string name = venueElement.GetProperty("name").ToString();

                    token.ThrowIfCancellationRequested();

                    var address = string.Empty;

                    if (venueElement.TryGetProperty("address", out var addressName))
                    {
                        address = addressName.GetProperty("line1").GetString();
                    }

                    if (venueElement.TryGetProperty("city", out var city))
                    {
                        if (address != string.Empty)
                        {
                            address += ", ";
                        }
                        address += city.GetProperty("name").GetString();
                    }

                    var newVenue = new SearchResult
                    {
                        Name = name,
                        Id = venueElement.GetProperty("id").ToString(),
                        Type = "Venue",
                        Icon = ImageSource.FromFile("venue"),
                        Description = address
                    };
                    
                    if (result.Any(ven => ven.Name == newVenue.Name && ven.Description == newVenue.Description))
                    {
                        continue;
                    }
                    result.Add(newVenue);

                    if (result.Count >= 3)
                    {
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new List<SearchResult>();
            }
        }
    }
}