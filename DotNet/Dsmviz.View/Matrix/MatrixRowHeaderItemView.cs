using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Main;
using Dsmviz.Viewmodel.Matrix;

namespace Dsmviz.View.Matrix
{
    public class MatrixRowHeaderItemView : MatrixFrameworkElement
    {
        private readonly MatrixViewmodel _matrixViewmodel;
        private static readonly string DataObjectName = "Element";
        private readonly MatrixTheme _theme;
        private ElementTreeItemViewmodel _Viewmodel;
        private readonly int _indicatorWith = 5;

        public MatrixRowHeaderItemView(MatrixViewmodel matrixViewmodel, MatrixTheme theme)
        {
            _matrixViewmodel = matrixViewmodel;
            _matrixViewmodel.PropertyChanged += OnMatrixViewmodelPropertyChanged;
            _theme = theme;

            AllowDrop = true;

            DataContextChanged += OnDataContextChanged;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData(DataObjectName, _Viewmodel);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            Mouse.SetCursor(e.Effects.HasFlag(DragDropEffects.Move) ? Cursors.Pen : Cursors.Arrow);
            e.Handled = true;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (IsValidDropTarget(e))
            {
                _Viewmodel.IsDropTarget = true;
            }
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            _Viewmodel.IsDropTarget = false;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            
            e.Effects = IsValidDropTarget(e) ? DragDropEffects.Move : DragDropEffects.None;
 
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataObjectName))
            {
                ElementTreeItemViewmodel dragged = (ElementTreeItemViewmodel)e.Data.GetData(DataObjectName);
                ElementTreeItemViewmodel dropTarget = _Viewmodel;

                if ((dragged != null) &&
                    (dropTarget != null) &&
                    (dragged != dropTarget))
                {
                    int index = GetDropAtIndex(e);
                    Tuple<IDsmElement, IDsmElement, int> moveParameter = new Tuple<IDsmElement, IDsmElement, int>(dragged.Element, dropTarget.Element, index);
                    _Viewmodel.MoveCommand.Execute(moveParameter);
                }

                e.Effects = DragDropEffects.Move;
            }
            _Viewmodel.IsDropTarget = false;
            e.Handled = true;
        }

        private bool IsValidDropTarget(DragEventArgs e)
        {
            bool isValidDropTarget = false;

            if (e.Data.GetDataPresent(DataObjectName))
            {
                ElementTreeItemViewmodel dragged = (ElementTreeItemViewmodel)e.Data.GetData(DataObjectName);
                ElementTreeItemViewmodel dropTarget = _Viewmodel;

                if ((dragged != null) &&
                    (dropTarget != null) &&
                    (!dropTarget.Element.IsRecursiveChildOf(dragged.Element)))
                {
                    isValidDropTarget = true;
                }
            }

            return isValidDropTarget;
        }

        private int GetDropAtIndex(DragEventArgs e)
        {
            Point point = e.GetPosition(this);

            double pitch = _theme.MatrixCellSize + 2.0;

            int index = (int) (point.Y / pitch);
            return index;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _Viewmodel = e.NewValue as ElementTreeItemViewmodel;
            if (_Viewmodel != null)
            {
                _Viewmodel.PropertyChanged += OnViewmodelPropertyChanged;
                ToolTip = _Viewmodel.ToolTipViewmodel;
            }
        }

        public void Redraw()
        {
            InvalidateVisual();
        }

        private void OnMatrixViewmodelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(MatrixViewmodel.SelectedRow)) ||
                (e.PropertyName == nameof(MatrixViewmodel.HoveredRow)))
            {
                InvalidateVisual();
            }
        }

        private void OnViewmodelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ElementTreeItemViewmodel.Color))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if ((_Viewmodel != null) && (ActualWidth > _theme.SpacingWidth) && (ActualHeight > _theme.SpacingWidth))
            {
                bool isHovered = _matrixViewmodel.HoveredTreeItem == _Viewmodel;
                bool isSelected = _matrixViewmodel.SelectedTreeItem == _Viewmodel;
                SolidColorBrush background = _theme.GetBackground(_Viewmodel.Color, isHovered, isSelected);
                Rect backgroundRect = new Rect(1.0, 1.0, ActualWidth - _theme.SpacingWidth, ActualHeight - _theme.SpacingWidth);
                dc.DrawRectangle(background, null, backgroundRect);

                string content = _Viewmodel.Name;

                if (_Viewmodel.IsExpanded)
                {
                    Point textLocation = new Point(backgroundRect.X + 10.0, backgroundRect.Y - 20.0);
                    DrawRotatedText(dc, content, textLocation, _theme.TextColor, backgroundRect.Height - 20.0);
                }
                else
                {
                    Rect indicatorRect =  new Rect(backgroundRect.Width - _indicatorWith, 1.0, _indicatorWith, ActualHeight - _theme.SpacingWidth);

                    switch (_Viewmodel.SelectedIndicatorViewMode)
                    {
                        case IndicatorViewMode.Default:
                            {
                                SolidColorBrush brush = GetIndicatorColor();
                                if (brush != null)
                                {
                                    dc.DrawRectangle(brush, null, indicatorRect);
                                };
                            }
                            break;
                        case IndicatorViewMode.Search:
                            {
                                if (_Viewmodel.IsMatch)
                                {
                                    dc.DrawRectangle(_theme.MatrixColorMatch, null, indicatorRect);
                                }
                            }
                            break;
                        case IndicatorViewMode.Bookmarks:
                            {
                                if (_Viewmodel.IsBookmarked)
                                {
                                    dc.DrawRectangle(_theme.MatrixColorBookmark, null, indicatorRect);
                                }
                            }
                            break;
                    }

                    if (ActualWidth > 70.0)
                    {
                        Point contentTextLocation = new Point(backgroundRect.X + 20.0, backgroundRect.Y + 15.0);
                        DrawText(dc, content, contentTextLocation, _theme.TextColor, ActualWidth - 70.0);
                    }

                    string order = _Viewmodel.Order.ToString();
                    double textWidth = MeasureText(order);

                    Point orderTextLocation = new Point(backgroundRect.X - 25.0 + backgroundRect.Width - textWidth, backgroundRect.Y + 15.0);
                    if (orderTextLocation.X > 0)
                    {
                        DrawText(dc, order, orderTextLocation, _theme.TextColor, ActualWidth - 25.0);
                    }
                }

                Point expanderLocation = new Point(backgroundRect.X + 1.0, backgroundRect.Y + 1.0);
                DrawExpander(dc, expanderLocation);
            }
        }

        private SolidColorBrush GetIndicatorColor()
        {
            SolidColorBrush brush = null;
            if (_Viewmodel.IsConsumer)
            {
                if (_Viewmodel.IsProvider)
                {
                    brush = _theme.MatrixColorCycle;
                }
                else
                {
                    brush = _theme.MatrixColorConsumer;
                }
            }
            else if (_Viewmodel.IsProvider)
            {
                brush = _theme.MatrixColorProvider;
            }

            return brush;
        }

        private void DrawExpander(DrawingContext dc, Point location)
        {
            if (_Viewmodel.IsExpandable)
            {
                dc.DrawText(
                    _Viewmodel.IsExpanded
                        ? _theme.DownArrowFormattedText
                        : _theme.RightArrowFormattedText, location);
            }
        }
    }
}