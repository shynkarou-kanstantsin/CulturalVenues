namespace CulturalVenue.Models
{
    public class Venue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public List<string> PhotoUrl { get; set; }
    }
}