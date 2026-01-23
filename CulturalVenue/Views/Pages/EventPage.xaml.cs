using CulturalVenue.ViewModels;

namespace CulturalVenue.Views.Pages;

public partial class EventPage : ContentPage
{
	public EventPage(EventPageViewModel _eventPageViewModel)
	{
		InitializeComponent();
		BindingContext = _eventPageViewModel;
	}
}