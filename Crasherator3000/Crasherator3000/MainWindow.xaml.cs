using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using Esri.ArcGISRuntime.Mapping;
using System.Reactive.Subjects;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Graphics.Printing;
using LanguageExt;
using Map = Esri.ArcGISRuntime.Mapping.Map;
using System.Reactive.Concurrency;
using Esri.ArcGISRuntime.UI.Controls;

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

            _enumerator = new LayerQueryExecutor(MyMapView);

            MyMapView.Map = new Map();

            _addLayerObservable = Observable.Interval(TimeSpan.FromMilliseconds(10)).Do(_ => LayerAction());
        }

        private void LayerAction()
        {
            var map = Map();
            map.Do(m => layerAction(m));
        }

        private const string FeatureLayerUrl = "https://sampleserver6.arcgisonline.com/arcgis/rest/services/Wildfire/FeatureServer/0";

        private static Action<Map> layerAction = AddLayer;
        private readonly LayerQueryExecutor _enumerator;
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
            
            var layer = map.AllLayers.HeadOrNone();

            layer.Do(l => map.OperationalLayers.Remove(l)).IfNone(NoMoreLayers);
        }

        private static void NoMoreLayers()
        {
            layerAction = AddLayer;
        }

        private static void StopAddingLayers()
        {
            layerAction = RemoveLayer;
        }

        private Option<Map> Map() => Prelude.Optional(GetMapFromUiThread());

        private Map? GetMapFromUiThread()
        {
            try
            {
                Map? map = MyMapView.Dispatcher.Invoke(() => MyMapView?.Map);
                return map;
            }
            catch (TaskCanceledException)
            {
            }

            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _addLayerObservable.Subscribe();
        }
    }
}