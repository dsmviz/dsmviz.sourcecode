using Dsmviz.Viewmodel.Lists;
using System.Windows;

namespace Dsmviz.View.Lists
{
    /// <summary>
    /// Interaction logic for ElementListView.xaml
    /// </summary>
    public partial class ElementListView
    {
        private ElementListViewmodel _Viewmodel;

        public ElementListView()
        {
            InitializeComponent();
        }

        private void ElementListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _Viewmodel = DataContext as ElementListViewmodel;
        }
    }
}
