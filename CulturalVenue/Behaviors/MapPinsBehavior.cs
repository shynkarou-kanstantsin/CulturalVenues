using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.UI.Maui;
using System.Collections;
using System.Collections.Specialized;

namespace CulturalVenue.Behaviors
{
    class MapPinsBehavior : Behavior<MapControl>
    {
        //private int _bitmapId = -1;

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
            // Оставляем базовые проверки
            if (_mapControl?.Map is null || _pinsLayer is null)
                return;

            var features = new List<IFeature>();

            // Добавляем реальные данные, если они есть
            if (ItemSource != null)
            {
                foreach (var item in ItemSource)
                {
                    if (item is Models.Event evnt && evnt.Venue is not null)
                    {
                        var point = SphericalMercator.FromLonLat(evnt.Venue.Longitude, evnt.Venue.Latitude);
                        var feature = new PointFeature(point);
                        feature.Styles.Add(new Mapsui.Styles.SymbolStyle { SymbolScale = 0.5, Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red) });
                        features.Add(feature);
                    }
                }
            }

            _pinsLayer.Features = features;

            _pinsLayer.DataHasChanged();
            _mapControl.RefreshData();
        }

        /*
        private void RegisterIcon()
        {
            if (_bitmapId != -1)
                return;

            var resourceName = "CulturalVenue.Resources.Image.search.png";
            var assembly = typeof(MapPinsBehavior).Assembly;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is not null)
            {
                _bitmapId = BitmapRegistry.Instance.Register(stream);
                _bitmapId = Mapsui.Styles.BitmapRegistry.Instance.Register(stream);
                _bitmapId = Mapsui.UI.BitmapRegistry.Instance.Register(stream);
            }
        }
        */
    }
}
