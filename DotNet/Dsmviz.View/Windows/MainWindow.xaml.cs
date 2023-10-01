using System.Reflection;
using System.Windows;
using Dsmviz.Viewmodel.Main;
using Dsmviz.Application.Core;
using Dsmviz.Datamodel.Dsm.Core;
using Dsmviz.Viewmodel.Editing.Element;
using Dsmviz.Viewmodel.Editing.Relation;
using Dsmviz.Viewmodel.Editing.Snapshot;
using Dsmviz.Viewmodel.Settings;
using SettingsView = Dsmviz.View.Settings.SettingsView;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Dsmviz.View.Editing;
using Dsmviz.View.Lists;
using Dsmviz.View.Settings;

namespace Dsmviz.View.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainViewmodel _mainViewmodel;
        private ProgressWindow _progressWindow;

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_mainViewmodel.IsModified)
            {
                e.Cancel = MessageBox.Show("Are you sure to exit?", "You have unsaved changes", MessageBoxButton.YesNo) != MessageBoxResult.Yes;
            }
        }

        public void OpenModel(string filename)
        {
            _mainViewmodel.OpenFileCommand.Execute(filename);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DsmModel model = new DsmModel("Viewer", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);
            _mainViewmodel = new MainViewmodel(application);
            _mainViewmodel.ElementsReportReady += OnElementsReportReady;
            _mainViewmodel.RelationsReportReady += OnRelationsReportReady;
            _mainViewmodel.ProgressViewmodel.BusyChanged += OnProgressViewmodelBusyChanged;

            _mainViewmodel.ElementEditStarted += OnElementEditStarted;

            _mainViewmodel.SnapshotMakeStarted += OnSnapshotMakeStarted;

            _mainViewmodel.ActionsVisible += OnActionsVisible;
            _mainViewmodel.SettingsVisible += OnSettingsVisible;

            _mainViewmodel.ScreenshotRequested += OnScreenshotRequested;
            DataContext = _mainViewmodel;

            OpenModelFile();
        }

        private void OnSettingsVisible(object sender, SettingsViewmodel Viewmodel)
        {
            SettingsView view = new SettingsView { DataContext = Viewmodel };
            view.ShowDialog();
        }

        private void OnActionsVisible(object sender, Viewmodel.Lists.ActionListViewmodel Viewmodel)
        {
            ActionListView view = new ActionListView {DataContext = Viewmodel};
            view.Show();
        }

        private void OnSnapshotMakeStarted(object sender, SnapshotMakeViewmodel Viewmodel)
        {
            SnapshotCreateDialog view = new SnapshotCreateDialog { DataContext = Viewmodel};
            view.ShowDialog();
        }

        private void OnElementEditStarted(object sender, ElementEditViewmodel Viewmodel)
        {
            ElementEditDialog view = new ElementEditDialog { DataContext = Viewmodel};
            view.ShowDialog();
        }

        private void OpenModelFile()
        {
            App app = System.Windows.Application.Current as App;
            if ((app != null) && (app.CommandLineArguments.Length == 1))
            {
                string filename = app.CommandLineArguments[0];
                if (filename.EndsWith(".dsm") || filename.EndsWith(".dsi"))
                {
                    _mainViewmodel.OpenFileCommand.Execute(filename);
                }
            }
        }

        private void OnElementsReportReady(object sender, Viewmodel.Lists.ElementListViewmodel e)
        {
            ElementListView view = new ElementListView
            {
                DataContext = e,
                Owner = this
            };
            view.Show();
        }

        private void OnRelationsReportReady(object sender, Viewmodel.Lists.RelationListViewmodel e)
        {
            RelationListView view = new RelationListView
            {
                DataContext = e,
                Owner = this
            };
            view.Show();
        }

        private void OnProgressViewmodelBusyChanged(object sender, bool visible)
        {
            if (visible)
            {
                if (_progressWindow == null)
                {
                    _progressWindow = new ProgressWindow
                    {
                        DataContext = _mainViewmodel.ProgressViewmodel,
                        Owner = this
                    };
                    _progressWindow.ShowDialog();
                }
            }
            else
            {
                _progressWindow.Close();
                _progressWindow = null;
            }
        }

        private void OnScreenshotRequested(object sender, System.EventArgs e)
        {
            const int leftMargin = 5;
            const int topMargin = 70;
            const int bottomMargin = 2;
            int width = (int)(Matrix.UsedWidth * _mainViewmodel.ActiveMatrix.ZoomLevel) + leftMargin;
            int height = (int)(Matrix.UsedHeight * _mainViewmodel.ActiveMatrix.ZoomLevel) + topMargin;
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(Matrix);
            Int32Rect rect = new Int32Rect(leftMargin, topMargin, width - leftMargin, height - topMargin - bottomMargin);
            CroppedBitmap croppedBitmap = new CroppedBitmap(renderTargetBitmap, rect);
            Clipboard.SetImage(croppedBitmap);
        }
    }
}
