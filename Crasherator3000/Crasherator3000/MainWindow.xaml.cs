using System.Reactive.Linq;
using Esri.ArcGISRuntime.Mapping;
using System.Windows;

namespace Crasherator3000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            MyMapView.Map = new Map();

            _addLayerObservable = Observable.Interval(TimeSpan.FromMilliseconds(10)).Do(_ => LayerAction());
        }

        private void LayerAction()
        {
            if (Map() is { } map)
            {
                layerAction(map);
            }
        }

        private const string FeatureLayerUrl = "https://sampleserver6.arcgisonline.com/arcgis/rest/services/Wildfire/FeatureServer/0";

        private static Action<Map> layerAction = AddLayer;
        private readonly IObservable<long> _addLayerObservable;

        private static void AddLayer(Map map)
        {
            var layer = new FeatureLayer(new Uri(FeatureLayerUrl));

            map.OperationalLayers.Add(layer);

            if (map.AllLayers.Count > 10)
            {
                StopAddingLayers();
            }
        }

        private static void RemoveLayer(Map map)
        {
            if (map.AllLayers.FirstOrDefault() is { } layer)
            {
                map.OperationalLayers.Remove(layer);
            }
            else
            {
                NoMoreLayers();
            }
        }

        private static void NoMoreLayers()
        {
            layerAction = AddLayer;
        }

        private static void StopAddingLayers()
        {
            layerAction = RemoveLayer;
        }

        private Map? Map() => GetMapFromUiThread();

        private Map? GetMapFromUiThread()
        {
            try
            {
                return MyMapView.Dispatcher.Invoke(() => MyMapView?.Map);
            }
            catch (TaskCanceledException)
            {
            }

            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _addLayerObservable.Subscribe(); 
            _ = new LayerQueryExecutor(MyMapView);
        }
    }
}