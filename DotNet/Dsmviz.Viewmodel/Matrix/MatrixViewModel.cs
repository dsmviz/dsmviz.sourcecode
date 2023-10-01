using System.Collections.ObjectModel;
using Dsmviz.Viewmodel.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Main;
using Dsmviz.Viewmodel.Lists;

namespace Dsmviz.Viewmodel.Matrix
{
    public class MatrixViewmodel : ViewmodelBase, IMatrixViewmodel
    {
        private double _zoomLevel;
        private readonly IMainViewmodel _mainViewmodel;
        private readonly IDsmApplication _application;
        private readonly IEnumerable<IDsmElement> _selectedElements;
        private ObservableCollection<ElementTreeItemViewmodel> _elementViewmodelTree;
        private List<ElementTreeItemViewmodel> _elementViewmodelLeafs;
        private ElementTreeItemViewmodel _selectedTreeItem;
        private ElementTreeItemViewmodel _hoveredTreeItem;
        private int? _selectedRow;
        private int? _selectedColumn;
        private int? _hoveredRow;
        private int? _hoveredColumn;
        private int _matrixSize;
        private bool _isMetricsViewExpanded;

        private List<List<MatrixColor>> _cellColors;
        private List<List<int>> _cellWeights;
        private List<MatrixColor> _columnColors;
        private List<int> _columnElementIds;
        private List<string> _metrics;
        private int? _selectedConsumerId;
        private int? _selectedProviderId;

        private ElementToolTipViewmodel _columnHeaderTooltipViewmodel;
        private CellToolTipViewmodel _cellTooltipViewmodel;

        private readonly Dictionary<MetricType, string> _metricTypeNames;
        private string _selectedMetricTypeName;
        private MetricType _selectedMetricType;
        private string _searchText = "";

        public MatrixViewmodel(IMainViewmodel mainViewmodel, IDsmApplication application, IEnumerable<IDsmElement> selectedElements)
        {
            _mainViewmodel = mainViewmodel;
            _application = application;
            _selectedElements = selectedElements;

            ToggleElementExpandedCommand = mainViewmodel.ToggleElementExpandedCommand;

            SortElementCommand = mainViewmodel.SortElementCommand;
            MoveUpElementCommand = mainViewmodel.MoveUpElementCommand;
            MoveDownElementCommand = mainViewmodel.MoveDownElementCommand;

            ToggleElementBookmarkCommand = mainViewmodel.ToggleElementBookmarkCommand;

            AddChildElementCommand = mainViewmodel.AddChildElementCommand;
            AddSiblingElementAboveCommand = mainViewmodel.AddSiblingElementAboveCommand;
            AddSiblingElementBelowCommand = mainViewmodel.AddSiblingElementBelowCommand;
            ModifyElementCommand = mainViewmodel.ModifyElementCommand;
            ChangeElementParentCommand = mainViewmodel.ChangeElementParentCommand;
            DeleteElementCommand = mainViewmodel.DeleteElementCommand;

            CopyElementCommand = mainViewmodel.DeleteElementCommand;
            CutElementCommand = mainViewmodel.DeleteElementCommand;
            PasteAsChildElementCommand = mainViewmodel.DeleteElementCommand;
            PasteAsSiblingElementAboveCommand = mainViewmodel.DeleteElementCommand;
            PasteAsSiblingElementBelowCommand = mainViewmodel.DeleteElementCommand;

            ShowElementIngoingRelationsCommand = new RelayCommand<object>(ShowElementIngoingRelationsExecute, ShowElementIngoingRelationsCanExecute);
            ShowElementOutgoingRelationCommand = new RelayCommand<object>(ShowElementOutgoingRelationExecute, ShowElementOutgoingRelationCanExecute);
            ShowElementinternalRelationsCommand = new RelayCommand<object>(ShowElementinternalRelationsExecute, ShowElementinternalRelationsCanExecute);

            ShowElementConsumersCommand = new RelayCommand<object>(ShowElementConsumersExecute, ShowConsumersCanExecute);
            ShowElementProvidedInterfacesCommand = new RelayCommand<object>(ShowProvidedInterfacesExecute, ShowElementProvidedInterfacesCanExecute);
            ShowElementRequiredInterfacesCommand = new RelayCommand<object>(ShowElementRequiredInterfacesExecute, ShowElementRequiredInterfacesCanExecute);
            ShowCellDetailMatrixCommand = mainViewmodel.ShowCellDetailMatrixCommand;

            ShowCellRelationsCommand = new RelayCommand<object>(ShowCellRelationsExecute, ShowCellRelationsCanExecute);
            ShowCellConsumersCommand = new RelayCommand<object>(ShowCellConsumersExecute, ShowCellConsumersCanExecute);
            ShowCellProvidersCommand = new RelayCommand<object>(ShowCellProvidersExecute, ShowCellProvidersCanExecute);
            ShowElementDetailMatrixCommand = mainViewmodel.ShowElementDetailMatrixCommand;
            ShowElementContextMatrixCommand = mainViewmodel.ShowElementContextMatrixCommand;

            ToggleMetricsViewExpandedCommand = new RelayCommand<object>(ToggleMetricsViewExpandedExecute, ToggleMetricsViewExpandedCanExecute);

            PreviousMetricCommand = new RelayCommand<object>(PreviousMetricExecute, PreviousMetricCanExecute);
            NextMetricCommand = new RelayCommand<object>(NextMetricExecute, NextMetricCanExecute);

            Reload();

            ZoomLevel = 1.0;

            _metricTypeNames = new Dictionary<MetricType, string>
            {
                [MetricType.NumberOfElements] = "Internal\nElements",
                [MetricType.RelativeSizePercentage] = "Relative\nSize",
                [MetricType.IngoingRelations] = "Ingoing Relations",
                [MetricType.OutgoingRelations] = "Outgoing\nRelations",
                [MetricType.InternalRelations] = "Internal\nRelations",
                [MetricType.ExternalRelations] = "External\nRelations",
                [MetricType.HierarchicalCycles] = "Hierarchical\nCycles",
                [MetricType.SystemCycles] = "System\nCycles",
                [MetricType.Cycles] = "Total\nCycles",
                [MetricType.CycalityPercentage] = "Total\nCycality"
            };

            _selectedMetricType = MetricType.NumberOfElements;
            SelectedMetricTypeName = _metricTypeNames[_selectedMetricType];
        }

