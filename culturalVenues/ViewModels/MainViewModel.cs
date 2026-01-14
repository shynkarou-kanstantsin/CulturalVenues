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
                },
                new Event
                {
                    Id = "3",
                    Title = "Theater Play",
                    Description = "A captivating drama performed by local actors.",
                    Date = new DateTime(2024, 9, 10),
                    TimeStart = new TimeSpan(19, 30, 0),
                    StartingPrice = 15.00m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://www.maly.ru/images/performances/5a43aa03ea87e.jpg" },
                    Type = "Theater",
                    Venue = new Venue
                    {
                        Id = "V3",
                        Name = "Downtown Theater",
                        Latitude = 41.8781,
                        Longitude = -87.6298,
                        Address = "789 Drama Rd, Chicago, IL",
                        PhotoUrl = new List<string> { "https://example.com/venue3.jpg" }
                    }
                },
                new Event
                {
                    Id = "4",
                    Title = "Jazz Night",
                    Description = "A night of smooth jazz performances.",
                    Date = new DateTime(2024, 10, 5),
                    TimeStart = new TimeSpan(21, 0, 0),
                    StartingPrice = 20.00m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://showcatalog.by/Shutterstock/Jazz-Banner.jpg" },
                    Type = "Concert",
                    Venue = new Venue
                    {
                        Id = "V4",
                        Name = "Jazz Club",
                        Latitude = 29.7604,
                        Longitude = -95.3698,
                        Address = "321 Jazz Ln, Houston, TX",
                        PhotoUrl = new List<string> { "https://example.com/venue4.jpg" }
                    }
                },
                new Event
                {
                    Id = "5",
                    Title = "Modern Dance Performance",
                    Description = "A contemporary dance show by a renowned troupe.",
                    Date = new DateTime(2024, 11, 12),
                    TimeStart = new TimeSpan(19, 0, 0),
                    StartingPrice = 30.00m,
                    Currency = "USD",
                    PhotoUrl = new List<string> { "https://godance.tv/sites/default/files/users/1/liteblog/122/084D8CD4-7492-405E-B31D-462E4ADC585A.jpeg" },
                    Type = "Dance",
                    Venue = new Venue
                    {
                        Id = "V5",
                        Name = "City Dance Theater",
                        Latitude = 33.4484,
                        Longitude = -112.0740,
                        Address = "654 Dance Blvd, Phoenix, AZ",
                        PhotoUrl = new List<string> { "https://example.com/venue5.jpg" }
                    }
                }
            };
        }
            

    }
}
