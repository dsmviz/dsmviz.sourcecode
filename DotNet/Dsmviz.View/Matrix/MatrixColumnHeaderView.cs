using System.Windows;
using Dsmviz.Viewmodel.Matrix;
using System.Windows.Input;
using System.Windows.Media;

namespace Dsmviz.View.Matrix
{
    public class MatrixColumnHeaderView : MatrixFrameworkElement
    {
        private MatrixViewmodel _Viewmodel;
        private readonly MatrixTheme _theme;
        private Rect _rect;
        private int? _hoveredColumn;
        private readonly double _pitch;
        private readonly double _offset;

        public MatrixColumnHeaderView()
        {
            _theme = new MatrixTheme(this);
            _rect = new Rect(new Size(_theme.MatrixCellSize, _theme.MatrixHeaderHeight));
            _hoveredColumn = null;
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
            int column = GetHoveredColumn(e.GetPosition(this));
            if (_hoveredColumn != column)
            {
                _hoveredColumn = column;
                _Viewmodel.HoverColumn(column);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _Viewmodel.HoverColumn(null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int column = GetHoveredColumn(e.GetPosition(this));
            _Viewmodel.SelectColumn(column);
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
                for (int column = 0; column < matrixSize; column++)
                {
                    _rect.X = _offset + column * _pitch;
                    _rect.Y = 0;

                    bool isHovered = _Viewmodel.HoveredColumn.HasValue && (column == _Viewmodel.HoveredColumn.Value);
                    bool isSelected = _Viewmodel.SelectedColumn.HasValue && (column == _Viewmodel.SelectedColumn.Value);
                    MatrixColor color = _Viewmodel.ColumnColors[column];
                    SolidColorBrush background = _theme.GetBackground(color, isHovered, isSelected);

                    dc.DrawRectangle(background, null, _rect);

                    string content = _Viewmodel.ColumnElementIds[column].ToString();

                    double textWidth = MeasureText(content);

                    Point location = new Point(_rect.X + 10.0, _rect.Y - _rect.Height + textWidth + 10.0);
                    DrawRotatedText(dc, content, location, _theme.TextColor, _theme.MatrixHeaderHeight - _theme.SpacingWidth);
                }

                Height = _theme.MatrixHeaderHeight + _theme.SpacingWidth;
                Width = _theme.MatrixCellSize * matrixSize + _theme.SpacingWidth;
            }
        }
        
        private int GetHoveredColumn(Point location)
        {
            double column = (location.X - _offset) / _pitch;
            return (int)column;
        }
    }

}