        public ICommand ToggleElementExpandedCommand { get; }

        public ICommand SortElementCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }

        public ICommand ToggleElementBookmarkCommand { get; }

        public ICommand AddChildElementCommand { get; }
        public ICommand AddSiblingElementAboveCommand { get; }
        public ICommand AddSiblingElementBelowCommand { get; }
        public ICommand ModifyElementCommand { get; }
        public ICommand ChangeElementParentCommand { get; }
        public ICommand DeleteElementCommand { get; }

        public ICommand CopyElementCommand { get; }
        public ICommand CutElementCommand { get; }
        public ICommand PasteAsChildElementCommand { get; }
        public ICommand PasteAsSiblingElementAboveCommand { get; }
        public ICommand PasteAsSiblingElementBelowCommand { get; }

        public ICommand ShowElementIngoingRelationsCommand { get; }
        public ICommand ShowElementOutgoingRelationCommand { get; }
        public ICommand ShowElementinternalRelationsCommand { get; }

        public ICommand ShowElementConsumersCommand { get; }
        public ICommand ShowElementProvidedInterfacesCommand { get; }
        public ICommand ShowElementRequiredInterfacesCommand { get; }
        public ICommand ShowElementDetailMatrixCommand { get; }
        public ICommand ShowElementContextMatrixCommand { get; }

        public ICommand ShowCellRelationsCommand { get; }
        public ICommand ShowCellConsumersCommand { get; }
        public ICommand ShowCellProvidersCommand { get; }
        public ICommand ShowCellDetailMatrixCommand { get; }

        public ICommand PreviousMetricCommand { get; }
        public ICommand NextMetricCommand { get; }

        public ICommand ToggleMetricsViewExpandedCommand { get; }

        public string SelectedMetricTypeName
        {
            get { return _selectedMetricTypeName; }
            set
            {
                _selectedMetricTypeName = value;
                _selectedMetricType = _metricTypeNames.FirstOrDefault(x => x.Value == _selectedMetricTypeName).Key;
                Reload();
                OnPropertyChanged();
            }
        }

