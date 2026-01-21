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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ViewModels.MainViewModel viewModel)
            {
                await viewModel.StartLocationUpdates();
            }
        }
    }
}
