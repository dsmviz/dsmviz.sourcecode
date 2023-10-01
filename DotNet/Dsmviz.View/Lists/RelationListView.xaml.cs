using Dsmviz.Viewmodel.Editing.Relation;
using Dsmviz.View.Editing;
using Dsmviz.Viewmodel.Lists;
using System.Windows;

namespace Dsmviz.View.Lists
{
    /// <summary>
    /// Interaction logic for RelationListView.xaml
    /// </summary>
    public partial class RelationListView
    {
        private RelationListViewmodel _Viewmodel;

        public RelationListView()
        {
            InitializeComponent();
        }

        private void RelationListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _Viewmodel = DataContext as RelationListViewmodel;
            _Viewmodel.RelationAddStarted += OnRelationAddStarted;
            _Viewmodel.RelationEditStarted += OnRelationEditStarted;
        }

        private void OnRelationAddStarted(object sender, RelationEditViewmodel Viewmodel)
        {
            RelationEditDialog view = new RelationEditDialog { DataContext = Viewmodel };
            view.ShowDialog();
        }

        private void OnRelationEditStarted(object sender, RelationEditViewmodel Viewmodel)
        {
            RelationEditDialog view = new RelationEditDialog { DataContext = Viewmodel };
            view.ShowDialog();
        }
    }
}
