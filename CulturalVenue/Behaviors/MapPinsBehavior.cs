using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.UI.Maui;
using System.Collections;
using System.Collections.Specialized;
using Mapsui.Styles;

namespace CulturalVenue.Behaviors
{
    class MapPinsBehavior : Behavior<MapControl>
    {
        private Mapsui.Styles.Image? _pinImage;
        
        private static readonly Mapsui.Styles.Color MusicColor = Mapsui.Styles.Color.FromString("#FFD54F");
        private static readonly Mapsui.Styles.Color FilmColor = Mapsui.Styles.Color.FromString("#FF8A65");
        private static readonly Mapsui.Styles.Color SportColor = Mapsui.Styles.Color.FromString("#64B5F6");
        private static readonly Mapsui.Styles.Color ArtColor = Mapsui.Styles.Color.FromString("#81C784");

        private readonly Dictionary<string, Mapsui.Styles.SymbolStyle> _pinStyles = new()
        {
            { "Music", CreateRoundStyle(MusicColor) },
            { "Film", CreateRoundStyle(FilmColor) },
            { "Sports", CreateRoundStyle(SportColor) },
            { "Arts & Theater", CreateRoundStyle(ArtColor) }
        };


        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(
                nameof(ItemSource),
                typeof(IEnumerable),
                typeof(MapPinsBehavior),
                default(IEnumerable),
                propertyChanged: OnItemsSourceChanged);

        public IEnumerable ItemSource
        {
            get => (IEnumerable)GetValue(ItemSourceProperty);
            set => SetValue(ItemSourceProperty, value);
        }

        private MemoryLayer _pinsLayer;

        private MapControl _mapControl;

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (MapPinsBehavior)bindable;

            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= behavior.OnCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += behavior.OnCollectionChanged;
            }

            behavior.RefreshPins();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshPins();
        }

        private void OnMapControlPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MapControl.Map))
            {
                if (_mapControl.Map is not null)
                {
                    AddLayerToMap(_mapControl.Map);
                    RefreshPins();
                }
            }
        }

        protected override void OnAttachedTo(MapControl bindable)
        {
            base.OnAttachedTo(bindable);
            _mapControl = bindable;

            _pinsLayer = new MemoryLayer
            {
                Name = "EventsLayer"
            };

            if (_mapControl.Map is not null)
            {
                AddLayerToMap(_mapControl.Map);
            }

            _mapControl.PropertyChanged += OnMapControlPropertyChanged;

            RefreshPins();
        }

        private void AddLayerToMap(Mapsui.Map map)
        { 
            if (!map.Layers.Contains(_pinsLayer))
            {
                map.Layers.Add(_pinsLayer);
            }
        }

        protected override void OnDetachingFrom(MapControl bindable)
        {
            _mapControl.PropertyChanged -= OnMapControlPropertyChanged;
            if (_mapControl?.Map is not null && _pinsLayer is not null)
            {
                _mapControl.Map.Layers.Remove(_pinsLayer);
            }

            _mapControl = null;
            base.OnDetachingFrom(bindable);
        }

        private void RefreshPins()
        {
            if (_mapControl?.Map is null || _pinsLayer is null)
                return;

            var features = new List<IFeature>();

            if (ItemSource != null)
            {
                foreach (var item in ItemSource)
                {
                    if (item is Models.Event evnt && evnt.Venue is not null)
                    {
                        var point = SphericalMercator.FromLonLat(evnt.Venue.Longitude, evnt.Venue.Latitude);
                        var feature = new PointFeature(point);
                        if (_pinStyles.TryGetValue(evnt.Type, out var style))
                        {
                            feature.Styles.Add(style);
                        }
                        else
                        {
                            feature.Styles.Add(CreateRoundStyle(Mapsui.Styles.Color.Gray));
                        }

                        features.Add(feature);
                    }
                }
            }

            _pinsLayer.Features = features;
            _pinsLayer.DataHasChanged();
            _mapControl.RefreshData();
        }

        private static Mapsui.Styles.SymbolStyle CreateRoundStyle(Mapsui.Styles.Color color)
        {
            return new Mapsui.Styles.SymbolStyle
            {
                SymbolType = Mapsui.Styles.SymbolType.Ellipse,
                Fill = new Mapsui.Styles.Brush(color),
                SymbolScale = 0.9
            };
        }
        /*
        private void RegisterIcon()
        {
            if (_pinImage is null)
            {
                var assembly = typeof(MapPinsBehavior).Assembly;
                using var stream = assembly.GetManifestResourceStream("CulturalVenue.Resources.search.png");
                if (stream is not null)
                {
                    _pinImage = new Mapsui.Styles.Image(stream);
                }
            }
        }
        */
    }
}
