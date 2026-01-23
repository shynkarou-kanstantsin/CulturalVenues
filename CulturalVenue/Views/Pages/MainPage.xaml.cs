using CulturalVenue.ViewModels;
using Mapsui.UI.Maui;
using Mapsui.Widgets;
using Mapsui.Widgets.InfoWidgets;
using Syncfusion.Maui.Toolkit.BottomSheet;

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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            bottomSheet.State = BottomSheetState.Collapsed;
            bottomSheet.IsOpen = true;
            bottomSheet.StateChanged += OnBottomSheetStateChanged;
        }

        protected override void OnDisappearing()
        {
            bottomSheet.StateChanged -= OnBottomSheetStateChanged;

            base.OnDisappearing();
        }

        private bool _handlingOverlayClose;

        private async void OnBottomSheetStateChanged(object sender, StateChangedEventArgs e)
        {
            if (_handlingOverlayClose)
                return;

            if (!bottomSheet.IsOpen)
            {
                _handlingOverlayClose = true;

                await Task.Delay(50);

                bottomSheet.IsOpen = true;

                await Task.Delay(10);
                bottomSheet.State = BottomSheetState.Collapsed;

                _handlingOverlayClose = false;
            }
        }


    }
}
