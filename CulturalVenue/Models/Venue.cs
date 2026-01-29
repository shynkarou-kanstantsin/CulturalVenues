namespace CulturalVenue.Models
{
    public class Venue
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public List<string> PhotoUrl { get; set; }
    }
}