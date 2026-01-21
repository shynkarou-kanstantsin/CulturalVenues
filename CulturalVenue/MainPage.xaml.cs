using Mapsui.UI.Maui;
using Mapsui.Widgets;
using Mapsui.Widgets.InfoWidgets;

namespace CulturalVenue
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.MainViewModel();

            if (mapControl.Map != null)
            {
                mapControl.Map.Widgets.Clear();
            }
        }
    }
}
