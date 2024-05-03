using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.UI.Controls;
using LanguageExt;

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

            Observable.Interval(TimeSpan.FromMilliseconds(1.11)).Do(_ => EnumerateMapLayers()).Subscribe();
            Observable.Interval(TimeSpan.FromMilliseconds(1.21)).Do(_ => EnumerateMapLayers()).Subscribe();
            Observable.Interval(TimeSpan.FromMilliseconds(1.31)).Do(_ => EnumerateMapLayers()).Subscribe();
            Observable.Interval(TimeSpan.FromMilliseconds(1.51)).Do(_ => EnumerateMapLayers()).Subscribe();
        }

        public void EnumerateMapLayers()
        {
            var lines = Map().SelectMany(m => m.AllLayers).Select(l => l.Name).Select(l => l.ToUpper());

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        private Option<Map> Map() => Prelude.Optional(GetMapFromUiThread());

        private Map? GetMapFromUiThread()
        {
            try
            {
                Map? map = _mapView.Dispatcher.Invoke(() => _mapView?.Map);
                return map;
            }
            catch (TaskCanceledException)
            {
            }

            return null;
        }
    }
}
