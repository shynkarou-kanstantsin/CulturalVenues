namespace CulturalVenue
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("EventPage", typeof(CulturalVenue.Views.Pages.EventPage));
        }
    }
}
