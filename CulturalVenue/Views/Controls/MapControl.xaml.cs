using CulturalVenue.Models;
using Maui.GoogleMaps;
using System.ComponentModel;
using System.Windows.Input;

namespace CulturalVenue.Views.Controls;

public partial class MapView : ContentView
{
    private IDispatcherTimer timer;

    public event EventHandler<ScreenDetails> ScreenDetailsChanged;

    public static readonly BindableProperty CameraChangedCommandProperty = BindableProperty.Create(
        nameof(CameraChangedCommand),
        typeof(ICommand),
        typeof(MapView));

    public ICommand CameraChangedCommand
    {
        get => (ICommand)GetValue(CameraChangedCommandProperty);
        set => SetValue(CameraChangedCommandProperty, value);
    }

    public MapView()
	{
		InitializeComponent();
        
        Map.PropertyChanged += Map_PropertyChanged;
        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(800);

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
        if (Map.VisibleRegion is null) return;
        
        double centerLongitude = Map.VisibleRegion.Center.Longitude;
        double centerLatitude = Map.VisibleRegion.Center.Latitude;
        
        Location center = new Location(centerLatitude, centerLongitude);

        Location westSide = new Location(centerLatitude, centerLongitude + Map.VisibleRegion.LongitudeDegrees / 2);

        double radius = Location.CalculateDistance(center, westSide, DistanceUnits.Kilometers) / 2;
    
        var details = new Models.ScreenDetails(
            centerLatitude,
            centerLongitude,
            radius
        );
        
        if (CameraChangedCommand != null && CameraChangedCommand.CanExecute(details))
        {
            CameraChangedCommand.Execute(details);
        }
        //ScreenDetailsChanged?.Invoke(this, details);
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