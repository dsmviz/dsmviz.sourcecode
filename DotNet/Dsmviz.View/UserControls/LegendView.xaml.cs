using System.Windows;
using System.Windows.Controls;
using Dsmviz.Viewmodel.Main;

namespace Dsmviz.View.UserControls
{
    /// <summary>
    /// Interaction logic for LegendView.xaml
    /// </summary>
    public partial class LegendView : UserControl
    {
        private MainViewmodel _mainViewmodel;

        public LegendView()
        {
            InitializeComponent();
        }

        private void LegendView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewmodel = DataContext as MainViewmodel;
        }
    }
}
