using CulturalVenues.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CulturalVenues.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public ObservableCollection<Event> Events { get; set; }
        

        public MainViewModel()
        {
            Events = new ObservableCollection<Event>
            {
                new Event
                {
                    Id = "1",
                    Title = "Art Exhibition",
                    Description = "An exhibition showcasing local artists.",
                    Date = new DateTime(2024, 7, 15),
                    TimeStart = new TimeSpan(18, 0, 0),
                    StartingPrice = 10.00m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://248006.selcdn.ru/main/iblock/344/3442c5677ef3477de3c980080d907a54/550be50fffb2d4448f55696361453d59.jpg" },
                    Type = "Exhibition",
                    Venue = new Venue
                    {
                        Id = "V1",
                        Name = "City Art Gallery",
                        Latitude = 40.7128,
                        Longitude = -74.0060,
                        Address = "123 Art St, New York, NY",
                        PhotoUrl = new List<string> { "https://example.com/venue1.jpg" }
                    }
                },
                new Event
                {
                    Id = "2",
                    Title = "Classical Music Concert",
                    Description = "An evening of classical music performances.",
                    Date = new DateTime(2024, 8, 20),
                    TimeStart = new TimeSpan(20, 0, 0),
                    StartingPrice = 25.00m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://img03.rl0.ru/afisha/e1200x630p478x500f2452x1401q85iw10cc/s5.afisha.ru/mediastorage/19/5a/28369821ab41424f89fdbddd5a19.jpg" },
                    Type = "Concert",
                    Venue = new Venue
                    {
                        Id = "V2",
                        Name = "Grand Concert Hall",
                        Latitude = 34.0522,
                        Longitude = -118.2437,
                        Address = "456 Music Ave, Los Angeles, CA",
                        PhotoUrl = new List<string> { "https://example.com/venue2.jpg" }
                    }
                }
            };
        }


    }
}
