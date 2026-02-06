using CulturalVenue.ViewModels;
using Syncfusion.Maui.Toolkit.BottomSheet;

namespace CulturalVenue.Views.Pages
{
    public partial class MainPage : ContentPage
    {
        private bool _handlingOverlayClose;

        public MainPage(MainViewModel _mainViewModel)
        {
            InitializeComponent();
            BindingContext = _mainViewModel;
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
