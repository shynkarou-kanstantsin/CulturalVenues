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
        public ObservableCollection<ChipFilter> ChipFilterList { get; } = new()
        {
            new ChipFilter { Name = "Arts & Theatre", ImageSource = "art", IsSelected = false },
            new ChipFilter { Name = "Music", ImageSource = "music", IsSelected = false },
            new ChipFilter { Name = "Film", ImageSource = "film", IsSelected = false },
            new ChipFilter { Name = "Sports", ImageSource = "sport", IsSelected = false }
        };

        private ScreenDetails _currentScreenDetails;

        private CancellationTokenSource _cancellationTokenSource;

        public string? activeChipFilterName { get; set; } = null;
        
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
            if (!String.IsNullOrEmpty(activeChipFilterName))
            {
                if (activeChipFilterName == choosedFilter.Name)
                {
                    activeChipFilterName = null;
                    choosedFilter.IsSelected = false;
                    await LoadEvents(_currentScreenDetails);
                    return;
                }
                else
                {
                    foreach (var chip in ChipFilterList)
                    {
                        if (chip.Name == activeChipFilterName)
                        {
                            chip.IsSelected = false;
                            break;
                        }
                    }
                }
            }

            choosedFilter.IsSelected = true;
            activeChipFilterName = choosedFilter.Name;
            await LoadEvents(_currentScreenDetails);
        }

        partial void OnSearchQueryChanged(string value)
        {
            _ = StartDelayedSearchAsync();
        }

        private async Task StartDelayedSearchAsync()
        {
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();
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

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

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

        public async void OnScreenDetailsChanged(object sender, ScreenDetails details)
        {
           _currentScreenDetails = details;
            await LoadEvents(details);
        }

        private async Task LoadEvents(ScreenDetails details)
        {
            var results = await TicketmasterService.GetEventsByMapPosition(details, activeChipFilterName);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Venues.Clear();
                foreach (var ev in results)
                {
                    Venues.Add(ev);
                }
            });
        }
    }
}
