using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Dsmviz.Viewmodel.Matrix;

namespace Dsmviz.View.Matrix
{
    public class MatrixCellsView : MatrixFrameworkElement
    {
        private MatrixViewmodel _Viewmodel;
        private readonly MatrixTheme _theme;
        private Rect _rect;
        private int? _hoveredRow;
        private int? _hoveredColumn;
        private readonly double _pitch;
        private readonly double _offset;
        private readonly double _verticalTextOffset = 16.0;

        public MatrixCellsView()
        {
            _theme = new MatrixTheme(this);
            _rect = new Rect(new Size(_theme.MatrixCellSize, _theme.MatrixCellSize));
            _hoveredRow = null;
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
            int row = GetHoveredRow(e.GetPosition(this));
            int column = GetHoveredColumn(e.GetPosition(this));
            if ((_hoveredRow != row) || (_hoveredColumn != column))
            {
                _hoveredRow = row;
                _hoveredColumn = column;
                _Viewmodel.HoverCell(row, column);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _Viewmodel.HoverCell(null, null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            int column = GetHoveredColumn(e.GetPosition(this));
            _Viewmodel.SelectCell(row, column);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewmodel.CellToolTipViewmodel))
            {
                ToolTip = _Viewmodel.CellToolTipViewmodel;
            }

            if ((e.PropertyName == nameof(MatrixViewmodel.MatrixSize)) ||
                (e.PropertyName == nameof(MatrixViewmodel.HoveredRow)) ||
                (e.PropertyName == nameof(MatrixViewmodel.SelectedRow)) ||
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
                    for (int column = 0; column < matrixSize; column++)
                    {
                        _rect.X = _offset + column * _pitch;
                        _rect.Y = _offset + row * _pitch;

                        bool isHovered = (_Viewmodel.HoveredRow.HasValue && (row == _Viewmodel.HoveredRow.Value)) ||
                                         (_Viewmodel.HoveredColumn.HasValue && (column == _Viewmodel.HoveredColumn.Value));
                        bool isSelected = (_Viewmodel.SelectedRow.HasValue && (row == _Viewmodel.SelectedRow.Value)) ||
                                          (_Viewmodel.SelectedColumn.HasValue && (column == _Viewmodel.SelectedColumn.Value));
                        MatrixColor color = _Viewmodel.CellColors[row][column];
                        SolidColorBrush background = _theme.GetBackground(color, isHovered, isSelected);

                        dc.DrawRectangle(background, null, _rect);

                        int weight = _Viewmodel.CellWeights[row][column];
                        if (weight > 0)
                        {
                            char infinity = '\u221E';
                            string content = weight > 9999 ? infinity.ToString() : weight.ToString();

                            double textWidth = MeasureText(content);

                            Point location = new Point
                            {
                                X = (column * _pitch) + (_pitch - textWidth) / 2,
                                Y = (row * _pitch) + _verticalTextOffset
                            };
                            DrawText(dc, content, location, _theme.TextColor,_rect.Width - _theme.SpacingWidth);
                        }
                    }
                }
                Height = Width = _pitch * matrixSize;
            }
        }

        private int GetHoveredRow(Point location)
        {
            double row = (location.Y - _offset) / _pitch;
            return (int)row;
        }

        private int GetHoveredColumn(Point location)
        {
            double column = (location.X - _offset) / _pitch;
            return (int)column;
        }
    }
}
