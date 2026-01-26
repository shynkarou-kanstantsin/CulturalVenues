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
}