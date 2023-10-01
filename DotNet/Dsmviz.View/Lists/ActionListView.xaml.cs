
using Dsmviz.Viewmodel.Lists;
using System.Windows;

namespace Dsmviz.View.Lists
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class ActionListView
    {
        private ActionListViewmodel _Viewmodel;

        public ActionListView()
        {
            InitializeComponent();
        }

        private void ActionListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _Viewmodel = DataContext as ActionListViewmodel;
        }
    }
}