        public int MatrixSize
        {
            get { return _matrixSize; }
            set { _matrixSize = value; OnPropertyChanged(); }
        }

        public bool IsMetricsViewExpanded
        {
            get { return _isMetricsViewExpanded; }
            set { _isMetricsViewExpanded = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ElementTreeItemViewmodel> ElementViewmodelTree
        {
            get { return _elementViewmodelTree; }
            private set { _elementViewmodelTree = value; OnPropertyChanged(); }
        }

        public IReadOnlyList<MatrixColor> ColumnColors => _columnColors;
        public IReadOnlyList<int> ColumnElementIds => _columnElementIds;
        public IReadOnlyList<IList<MatrixColor>> CellColors => _cellColors;
        public IReadOnlyList<IReadOnlyList<int>> CellWeights => _cellWeights;
        public IReadOnlyList<string> Metrics => _metrics;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { _zoomLevel = value; OnPropertyChanged(); }
        }

        public void Reload()
        {
            BackupSelectionBeforeReload();
            ElementViewmodelTree = CreateElementViewmodelTree();
            _elementViewmodelLeafs = FindLeafElementViewmodels();
            DefineColumnColors();
            DefineColumnContent();
            DefineCellColors();
            DefineCellContent();
            DefineMetrics();
            MatrixSize = _elementViewmodelLeafs.Count;
            RestoreSelectionAfterReload();
        }

        public void SelectTreeItem(ElementTreeItemViewmodel selectedTreeItem)
        {
            SelectCell(null, null);
            for (int row = 0; row < _elementViewmodelLeafs.Count; row++)
            {
                if (_elementViewmodelLeafs[row] == selectedTreeItem)
                {
                    SelectRow(row);
                }
            }
            _selectedTreeItem = selectedTreeItem;
        }

        public ElementTreeItemViewmodel SelectedTreeItem
        {
            get
            {
                ElementTreeItemViewmodel selectedTreeItem;
                if (SelectedRow.HasValue && (SelectedRow.Value < _elementViewmodelLeafs.Count))
                {
                    selectedTreeItem = _elementViewmodelLeafs[SelectedRow.Value];
                }
                else
                {
                    selectedTreeItem = _selectedTreeItem;
                }
                return selectedTreeItem;
            }
        }

        public void HoverTreeItem(ElementTreeItemViewmodel hoveredTreeItem)
        {
            HoverCell(null, null);
            for (int row = 0; row < _elementViewmodelLeafs.Count; row++)
            {
                if (_elementViewmodelLeafs[row] == hoveredTreeItem)
                {
                    HoverRow(row);
                }
            }
            _hoveredTreeItem = hoveredTreeItem;
        }

        public ElementTreeItemViewmodel HoveredTreeItem
        {
            get
            {
                ElementTreeItemViewmodel hoveredTreeItem;
                if (HoveredRow.HasValue && (HoveredRow.Value < _elementViewmodelLeafs.Count))
                {
                    hoveredTreeItem = _elementViewmodelLeafs[HoveredRow.Value];
                }
                else
                {
                    hoveredTreeItem = _hoveredTreeItem;
                }
                return hoveredTreeItem;
            }
        }

        public void SelectRow(int? row)
        {
            SelectedRow = row;
            SelectedColumn = null;
            UpdateProviderRows();
            UpdateConsumerRows();

            SelectedCellHasRelationCount = 0;
        }

        public void SelectColumn(int? column)
        {
            SelectedRow = null;
            SelectedColumn = column;
            UpdateProviderRows();
            UpdateConsumerRows();

            SelectedCellHasRelationCount = 0;
        }

        public void SelectCell(int? row, int? columnn)
        {
            SelectedRow = row;
            SelectedColumn = columnn;
            UpdateProviderRows();
            UpdateConsumerRows();

            SelectedCellHasRelationCount = _application.GetRelationCount(SelectedConsumer, SelectedProvider);
        }

        public int? SelectedRow
        {
            get { return _selectedRow; }
            private set { _selectedRow = value; OnPropertyChanged(); }
        }

        public int? SelectedColumn
        {
            get { return _selectedColumn; }
            private set { _selectedColumn = value; OnPropertyChanged(); }
        }

        public void HoverRow(int? row)
        {
            HoveredRow = row;
            HoveredColumn = null;
        }

        public void HoverColumn(int? column)
        {
            HoveredRow = null;
            HoveredColumn = column;
            UpdateColumnHeaderTooltip(column);
        }

        public void HoverCell(int? row, int? columnn)
        {
            HoveredRow = row;
            HoveredColumn = columnn;
            UpdateCellTooltip(row, columnn);
        }

        public int? HoveredRow
        {
            get { return _hoveredRow; }
            private set { _hoveredRow = value; OnPropertyChanged(); }
        }

        public int? HoveredColumn
        {
            get { return _hoveredColumn; }
            private set { _hoveredColumn = value; OnPropertyChanged(); }
        }

        public IDsmElement SelectedConsumer
        {
            get
            {
                IDsmElement selectedConsumer = null;
                if (SelectedColumn.HasValue)
                {
                    selectedConsumer = _elementViewmodelLeafs[SelectedColumn.Value].Element;
                }
                return selectedConsumer;
            }
        }

        public IDsmElement SelectedProvider => SelectedTreeItem?.Element;

        public int SelectedCellHasRelationCount { get; private set; }

        public ElementToolTipViewmodel ColumnHeaderToolTipViewmodel
        {
            get { return _columnHeaderTooltipViewmodel; }
            set { _columnHeaderTooltipViewmodel = value; OnPropertyChanged(); }
        }

        public CellToolTipViewmodel CellToolTipViewmodel
        {
            get { return _cellTooltipViewmodel; }
            set { _cellTooltipViewmodel = value; OnPropertyChanged(); }
        }

        public IEnumerable<string> MetricTypes => _metricTypeNames.Values;

        private ObservableCollection<ElementTreeItemViewmodel> CreateElementViewmodelTree()
        {
            int depth = 0;
            ObservableCollection<ElementTreeItemViewmodel> tree = new ObservableCollection<ElementTreeItemViewmodel>();
            foreach (IDsmElement element in _selectedElements)
            {
                ElementTreeItemViewmodel Viewmodel = new ElementTreeItemViewmodel(_mainViewmodel, this, _application, element, depth);
                tree.Add(Viewmodel);
                AddElementViewmodelChildren(Viewmodel);
            }
            return tree;
        }

        private void AddElementViewmodelChildren(ElementTreeItemViewmodel Viewmodel)
        {
            if (Viewmodel.Element.IsExpanded)
            {
                foreach (IDsmElement child in Viewmodel.Element.Children)
                {
                    ElementTreeItemViewmodel childViewmodel = new ElementTreeItemViewmodel(_mainViewmodel, this, _application, child, Viewmodel.Depth + 1);
                    Viewmodel.AddChild(childViewmodel);
                    AddElementViewmodelChildren(childViewmodel);
                }
            }
            else
            {
                Viewmodel.ClearChildren();
            }
        }

        private List<ElementTreeItemViewmodel> FindLeafElementViewmodels()
        {
            List<ElementTreeItemViewmodel> leafViewmodels = new List<ElementTreeItemViewmodel>();

            foreach (ElementTreeItemViewmodel Viewmodel in ElementViewmodelTree)
            {
                FindLeafElementViewmodels(leafViewmodels, Viewmodel);
            }

            return leafViewmodels;
        }

        private void FindLeafElementViewmodels(List<ElementTreeItemViewmodel> leafViewmodels, ElementTreeItemViewmodel Viewmodel)
        {
            if (!Viewmodel.IsExpanded)
            {
                leafViewmodels.Add(Viewmodel);
            }

            foreach (ElementTreeItemViewmodel childViewmodel in Viewmodel.Children)
            {
                FindLeafElementViewmodels(leafViewmodels, childViewmodel);
            }
        }

        private void DefineCellColors()
        {
            int matrixSize = _elementViewmodelLeafs.Count;

            _cellColors = new List<List<MatrixColor>>();

            // Define background color
            for (int row = 0; row < matrixSize; row++)
            {
                _cellColors.Add(new List<MatrixColor>());
                for (int column = 0; column < matrixSize; column++)
                {
                    _cellColors[row].Add(MatrixColor.Background);
                }
            }

            // Define expanded block color
            for (int row = 0; row < matrixSize; row++)
            {
                ElementTreeItemViewmodel Viewmodel = _elementViewmodelLeafs[row];

                Stack<ElementTreeItemViewmodel> ViewmodelHierarchy = new Stack<ElementTreeItemViewmodel>();
                ElementTreeItemViewmodel child = Viewmodel;
                ElementTreeItemViewmodel parent = Viewmodel.Parent;
                while ((parent != null) && (parent.Children[0] == child))
                {
                    ViewmodelHierarchy.Push(parent);
                    child = parent;
                    parent = parent.Parent;
                }

                foreach (ElementTreeItemViewmodel currentViewmodel in ViewmodelHierarchy)
                {
                    int leafElements = 0;
                    CountLeafElements(currentViewmodel.Element, ref leafElements);

                    if (leafElements > 0 && currentViewmodel.Depth > 0)
                    {
                        MatrixColor expandedColor = MatrixColorConverter.GetColor(currentViewmodel.Depth);

                        int begin = row;
                        int end = row + leafElements;

                        for (int rowDelta = begin; rowDelta < end; rowDelta++)
                        {
                            for (int columnDelta = begin; columnDelta < end; columnDelta++)
                            {
                                _cellColors[rowDelta][columnDelta] = expandedColor;
                            }
                        }
                    }
                }
            }

            // Define diagonal color
            for (int row = 0; row < matrixSize; row++)
            {
                int depth = _elementViewmodelLeafs[row].Depth;
                MatrixColor dialogColor = MatrixColorConverter.GetColor(depth);
                _cellColors[row][row] = dialogColor;
            }

            // Define cycle color
            for (int row = 0; row < matrixSize; row++)
            {
                for (int column = 0; column < matrixSize; column++)
                {
                    IDsmElement consumer = _elementViewmodelLeafs[column].Element;
                    IDsmElement provider = _elementViewmodelLeafs[row].Element;
                    CycleType cycleType = _application.IsCyclicDependency(consumer, provider);
                    if (cycleType != CycleType.None)
                    {
                        _cellColors[row][column] = MatrixColor.Cycle;
                    }
                }
            }
        }

        private void CountLeafElements(IDsmElement element, ref int count)
        {
            if (!element.IsExpanded)
            {
                count++;
            }
            else
            {
                foreach (IDsmElement child in element.Children)
                {
                    CountLeafElements(child, ref count);
                }
            }
        }

        private void DefineColumnColors()
        {
            _columnColors = new List<MatrixColor>();
            foreach (ElementTreeItemViewmodel provider in _elementViewmodelLeafs)
            {
                _columnColors.Add(provider.Color);
            }
        }

        private void DefineColumnContent()
        {
            _columnElementIds = new List<int>();
            foreach (ElementTreeItemViewmodel provider in _elementViewmodelLeafs)
            {
                _columnElementIds.Add(provider.Element.Order);
            }
        }

        private void DefineCellContent()
        {
            _cellWeights = new List<List<int>>();
            int matrixSize = _elementViewmodelLeafs.Count;

            for (int row = 0; row < matrixSize; row++)
            {
                _cellWeights.Add(new List<int>());
                for (int column = 0; column < matrixSize; column++)
                {
                    IDsmElement consumer = _elementViewmodelLeafs[column].Element;
                    IDsmElement provider = _elementViewmodelLeafs[row].Element;
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    _cellWeights[row].Add(weight);
                }
            }
        }

        private void DefineMetrics()
        {
            _metrics = new List<string>();
            switch (_selectedMetricType)
            {
                case MetricType.NumberOfElements:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int childElementCount = _application.GetElementSize(Viewmodel.Element);
                        _metrics.Add($"{childElementCount}");
                    }
                    break;
                case MetricType.RelativeSizePercentage:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int childElementCount = _application.GetElementSize(Viewmodel.Element);
                        int totalElementCount = _application.GetElementCount();
                        double metricCount = (totalElementCount > 0) ? childElementCount * 100.0 / totalElementCount : 0;
                        _metrics.Add($"{metricCount:0.000} %");
                    }
                    break;
                case MetricType.IngoingRelations:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int metricCount = _application.FindIngoingRelations(Viewmodel.Element).Count();
                        _metrics.Add($"{metricCount}");
                    }
                    break;
                case MetricType.OutgoingRelations:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int metricCount = _application.FindOutgoingRelations(Viewmodel.Element).Count();
                        _metrics.Add($"{metricCount}");
                    }
                    break;
                case MetricType.InternalRelations:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int metricCount = _application.FindInternalRelations(Viewmodel.Element).Count();
                        _metrics.Add($"{metricCount}");
                    }
                    break;
                case MetricType.ExternalRelations:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int metricCount = _application.FindExternalRelations(Viewmodel.Element).Count();
                        _metrics.Add($"{metricCount}");
                    }
                    break;
                case MetricType.HierarchicalCycles:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int metricCount = _application.GetHierarchicalCycleCount(Viewmodel.Element);
                        _metrics.Add(metricCount > 0 ? $"{metricCount}" : "-");
                    }
                    break;
                case MetricType.SystemCycles:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int metricCount = _application.GetSystemCycleCount(Viewmodel.Element);
                        _metrics.Add(metricCount > 0 ? $"{metricCount}" : "-");
                    }
                    break;
                case MetricType.Cycles:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int metricCount = _application.GetHierarchicalCycleCount(Viewmodel.Element) +
                                          _application.GetSystemCycleCount(Viewmodel.Element);
                        _metrics.Add(metricCount > 0 ? $"{metricCount}" : "-");
                    }
                    break;
                case MetricType.CycalityPercentage:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        int cycleCount = _application.GetHierarchicalCycleCount(Viewmodel.Element) +
                                          _application.GetSystemCycleCount(Viewmodel.Element);
                        int relationCount = _application.FindInternalRelations(Viewmodel.Element).Count();
                        double metricCount = (relationCount > 0) ? (cycleCount * 100.0 / relationCount) : 0;
                        _metrics.Add(metricCount > 0 ? $"{metricCount:0.000} %" : "-");
                    }
                    break;
                default:
                    foreach (ElementTreeItemViewmodel Viewmodel in _elementViewmodelLeafs)
                    {
                        _metrics.Add("");
                    }
                    break;
            }
        }

        private void ShowCellConsumersExecute(object parameter)
        {
            _mainViewmodel.NotifyElementsReportReady(ElementListViewmodelType.RelationConsumers, SelectedConsumer, SelectedProvider);
        }

        private bool ShowCellConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellProvidersExecute(object parameter)
        {
            _mainViewmodel.NotifyElementsReportReady(ElementListViewmodelType.RelationProviders, SelectedConsumer, SelectedProvider);
        }

        private bool ShowCellProvidersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementIngoingRelationsExecute(object parameter)
        {
            _mainViewmodel.NotifyRelationsReportReady(RelationsListViewmodelType.ElementIngoingRelations, null, SelectedProvider);
        }

        private bool ShowElementIngoingRelationsCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementOutgoingRelationExecute(object parameter)
        {
            var relations = _application.FindOutgoingRelations(SelectedProvider);
            _mainViewmodel.NotifyRelationsReportReady(RelationsListViewmodelType.ElementOutgoingRelations, null, SelectedProvider);
        }

        private bool ShowElementOutgoingRelationCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementinternalRelationsExecute(object parameter)
        {
            _mainViewmodel.NotifyRelationsReportReady(RelationsListViewmodelType.ElementInternalRelations, null, SelectedProvider);
        }

        private bool ShowElementinternalRelationsCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementConsumersExecute(object parameter)
        {
            _mainViewmodel.NotifyElementsReportReady(ElementListViewmodelType.ElementConsumers, null, SelectedProvider);
        }

        private bool ShowConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowProvidedInterfacesExecute(object parameter)
        {
            _mainViewmodel.NotifyElementsReportReady(ElementListViewmodelType.ElementProvidedInterface, null, SelectedProvider);
        }

        private bool ShowElementProvidedInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementRequiredInterfacesExecute(object parameter)
        {
            _mainViewmodel.NotifyElementsReportReady(ElementListViewmodelType.ElementRequiredInterface, null, SelectedProvider);
        }

        private bool ShowElementRequiredInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellRelationsExecute(object parameter)
        {
            _mainViewmodel.NotifyRelationsReportReady(RelationsListViewmodelType.ConsumerProviderRelations, SelectedConsumer, SelectedProvider);
        }

        private bool ShowCellRelationsCanExecute(object parameter)
        {
            return true;
        }

        private void ToggleMetricsViewExpandedExecute(object parameter)
        {
            IsMetricsViewExpanded = !IsMetricsViewExpanded;
        }

        private bool ToggleMetricsViewExpandedCanExecute(object parameter)
        {
            return true;
        }

        private void PreviousMetricExecute(object parameter)
        {
            _selectedMetricType--;
            SelectedMetricTypeName = _metricTypeNames[_selectedMetricType];
        }

        private bool PreviousMetricCanExecute(object parameter)
        {
            return _selectedMetricType != MetricType.NumberOfElements;
        }

        private void NextMetricExecute(object parameter)
        {
            _selectedMetricType++;
            SelectedMetricTypeName = _metricTypeNames[_selectedMetricType];
        }

        private bool NextMetricCanExecute(object parameter)
        {
            return _selectedMetricType != MetricType.CycalityPercentage;
        }

        private void UpdateColumnHeaderTooltip(int? column)
        {
            if (column.HasValue)
            {
                IDsmElement element = _elementViewmodelLeafs[column.Value].Element;
                if (element != null)
                {
                    ColumnHeaderToolTipViewmodel = new ElementToolTipViewmodel(element, _application);
                }
            }
        }

        private void UpdateCellTooltip(int? row, int? column)
        {
            if (row.HasValue && column.HasValue)
            {
                IDsmElement consumer = _elementViewmodelLeafs[column.Value].Element;
                IDsmElement provider = _elementViewmodelLeafs[row.Value].Element;

                if ((consumer != null) && (provider != null))
                {
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    CycleType cycleType = _application.IsCyclicDependency(consumer, provider);
                    CellToolTipViewmodel = new CellToolTipViewmodel(consumer, provider, weight, cycleType);
                }
            }
        }

        private void SelectElement(IDsmElement element)
        {
            SelectElement(ElementViewmodelTree, element);
        }

        private void SelectElement(IEnumerable<ElementTreeItemViewmodel> tree, IDsmElement element)
        {
            foreach (ElementTreeItemViewmodel treeItem in tree)
            {
                if (treeItem.Id == element.Id)
                {
                    SelectTreeItem(treeItem);
                }
                else
                {
                    SelectElement(treeItem.Children, element);
                }
            }
        }

        private void ExpandElement(IDsmElement element)
        {
            IDsmElement current = element.Parent;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.Parent;
            }
            Reload();
        }

        private void UpdateProviderRows()
        {
            if (SelectedRow.HasValue)
            {
                for (int row = 0; row < _elementViewmodelLeafs.Count; row++)
                {
                    _elementViewmodelLeafs[row].IsProvider = _cellWeights[row][SelectedRow.Value] > 0;
                }
            }
            else
            {
                for (int row = 0; row < _elementViewmodelLeafs.Count; row++)
                {
                    _elementViewmodelLeafs[row].IsProvider = false;
                }
            }
        }

        private void UpdateConsumerRows()
        {
            if (SelectedRow.HasValue)
            {
                for (int row = 0; row < _elementViewmodelLeafs.Count; row++)
                {
                    _elementViewmodelLeafs[row].IsConsumer = _cellWeights[SelectedRow.Value][row] > 0;
                }
            }
            else
            {
                for (int row = 0; row < _elementViewmodelLeafs.Count; row++)
                {
                    _elementViewmodelLeafs[row].IsConsumer = false;
                }
            }
        }

        private void BackupSelectionBeforeReload()
        {
            _selectedConsumerId = SelectedConsumer?.Id;
            _selectedProviderId = SelectedProvider?.Id;
        }

        private void RestoreSelectionAfterReload()
        {
            for (int i = 0; i < _elementViewmodelLeafs.Count; i++)
            {
                if (_selectedProviderId.HasValue && (_selectedProviderId.Value == _elementViewmodelLeafs[i].Id))
                {
                    SelectRow(i);
                }

                if (_selectedConsumerId.HasValue && (_selectedConsumerId.Value == _elementViewmodelLeafs[i].Id))
                {
                    SelectColumn(i);
                }
            }
        }
    }
}
