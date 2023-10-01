using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dsmviz.Viewmodel.Matrix;

namespace Dsmviz.View.Matrix
{
    public class MatrixRowHeaderView : Canvas
    {
        private MatrixViewmodel _matrixViewmodel;
        private readonly MatrixTheme _theme;

        public MatrixRowHeaderView()
        {
            _theme = new MatrixTheme(this);

            DataContextChanged += OnDataContextChanged;
            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseLeave += OnMouseLeave;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _matrixViewmodel = DataContext as MatrixViewmodel;
            if (_matrixViewmodel != null)
            {
                _matrixViewmodel.PropertyChanged += OnPropertyChanged;
                CreateChildViews();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CreateChildViews();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            ElementTreeItemViewmodel elementViewmodel = GetElementViewmodel(e.Source);
            if (elementViewmodel != null)
            {
                _matrixViewmodel?.HoverTreeItem(elementViewmodel);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _matrixViewmodel?.HoverTreeItem(null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MatrixRowHeaderItemView headerItemView = e.Source as MatrixRowHeaderItemView;
            if (headerItemView != null)
            {
                Point pt = e.GetPosition(headerItemView);

                if ((pt.X < 20) && (pt.Y < 24))
                {
                    _matrixViewmodel.ToggleElementExpandedCommand.Execute(null);
                    InvalidateVisual();
                }
            }

            ElementTreeItemViewmodel elementViewmodel = GetElementViewmodel(e.Source);
            if (elementViewmodel != null)
            {
                _matrixViewmodel?.SelectTreeItem(elementViewmodel);
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewmodel.ElementViewmodelTree))
            {
                CreateChildViews();
            }

            if ((e.PropertyName == nameof(MatrixViewmodel.SelectedRow)) ||
                (e.PropertyName == nameof(MatrixViewmodel.SelectedRow)))
            {
                RedrawChildViews();
            }
        }

        private void RedrawChildViews()
        {
            foreach (var child in Children)
            {
                MatrixRowHeaderItemView renderedRowHeaderItemView = child as MatrixRowHeaderItemView;
                renderedRowHeaderItemView?.Redraw();
            }
        }
        
        private void CreateChildViews()
        {
            double y = 0.0;

            Children.Clear();
            if (_matrixViewmodel?.ElementViewmodelTree != null)
            {
                foreach (ElementTreeItemViewmodel elementViewmodel in _matrixViewmodel.ElementViewmodelTree)
                {
                    Rect rect = GetCalculatedSize(elementViewmodel, y);

                    MatrixRowHeaderItemView elementView = new MatrixRowHeaderItemView(_matrixViewmodel, _theme)
                    {
                        Height = rect.Height,
                        Width = rect.Width
                    };
                    SetTop(elementView, rect.Top);
                    SetLeft(elementView, rect.Left);
                    elementView.DataContext = elementViewmodel;
                    Children.Add(elementView);

                    CreateChildViews(elementViewmodel, y);

                    y += rect.Height;
                }
            }

            Height = y;
            //Width = 5000; // Should be enough to draw very deep tree
        }

        private void CreateChildViews(ElementTreeItemViewmodel elementViewmodel, double y)
        {
            foreach (ElementTreeItemViewmodel child in elementViewmodel.Children)
            {
                Rect rect = GetCalculatedSize(child, y);

                MatrixRowHeaderItemView elementView = new MatrixRowHeaderItemView(_matrixViewmodel, _theme)
                {
                    Height = rect.Height,
                    Width = rect.Width
                };
                SetTop(elementView, rect.Top);
                SetLeft(elementView, rect.Left);
                elementView.DataContext = child;
                Children.Add(elementView);

                CreateChildViews(child, y);

                y += rect.Height;
            }
        }

        private ElementTreeItemViewmodel GetElementViewmodel(object source)
        {
            MatrixRowHeaderItemView headerItemView = source as MatrixRowHeaderItemView;
            return headerItemView?.DataContext as ElementTreeItemViewmodel;
        }

        private Rect GetCalculatedSize(ElementTreeItemViewmodel Viewmodel, double y)
        {
            Rect rect = new Rect();
            if (Viewmodel != null)
            {
                int leafElementCount = Viewmodel.LeafElementCount;

                double pitch = _theme.MatrixCellSize + 2.0;
                if (Viewmodel.IsExpanded)
                {
                    double x = Viewmodel.Depth * 26.0;
                    double width = pitch;
                    if (width > ActualWidth)
                    {
                        width = ActualWidth;
                    }
                    double height = leafElementCount * pitch;
                    rect = new Rect(x, y, width, height);
                }
                else
                {
                    double x = Viewmodel.Depth * 26.0;
                    double width = ActualWidth - x + 1.0;
                    if (width < 0)
                    {
                        width = 0;
                    }
                    double height = pitch;
                    rect = new Rect(x, y, width, height);
                }
            }
            return rect;
        }
    }
}

