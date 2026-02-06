using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CulturalVenue.Models;
using CulturalVenue.Services;
using System.Collections.ObjectModel;

namespace CulturalVenue.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<Event> Events { get; set; }
        public ObservableCollection<Venue> Venues { get; set; } 
        public ObservableCollection<SearchResult> SearchResultEvents { get; set; }
        public ObservableCollection<SearchResult> SearchResultVenues { get; set; }

        public List<ChipFilter> ChipFilterList { get; } = new()
        {
            new ChipFilter { Name = "Arts & Theatre", ImageSource = "art" },
            new ChipFilter { Name = "Music", ImageSource = "music" },
            new ChipFilter { Name = "Film", ImageSource = "film" },
            new ChipFilter { Name = "Sports", ImageSource = "sport" }
        };

        private readonly Dictionary<string, Venue> _allCachedVenues = new();

        private ScreenDetails _currentScreenDetails;
        private ScreenDetails _lastScreenDetails;

        private CancellationTokenSource _searchCancellationTokenSource;
        private CancellationTokenSource _pinsCancellationTokenSource;

        private List<SearchPoint> searchPoints = new List<SearchPoint>(5);
        private SearchPoint _lastCenterSearchPoint;

        [ObservableProperty]
        private string searchQuery;
        [ObservableProperty]
        private bool searchResultHasEvents;
        [ObservableProperty]
        private bool searchResultHasVenues;
        [ObservableProperty]
        private bool isSearchPanelVisible;
        [ObservableProperty]
        private bool searchInProgress;
        [ObservableProperty]
        private string? activeChipFilterName = null;

        public MainViewModel()
        {
            SearchResultEvents = new ObservableCollection<SearchResult>();
            SearchResultVenues = new ObservableCollection<SearchResult>();
            Venues = new ObservableCollection<Venue>();
            Events = new ObservableCollection<Event>
            {
                new Event
                {
                    Title = "Classical Music Concert",
                    Description = "An evening of classical music performances.",
                    Date = new DateTime(2024, 8, 20),
                    TimeStart = new TimeSpan(20, 0, 0),
                    StartingPrice = 25,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://img03.rl0.ru/afisha/e1200x630p478x500f2452x1401q85iw10cc/s5.afisha.ru/mediastorage/19/5a/28369821ab41424f89fdbddd5a19.jpg" },
                    Type = "Music",
                    Venue = new Venue
                    {
                        Name = "Grand Concert Hall",
                        Latitude = 34.0522,
                        Longitude = -118.2437,
                        Address = "456 Music Ave, Los Angeles, CA",
                        PhotoUrl = new List<string> { "https://example.com/venue2.jpg" }
                    }
                },
                new Event
                {
                    Title = "Theatre Play",
                    Description = "A captivating drama performed by local actors.",
                    Date = new DateTime(2024, 9, 10),
                    TimeStart = new TimeSpan(19, 30, 0),
                    StartingPrice = 15,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://www.maly.ru/images/performances/5a43aa03ea87e.jpg" },
                    Type = "Arts & Theatre",
                    Venue = new Venue
                    {
                        Name = "Downtown Theatre",
                        Latitude = 41.8781,
                        Longitude = -87.6298,
                        Address = "789 Drama Rd, Chicago, IL",
                        PhotoUrl = new List<string> { "https://example.com/venue3.jpg" }
                    }
                },
                new Event
                {
                    Title = "Jazz Night",
                    Description = "A night of smooth jazz performances.",
                    Date = new DateTime(2024, 10, 5),
                    TimeStart = new TimeSpan(21, 0, 0),
                    StartingPrice = 20m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://showcatalog.by/Shutterstock/Jazz-Banner.jpg" },
                    Type = "Music",
                    Venue = new Venue
                    {
                        Name = "Jazz Club",
                        Latitude = 29.7604,
                        Longitude = -95.3698,
                        Address = "321 Jazz Ln, Houston, TX",
                        PhotoUrl = new List<string> { "https://example.com/venue4.jpg" }
                    }
                },
                new Event
                {
                    Title = "Modern Dance Performance",
                    Description = "A contemporary dance show by a renowned troupe.",
                    Date = new DateTime(2024, 11, 12),
                    TimeStart = new TimeSpan(19, 0, 0),
                    StartingPrice = 30m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://godance.tv/sites/default/files/users/1/liteblog/122/084D8CD4-7492-405E-B31D-462E4ADC585A.jpeg" },
                    Type = "Arts & Theatre",
                    Venue = new Venue
                    {
                        Name = "City Dance Theatre",
                        Latitude = 33.4484,
                        Longitude = -112.0740,
                        Address = "654 Dance Blvd, Phoenix, AZ",
                        PhotoUrl = new List<string> { "https://example.com/venue5.jpg" }
                    }
                },
                new Event
                {
                    Title = "el Clasico",
                    Description = "Football match between FC Barcelona and Real Madrid.",
                    Date = new DateTime(2024, 12, 1),
                    TimeStart = new TimeSpan(16, 0, 0),
                    StartingPrice = 50m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://thefootballfreak.com/wp-content/uploads/2025/09/el-classico-phrase.jpg" },
                    Type = "Sports",
                    Venue = new Venue
                    {
                        Name = "National Stadium",
                        Latitude = 40.4168,
                        Longitude = -3.7038,
                        Address = "789 Sports Ave, Madrid, Spain",
                        PhotoUrl = new List<string> { "https://example.com/venue6.jpg" }
                    }
                },
                new Event
                {
                    Title = "The Lord of the rings",
                    Description = "A fantasy adventure film based on the novel by J.R.R. Tolkien.",
                    Date = new DateTime(2024, 12, 20),
                    TimeStart = new TimeSpan(19, 0, 0),
                    StartingPrice = 12m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://static.wikia.nocookie.net/lotr/images/0/0d/The_One_Ring_on_a_map_of_Middle-earth.jpg/revision/latest?cb=20200305221819" },
                    Type = "Film",
                    Venue = new Venue
                    {
                        Name = "Downtown Cinema",
                        Latitude = 51.5074,
                        Longitude = -0.1278,
                        Address = "123 Movie St, London, UK",
                        PhotoUrl = new List<string> { "https://example.com/venue7.jpg" }
                    }
                }
            };
            IsSearchPanelVisible = false;
        }

        [RelayCommand]
        public async Task SelectEvent(Event selectedEvent)
        {
            if (selectedEvent == null)
                return;

            var navigationParameters = new Dictionary<string, object>
            {
                { "Event", selectedEvent }
            };

            await Shell.Current.GoToAsync("EventPage", navigationParameters);
        }

        [RelayCommand]
        public async void FilterChipsChanged(ChipFilter choosedFilter)
        {
            if (choosedFilter.Name == ActiveChipFilterName)
            {
                ActiveChipFilterName = null;
            }
            else
            {
                ActiveChipFilterName = choosedFilter.Name;
            }

            Venues.Clear();
            await UpdateMapRegion(_currentScreenDetails, true);
        }

        partial void OnSearchQueryChanged(string value)
        {
            _ = StartDelayedSearchAsync();
        }

        private async Task StartDelayedSearchAsync()
        {
            if (_searchCancellationTokenSource is not null)
            {
                _searchCancellationTokenSource.Cancel();
                //_cancellationTokenSource.Dispose();
            }

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                IsSearchPanelVisible = false;
                SearchResultEvents.Clear();
                SearchResultVenues.Clear();
                SearchResultHasEvents = false;
                SearchResultHasVenues = false;
                return;
            }
            else if (SearchQuery.Length < 3)
            {
                SearchResultEvents.Clear();
                SearchResultVenues.Clear();
                SearchResultHasEvents = false;
                SearchResultHasVenues = false;
                return;
            }

            _searchCancellationTokenSource = new CancellationTokenSource();
            var token = _searchCancellationTokenSource.Token;

            try
            {
                SearchInProgress = true;

                await Task.Delay(500, token);

                var querySnapshot = SearchQuery;

                var (events, venues) = await PerformSearch(querySnapshot, token);

                if (token.IsCancellationRequested)
                    return;

                if (querySnapshot != SearchQuery)
                    return;

                SearchResultEvents.Clear();
                SearchResultVenues.Clear();

                foreach (var ev in events)
                {
                    SearchResultEvents.Add(ev);
                }

                foreach (var venue in venues)
                {
                    SearchResultVenues.Add(venue);
                }

                IsSearchPanelVisible = true;
                SearchInProgress = false;

                SearchResultHasEvents = SearchResultEvents.Count != 0;
                SearchResultHasVenues = SearchResultVenues.Count != 0;
            }
            catch (OperationCanceledException)
            { }
        }

        public async Task<(List<SearchResult>, List<SearchResult>)> PerformSearch(string searchQuery, CancellationToken token)
        {
            var eventResults = TicketmasterService.SearchEventsAsync(searchQuery, token);
            var venueResults = TicketmasterService.SearchVenuesAsync(searchQuery, token);

            await Task.WhenAll(eventResults, venueResults);

            return (eventResults.Result, venueResults.Result);
        }

        [RelayCommand]
        private async Task MapRegionChanged(ScreenDetails details)
        {
            _pinsCancellationTokenSource?.Cancel();
            _pinsCancellationTokenSource = new CancellationTokenSource();
            var token = _pinsCancellationTokenSource.Token;

            try
            {
                await Task.Delay(300, token);
                await UpdateMapRegion(details, false);
            }
            catch (OperationCanceledException) 
            { }
        }
        
        private async Task UpdateMapRegion(ScreenDetails details, bool forceRefresh, CancellationToken token=default)
        {
            _currentScreenDetails = details;
            
            CalculateSearchPoints(_currentScreenDetails);
            var currentCenter = searchPoints[0];

            if (MapMadeBigMove(currentCenter) || forceRefresh == true)
            {
                var emptyZones = GetEmptyZones();

                if (emptyZones.Any())
                {
                    await LoadEvents(emptyZones, token);
                }
                else
                {
                    await UpdateVisiblePins(details, token);
                }

                _lastScreenDetails = details;
                _lastCenterSearchPoint = searchPoints[0];
            }
            else
            {
                await UpdateVisiblePins(details);
            }
        }

        private List<SearchPoint> GetEmptyZones()
        {
            List<SearchPoint> emptyZones = new List<SearchPoint>();

            foreach (var point in searchPoints)
            {
                int venuesMinCount = 5;
                int venuesOnMapCount = 0;

                foreach (var venue in _allCachedVenues)
                {
                    if (!string.IsNullOrEmpty(ActiveChipFilterName) && venue.Value.Type != ActiveChipFilterName)
                    {
                        continue;
                    }

                    double distance = Location.CalculateDistance(point.Latitude, point.Longitude,
                                                                venue.Value.Latitude, venue.Value.Longitude,
                                                                DistanceUnits.Kilometers);
                    if (distance <= point.Radius)
                    {
                        venuesOnMapCount++;
                        if (venuesOnMapCount >= venuesMinCount)
                        {
                            break;
                        }
                    }
                }

                if (venuesOnMapCount < venuesMinCount)
                {
                    emptyZones.Add(point);
                }
            }
            return emptyZones;
        }

        private bool MapMadeBigMove(SearchPoint currentCenter)
        {
            if (_lastScreenDetails == null)
            {
                return true;
            }
            
            double distanceMoved = Location.CalculateDistance(_lastCenterSearchPoint.Latitude, _lastCenterSearchPoint.Longitude,
                                                             currentCenter.Latitude, currentCenter.Longitude,
                                                             DistanceUnits.Kilometers);

            double moveThreshold = _lastCenterSearchPoint.Radius * 0.1;
            double zoomThreshold = Math.Abs(_lastCenterSearchPoint.Radius- currentCenter.Radius);

            return distanceMoved > moveThreshold || zoomThreshold > currentCenter.Radius * 0.1;
        }

        private void CalculateSearchPoints(ScreenDetails screenDetails)
        {
            searchPoints.Clear();

            Location center = new Location(screenDetails.CenterLatitude, screenDetails.CenterLongitude);
            Location edge = new Location(screenDetails.CenterLatitude, screenDetails.CenterLongitude + screenDetails.LongitudeDelta / 2);
            
            double radius = Location.CalculateDistance(center, edge, DistanceUnits.Kilometers);
            double latDelta = screenDetails.LatitudeDelta / 4;
            double lonDelta = screenDetails.LongitudeDelta / 4;

            searchPoints.Add(new SearchPoint 
                (screenDetails.CenterLatitude, screenDetails.CenterLongitude, radius));

            searchPoints.Add(new SearchPoint
                (screenDetails.CenterLatitude + latDelta, screenDetails.CenterLongitude + lonDelta, radius));
            
            searchPoints.Add(new SearchPoint
                (screenDetails.CenterLatitude + latDelta, screenDetails.CenterLongitude - lonDelta, radius));

            searchPoints.Add(new SearchPoint
                (screenDetails.CenterLatitude - latDelta, screenDetails.CenterLongitude + lonDelta, radius));

            searchPoints.Add(new SearchPoint
                (screenDetails.CenterLatitude - latDelta, screenDetails.CenterLongitude - lonDelta, radius));
        }

        private async Task LoadEvents(List<SearchPoint> zones, CancellationToken token)
        {
            var results = await TicketmasterService.GetEventsByMapPosition(zones, ActiveChipFilterName, token);

            foreach(var venue in results)
            {
                token.ThrowIfCancellationRequested();
                _allCachedVenues[venue.Id] = venue;
            }

            UpdateVisiblePins(_currentScreenDetails, token); 
        }

        private async Task UpdateVisiblePins(ScreenDetails details, CancellationToken token=default)
        {
            if (details == null) details = _lastScreenDetails;

            const int MaxVisiblePins = 50;

            var venuesToShow = await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                
                double margin = 0.1;
                double minLat = details.CenterLatitude - (details.LatitudeDelta / 2) * (1 + margin);
                double maxLat = details.CenterLatitude + (details.LatitudeDelta / 2) * (1 + margin);
                double minLon = details.CenterLongitude - (details.LongitudeDelta / 2) * (1 + margin);
                double maxLon = details.CenterLongitude + (details.LongitudeDelta / 2) * (1 + margin); 
                
                return _allCachedVenues.Values.Where(v => v.Latitude >= minLat && v.Latitude <= maxLat &&
                                                            v.Longitude >= minLon && v.Longitude <= maxLon &&
                                                            (string.IsNullOrEmpty(ActiveChipFilterName) || v.Type == ActiveChipFilterName)).Take(MaxVisiblePins).ToList();
            });

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (token.IsCancellationRequested) return;
                
                var currentIds = Venues.Select(v => v.Id).ToHashSet();
                var newIds = venuesToShow.Select(v => v.Id).ToHashSet();

                if (newIds.SetEquals(currentIds))
                {
                    return;
                }

                for (int i = Venues.Count - 1; i >= 0; i--)
                {
                    if (!newIds.Contains(Venues[i].Id))
                    {
                        Venues.RemoveAt(i);
                    }
                }

                foreach (var venue in venuesToShow)
                {
                    if (!currentIds.Contains(venue.Id))
                    {
                        Venues.Add(venue);
                    }
                }
            });
        }
    }
}
