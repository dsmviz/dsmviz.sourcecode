using System.Windows;
using Dsmviz.Viewmodel.Matrix;


namespace Dsmviz.View.Matrix
{
    /// <summary>
    /// Interaction logic for MatrixTopCornerView.xaml
    /// </summary>
    public partial class MatrixTopCornerView
    {
        private MatrixViewmodel _Viewmodel;

        public MatrixTopCornerView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _Viewmodel = DataContext as MatrixViewmodel;
        }

        private void OnClearSelection(object sender, RoutedEventArgs e)
        {
            _Viewmodel.SelectCell(null, null);
            _Viewmodel.SelectTreeItem(null);
        }
    }
}
