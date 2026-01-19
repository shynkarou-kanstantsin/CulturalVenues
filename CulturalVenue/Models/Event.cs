namespace CulturalVenue.Models
{
    internal class Event
    {
        private string? currency;
        public string Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public decimal? StartingPrice { get; set; }
        public string? Currency 
        { 
            get => currency;
            set
            {
                if (value == "USD")
                {
                    currency = "$";
                }
                else
                {
                    currency = value;
                }
            }
        }
        public List<string> PhotoUrl { get; set; }
        public string Type { get; set; }
        public Venue Venue { get; set; }
    }
}