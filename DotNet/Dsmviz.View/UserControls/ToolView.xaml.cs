using System.Windows;
using System.Windows.Controls;
using Dsmviz.Viewmodel.Main;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Dsmviz.View.UserControls
{
    /// <summary>
    /// Interaction logic for ControlAndInfoView.xaml
    /// </summary>
    public partial class ToolView
    {
        private MainViewmodel _mainViewmodel;

        public ToolView()
        {
            InitializeComponent();
        }

        private void ToolView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewmodel = DataContext as MainViewmodel;
        }

        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            string filename = GetSelectedFile();
            if (_mainViewmodel.OpenFileCommand.CanExecute(filename))
            {
                _mainViewmodel.OpenFileCommand.Execute(filename);
            }
        }

        private string GetSelectedFile()
        {
            string selectedFile = null;

            OpenFileDialog dlg = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "dsm",
                Filter = "DSM model|*.dsm|DSI import|*.dsi|All Types|*.dsm;*.dsi",
                Title = "Open DSM project"
            };
            
            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                selectedFile = dlg.FileName;
            }

            return selectedFile;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }
    }
}
