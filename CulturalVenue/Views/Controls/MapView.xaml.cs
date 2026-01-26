using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;

namespace CulturalVenue.Views.Controls;

public partial class MapView : ContentView
{
	public MapView()
	{
		InitializeComponent();

        var Location = new Location(53.9000, 27.5667);
        var mapSpan = MapSpan.FromCenterAndRadius(Location, Distance.FromKilometers(10));
        Map.MoveToRegion(mapSpan);
    }
}