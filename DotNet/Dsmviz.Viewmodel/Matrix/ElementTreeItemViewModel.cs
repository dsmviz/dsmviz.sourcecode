using System.Windows.Input;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Common;
using System.Collections.Generic;
using Dsmviz.Application.Interfaces;
using Dsmviz.Viewmodel.Main;

namespace Dsmviz.Viewmodel.Matrix
{
    public class ElementTreeItemViewmodel : ViewmodelBase
    {
        private readonly List<ElementTreeItemViewmodel> _children;
        private ElementTreeItemViewmodel _parent;
        private bool _isDropTarget;
        private MatrixColor _color;

        public ElementTreeItemViewmodel(IMainViewmodel mainViewmodel, IMatrixViewmodel matrixViewmodel, IDsmApplication application, IDsmElement element, int depth)
        {
            _children = new List<ElementTreeItemViewmodel>();
            _parent = null;
            Element = element;
            Depth = depth;
            UpdateColor();

            MoveCommand = matrixViewmodel.ChangeElementParentCommand;
            MoveUpElementCommand = matrixViewmodel.MoveUpElementCommand;
            MoveDownElementCommand = matrixViewmodel.MoveDownElementCommand;
            SortElementCommand = matrixViewmodel.SortElementCommand;
            ToggleElementExpandedCommand = matrixViewmodel.ToggleElementExpandedCommand;
            BookmarkElementCommand = matrixViewmodel.ToggleElementBookmarkCommand;

            SelectedIndicatorViewMode = mainViewmodel.SelectedIndicatorViewMode;

            ToolTipViewmodel = new ElementToolTipViewmodel(Element, application);
        }

        public ElementToolTipViewmodel ToolTipViewmodel { get; }
        public IDsmElement Element { get; }

        public bool IsDropTarget
        {
            get { return _isDropTarget; }
            set { _isDropTarget = value; UpdateColor(); OnPropertyChanged(); }
        }

        public MatrixColor Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged();  }
        }

        public int Depth { get; }

        public int Id => Element.Id;
        public int Order => Element.Order;
        public bool IsConsumer { get; set; }
        public bool IsProvider { get; set; }
        public bool IsMatch => Element.IsMatch;
        public bool IsBookmarked => Element.IsBookmarked;
        public string Name => Element.IsRoot ? "Root" : Element.Name;

        public string Fullname => Element.Fullname;

        public ICommand MoveCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }
        public ICommand SortElementCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }
        public ICommand BookmarkElementCommand { get; }

        public IndicatorViewMode SelectedIndicatorViewMode { get; }

        public bool IsExpandable => Element.HasChildren;

        public bool IsExpanded
        {
            get
            {
                return Element.IsExpanded;
            }
            set
            {
                Element.IsExpanded = value;
            }
        }

        public IReadOnlyList<ElementTreeItemViewmodel> Children => _children;

        public ElementTreeItemViewmodel Parent => _parent;

        public void AddChild(ElementTreeItemViewmodel Viewmodel)
        {
            _children.Add(Viewmodel);
            Viewmodel._parent = this;
        }

        public void ClearChildren()
        {
            foreach (ElementTreeItemViewmodel Viewmodel in _children)
            {
                Viewmodel._parent = null;
            }
            _children.Clear();
        }

        public int LeafElementCount
        {
            get
            {
                int count = 0;
                CountLeafElements(this, ref count);
                return count;
            }
        }

        private void CountLeafElements(ElementTreeItemViewmodel Viewmodel, ref int count)
        {
            if (Viewmodel.Children.Count == 0)
            {
                count++;
            }
            else
            {
                foreach (ElementTreeItemViewmodel child in Viewmodel.Children)
                {
                    CountLeafElements(child, ref count);
                }
            }
        }

        private void UpdateColor()
        {
            if (_isDropTarget)
            {
                Color = MatrixColor.Cycle;
            }
            else
            {
                Color =  MatrixColorConverter.GetColor(Depth);
            }
        }
    }
}
