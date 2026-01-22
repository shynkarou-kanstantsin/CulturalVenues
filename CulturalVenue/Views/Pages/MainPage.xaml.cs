using CulturalVenue.ViewModels;
using Mapsui.UI.Maui;
using Mapsui.Widgets;
using Mapsui.Widgets.InfoWidgets;

namespace CulturalVenue.Views.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel _mainViewModel)
        {
            InitializeComponent();
            BindingContext = _mainViewModel;

            if (mapControl.Map != null)
            {
                mapControl.Map.Widgets.Clear();
            }
        }
    }
}
