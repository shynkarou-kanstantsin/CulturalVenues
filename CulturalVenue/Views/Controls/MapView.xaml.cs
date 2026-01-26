using Maui.GoogleMaps;

namespace CulturalVenue.Views.Controls;

public partial class MapView : ContentView
{
	public MapView()
	{
		InitializeComponent();

        var position = new Position(53.9000, 27.5667);
        var mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(10));
        Map.MoveToRegion(mapSpan);
    }

    protected override async void OnParentSet()
    {
        base.OnParentSet();

        if (Parent != null)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                Map.MyLocationEnabled = true;
                Map.UiSettings.MyLocationButtonEnabled = true;
                Map.UiSettings.CompassEnabled = true;
            }
        }
    }
}