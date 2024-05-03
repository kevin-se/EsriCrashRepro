using System.Reactive.Linq;
using Esri.ArcGISRuntime.UI.Controls;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace Crasherator3000
{
    internal class LayerQueryExecutor
    {
        private readonly MapView _mapView;
        public LayerQueryExecutor(MapView mapView)
        {
            _mapView = mapView;

            Observable.Interval(TimeSpan.FromMilliseconds(1)).Do(_ => EnumerateMapLayers()).Subscribe();
        }

        private void EnumerateMapLayers()
        {
            if (Map() is { } map) {

                var lines = map.AllLayers
                    .Select(l => l.Name)
                    .Select(l => l.ToUpper());

                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
            }
        }

        private Map? Map() => GetMapFromUiThread();

        private Map? GetMapFromUiThread()
        {
            try
            {
                return _mapView.Dispatcher.Invoke(() => _mapView?.Map);
            }
            catch (TaskCanceledException)
            {
            }

            return null;
        }
    }
}
