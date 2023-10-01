using System.Windows;
using Dsmviz.Viewmodel.Matrix;
using System.Windows.Input;
using System.Windows.Media;

namespace Dsmviz.View.Matrix
{
    public class MatrixRowMetricsView : MatrixFrameworkElement
    {
        private MatrixViewmodel _Viewmodel;
        private readonly MatrixTheme _theme;
        private Rect _rect;
        private int? _hoveredRow;
        private readonly double _pitch;
        private readonly double _offset;
        
        public MatrixRowMetricsView()
        {
            _theme = new MatrixTheme(this);
            _rect = new Rect(new Size(_theme.MatrixMetricsViewWidth, _theme.MatrixCellSize));
            _hoveredRow = null;
            _pitch = _theme.MatrixCellSize + _theme.SpacingWidth;
            _offset = _theme.SpacingWidth / 2;

            DataContextChanged += OnDataContextChanged;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseLeave += OnMouseLeave;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _Viewmodel = DataContext as MatrixViewmodel;
            if (_Viewmodel != null)
            {
                _Viewmodel.PropertyChanged += OnPropertyChanged;
                InvalidateVisual();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            if (_hoveredRow != row)
            {
                _hoveredRow = row;
                _Viewmodel.HoverRow(row);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _Viewmodel.HoverColumn(null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            _Viewmodel.SelectRow(row);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewmodel.ColumnHeaderToolTipViewmodel))
            {
                ToolTip = _Viewmodel.ColumnHeaderToolTipViewmodel;
            }

            if ((e.PropertyName == nameof(MatrixViewmodel.MatrixSize)) ||
                (e.PropertyName == nameof(MatrixViewmodel.HoveredColumn)) ||
                (e.PropertyName == nameof(MatrixViewmodel.SelectedColumn)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_Viewmodel != null)
            {
                int matrixSize = _Viewmodel.MatrixSize;
                for (int row = 0; row < matrixSize; row++)
                {
                    _rect.X = 0;
                    _rect.Y = _offset + row * _pitch;

                    bool isHovered = _Viewmodel.HoveredRow.HasValue && (row == _Viewmodel.HoveredRow.Value);
                    bool isSelected = _Viewmodel.SelectedRow.HasValue && (row == _Viewmodel.SelectedRow.Value);
                    MatrixColor color = _Viewmodel.ColumnColors[row];
                    SolidColorBrush background = _theme.GetBackground(color, isHovered, isSelected);

                    dc.DrawRectangle(background, null, _rect);

                    string content = _Viewmodel.Metrics[row];
                    double textWidth = MeasureText(content);
                    Point texLocation = new Point(_rect.X + _rect.Width - 30.0 - textWidth, _rect.Y + 15.0);
                    DrawText(dc, content, texLocation, _theme.TextColor, _rect.Width - _theme.SpacingWidth);
                }

                Height = _theme.MatrixHeaderHeight + _theme.SpacingWidth;
                Width = _theme.MatrixCellSize * matrixSize + _theme.SpacingWidth;
            }
        }

        private int GetHoveredRow(Point location)
        {
            double row = (location.Y - _offset) / _pitch;
            return (int)row;
        }
    }

}
