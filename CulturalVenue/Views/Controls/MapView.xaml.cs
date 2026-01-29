using CulturalVenue.Models;
using Maui.GoogleMaps;
using Microsoft.Maui.Dispatching;
using System.ComponentModel;
using System.Diagnostics;

namespace CulturalVenue.Views.Controls;

public partial class MapView : ContentView
{
    private IDispatcherTimer timer;

    public event EventHandler<ScreenDetails> ScreenDetailsChanged;

    public MapView()
	{
		InitializeComponent();
        
        Map.PropertyChanged += Map_PropertyChanged;
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500);

        timer.Tick += (s, e) =>
        {
            timer.Stop();
            CalculateScreenDetails();
        };

        var position = new Position(40.7128, -74.0060);
        var mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(10));
        Map.MoveToRegion(mapSpan);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        Debug.WriteLine($"MapView BC: {BindingContext?.GetType().Name}");
        Map.BindingContext = BindingContext;
    }


    public void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Map.VisibleRegion))
        {
            timer.Stop();
            timer.Start();
        }
    }

    public void CalculateScreenDetails()
    {
        if (Map.VisibleRegion is null)
            return;
        
        double centerLongitude = Map.VisibleRegion.Center.Longitude;
        double centerLatitude = Map.VisibleRegion.Center.Latitude;
        
        Location northEast = new Location(
            centerLatitude + (Map.VisibleRegion.LatitudeDegrees / 2),
            centerLongitude + (Map.VisibleRegion.LongitudeDegrees / 2)
        );

        Location southWest = new Location(
            Map.VisibleRegion.Center.Latitude - (Map.VisibleRegion.LatitudeDegrees / 2),
            Map.VisibleRegion.Center.Longitude - (Map.VisibleRegion.LongitudeDegrees / 2)
        );
        
        double radius = Location.CalculateDistance(northEast, southWest, DistanceUnits.Kilometers) / 2;
    
        var details = new Models.ScreenDetails(
            centerLatitude,
            centerLongitude,
            radius
        );
        
        
        Debug.WriteLine($"ScreenDetails fired: lat={centerLatitude}, lon={centerLongitude}, r={radius}");


        ScreenDetailsChanged?.Invoke(this, details);
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