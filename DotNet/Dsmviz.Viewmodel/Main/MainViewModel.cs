using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Dsmviz.Viewmodel.Common;
using Dsmviz.Viewmodel.Matrix;
using Dsmviz.Viewmodel.Lists;
using System.Linq;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Editing.Element;
using Dsmviz.Viewmodel.Editing.Relation;
using Dsmviz.Viewmodel.Editing.Snapshot;
using Dsmviz.Util;
using Dsmviz.Viewmodel.Settings;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Dsmviz.Viewmodel.Main
{
    public class MainViewmodel : ViewmodelBase, IMainViewmodel
    {
        public void NotifyElementsReportReady(ElementListViewmodelType ViewmodelType, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            ElementListViewmodel elementListViewmodel = new ElementListViewmodel(ViewmodelType, _application, selectedConsumer, selectedProvider);
            ElementsReportReady?.Invoke(this, elementListViewmodel);
        }

        public void NotifyRelationsReportReady(RelationsListViewmodelType ViewmodelType, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            RelationListViewmodel Viewmodel = new RelationListViewmodel(ViewmodelType, _application, selectedConsumer, selectedProvider);
            RelationsReportReady?.Invoke(this, Viewmodel);
        }

        public event EventHandler<ElementEditViewmodel> ElementEditStarted;

        public event EventHandler<SnapshotMakeViewmodel> SnapshotMakeStarted;

        public event EventHandler<ElementListViewmodel> ElementsReportReady;
        public event EventHandler<RelationListViewmodel> RelationsReportReady;

        public event EventHandler<ActionListViewmodel> ActionsVisible;

        public event EventHandler<SettingsViewmodel> SettingsVisible;

        public event EventHandler ScreenshotRequested;

        private readonly IDsmApplication _application;
        private string _modelFilename;
        private string _title;
        private string _version;

        private bool _isModified;
        private bool _isLoaded;
        private readonly double _minZoom = 0.50;
        private readonly double _maxZoom = 2.00;
        private readonly double _zoomFactor = 1.25;

        private MatrixViewmodel _activeMatrix;

        private readonly ProgressViewmodel _progressViewmodel;
        private string _redoText;
        private string _undoText;
        private string _selectedSortAlgorithm;
        private IndicatorViewMode _selectedIndicatorViewMode;

        public MainViewmodel(IDsmApplication application)
        {
            _application = application;
            _application.Modified += OnModelModified;
            _application.ActionPerformed += OnActionPerformed;

            OpenFileCommand = new RelayCommand<object>(OpenFileExecute, OpenFileCanExecute);
            SaveFileCommand = new RelayCommand<object>(SaveFileExecute, SaveFileCanExecute);

            HomeCommand = new RelayCommand<object>(HomeExecute, HomeCanExecute);

            MoveUpElementCommand = new RelayCommand<object>(MoveUpElementExecute, MoveUpElementCanExecute);
            MoveDownElementCommand = new RelayCommand<object>(MoveDownElementExecute, MoveDownElementCanExecute);
            SortElementCommand = new RelayCommand<object>(SortElementExecute, SortElementCanExecute);

            ToggleElementBookmarkCommand = new RelayCommand<object>(ToggleElementBookmarkExecute, ToggleElementBookmarkCanExecute);

            ShowElementDetailMatrixCommand = new RelayCommand<object>(ShowElementDetailMatrixExecute, ShowElementDetailMatrixCanExecute);
            ShowElementContextMatrixCommand = new RelayCommand<object>(ShowElementContextMatrixExecute, ShowElementContextMatrixCanExecute);
            ShowCellDetailMatrixCommand = new RelayCommand<object>(ShowCellDetailMatrixExecute, ShowCellDetailMatrixCanExecute);

            ZoomInCommand = new RelayCommand<object>(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand<object>(ZoomOutExecute, ZoomOutCanExecute);
            ToggleElementExpandedCommand = new RelayCommand<object>(ToggleElementExpandedExecute, ToggleElementExpandedCanExecute);

            UndoCommand = new RelayCommand<object>(UndoExecute, UndoCanExecute);
            RedoCommand = new RelayCommand<object>(RedoExecute, RedoCanExecute);

            AddChildElementCommand = new RelayCommand<object>(AddChildElementExecute, AddChildElementCanExecute);
            AddSiblingElementAboveCommand = new RelayCommand<object>(AddSiblingElementAboveExecute, AddSiblingElementAboveCanExecute);
            AddSiblingElementBelowCommand = new RelayCommand<object>(AddSiblingElementBelowExecute, AddSiblingElementBelowCanExecute);
            ModifyElementCommand = new RelayCommand<object>(ModifyElementExecute, ModifyElementCanExecute);
            DeleteElementCommand = new RelayCommand<object>(DeleteElementExecute, DeleteElementCanExecute);
            ChangeElementParentCommand = new RelayCommand<object>(MoveElementExecute, MoveElementCanExecute);

            CopyElementCommand = new RelayCommand<object>(CopyElementExecute, CopyElementCanExecute);
            CutElementCommand = new RelayCommand<object>(CutElementExecute, CutElementCanExecute);
            PasteAsChildElementCommand = new RelayCommand<object>(PasteAsChildElementExecute, PasteAsChildElementCanExecute);
            PasteAsSiblingElementAboveCommand = new RelayCommand<object>(PasteAsSiblingElementAboveExecute, PasteAsSiblingElementAboveCanExecute);
            PasteAsSiblingElementBelowCommand = new RelayCommand<object>(PasteAsSiblingElementBelowExecute, PasteAsSiblingElementBelowCanExecute);

            MakeSnapshotCommand = new RelayCommand<object>(MakeSnapshotExecute, MakeSnapshotCanExecute);
            ShowHistoryCommand = new RelayCommand<object>(ShowHistoryExecute, ShowHistoryCanExecute);
            ShowSettingsCommand = new RelayCommand<object>(ShowSettingsExecute, ShowSettingsCanExecute);

            TakeScreenshotCommand = new RelayCommand<object>(TakeScreenshotExecute);

            _modelFilename = "";
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _title = $"DSM Viewer";

            _isModified = false;
            _isLoaded = false;

            _selectedSortAlgorithm = SupportedSortAlgorithms[0];

            _selectedIndicatorViewMode = IndicatorViewMode.Default;

            _progressViewmodel = new ProgressViewmodel();

            ActiveMatrix = new MatrixViewmodel(this, _application, new List<IDsmElement>());
            ElementSearchViewmodel = new ElementSearchViewmodel(_application, null, null, null, true);
            ElementSearchViewmodel.SearchUpdated += OnSearchUpdated;
        }

        private void OnSearchUpdated(object sender, EventArgs e)
        {
            SelectDefaultIndicatorMode();
            ActiveMatrix.Reload();
        }

        private void OnModelModified(object sender, bool e)
        {
            IsModified = e;
        }

        public IDsmElement SelectedConsumer => ActiveMatrix?.SelectedConsumer;

        public IDsmElement SelectedProvider => ActiveMatrix?.SelectedProvider;

        public ElementTreeItemViewmodel SelectedProviderTreeItem => ActiveMatrix?.SelectedTreeItem;

        public MatrixViewmodel ActiveMatrix
        {
            get { return _activeMatrix; }
            set { _activeMatrix = value; OnPropertyChanged(); }
        }

        public ElementSearchViewmodel ElementSearchViewmodel { get; }

        private bool _isMetricsViewExpanded;

        public bool IsMetricsViewExpanded
        {
            get { return _isMetricsViewExpanded; }
            set { _isMetricsViewExpanded = value; OnPropertyChanged(); }
        }

        public List<string> SupportedSortAlgorithms => _application.GetSupportedSortAlgorithms().ToList();

        public string SelectedSortAlgorithm
        {
            get { return _selectedSortAlgorithm; }
            set { _selectedSortAlgorithm = value; OnPropertyChanged(); }
        }

        public List<IndicatorViewMode> SupportedIndicatorViewModes => Enum.GetValues(typeof(IndicatorViewMode)).Cast<IndicatorViewMode>().ToList();

        public IndicatorViewMode SelectedIndicatorViewMode
        {
            get { return _selectedIndicatorViewMode; }
            set { _selectedIndicatorViewMode = value; OnPropertyChanged(); ActiveMatrix?.Reload(); }
        }

        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand HomeCommand { get; }

        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }

        public ICommand ToggleElementBookmarkCommand { get; }

        public ICommand SortElementCommand { get; }
        public ICommand ShowElementDetailMatrixCommand { get; }
        public ICommand ShowElementContextMatrixCommand { get; }
        public ICommand ShowCellDetailMatrixCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand AddChildElementCommand { get; }
        public ICommand AddSiblingElementAboveCommand { get; }
        public ICommand AddSiblingElementBelowCommand { get; }
        public ICommand ModifyElementCommand { get; }
        public ICommand DeleteElementCommand { get; }
        public ICommand ChangeElementParentCommand { get; }

        public ICommand CopyElementCommand { get; }
        public ICommand CutElementCommand { get; }
        public ICommand PasteAsChildElementCommand { get; }
        public ICommand PasteAsSiblingElementAboveCommand { get; }
        public ICommand PasteAsSiblingElementBelowCommand { get; }

        public ICommand MakeSnapshotCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand TakeScreenshotCommand { get; }

        public string ModelFilename
        {
            get { return _modelFilename; }
            set { _modelFilename = value; OnPropertyChanged(); }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set { _isModified = value; OnPropertyChanged(); }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; OnPropertyChanged(); }
        }

        public ProgressViewmodel ProgressViewmodel => _progressViewmodel;

        private async void OpenFileExecute(object parameter)
        {
            var progress = new Progress<ProgressInfo>(p =>
            {
                _progressViewmodel.Update(p);
            });

            _progressViewmodel.Action = "Reading";
            string filename = parameter as string;
            if (filename != null)
            {
                FileInfo fileInfo = new FileInfo(filename);

                switch (fileInfo.Extension)
                {
                    case ".dsm":
                        {
                            FileInfo dsmFileInfo = fileInfo;
                            await _application.OpenModel(dsmFileInfo.FullName, progress);
                            ModelFilename = dsmFileInfo.FullName;
                            Title = $"DSM Viewer - {dsmFileInfo.Name}";
                            IsLoaded = true;
                        }
                        break;
                    case ".dsi":
                        {
                            FileInfo dsiFileInfo = fileInfo;
                            FileInfo dsmFileInfo = new FileInfo(fileInfo.FullName.Replace(".dsi", ".dsm"));
                            await _application.AsyncImportDsiModel(dsiFileInfo.FullName, dsmFileInfo.FullName, false, true, progress);
                            ModelFilename = dsmFileInfo.FullName;
                            Title = $"DSM Viewer - {dsmFileInfo.Name}";
                            IsLoaded = true;
                        }
                        break;
                }
                ActiveMatrix = new MatrixViewmodel(this, _application, GetRootElements());
            }
        }

        private bool OpenFileCanExecute(object parameter)
        {
            string fileToOpen = parameter as string;
            if (fileToOpen != null)
            {
                return File.Exists(fileToOpen);
            }
            else
            {
                return false;
            }
        }

        private async void SaveFileExecute(object parameter)
        {
            var progress = new Progress<ProgressInfo>(p =>
            {
                _progressViewmodel.Update(p);
            });

            _progressViewmodel.Action = "Reading";
            await _application.SaveModel(ModelFilename, progress);
        }

        private bool SaveFileCanExecute(object parameter)
        {
            return _application.IsModified;
        }

        private void HomeExecute(object parameter)
        {
            IncludeAllInTree();
            ActiveMatrix.Reload();
        }

        private IEnumerable<IDsmElement> GetRootElements()
        {
            return new List<IDsmElement> { _application.RootElement };
        }

        private bool HomeCanExecute(object parameter)
        {
            return IsLoaded;
        }

        private void SortElementExecute(object parameter)
        {
            _application.Sort(SelectedProvider, SelectedSortAlgorithm);
        }

        private bool SortElementCanExecute(object parameter)
        {
            return _application.HasChildren(SelectedProvider);
        }

        private void ShowElementDetailMatrixExecute(object parameter)
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);
            ActiveMatrix.Reload();
        }

        private bool ShowElementDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementContextMatrixExecute(object parameter)
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);

            foreach (IDsmElement consumer in _application.GetElementConsumers(SelectedProvider))
            {
                IncludeInTree(consumer);
            }

            foreach (IDsmElement provider in _application.GetElementProviders(SelectedProvider))
            {
                IncludeInTree(provider);
            }

            ActiveMatrix.Reload();
        }

        private bool ShowElementContextMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellDetailMatrixExecute(object parameter)
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);
            IncludeInTree(SelectedConsumer);

            ActiveMatrix.Reload();
        }

        private bool ShowCellDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void MoveUpElementExecute(object parameter)
        {
            _application.MoveUp(SelectedProvider);
        }

        private bool MoveUpElementCanExecute(object parameter)
        {
            IDsmElement current = SelectedProvider;
            IDsmElement previous = _application.PreviousSibling(current);
            return (current != null) && (previous != null);
        }

        private void MoveDownElementExecute(object parameter)
        {
            _application.MoveDown(SelectedProvider);
        }

        private bool MoveDownElementCanExecute(object parameter)
        {
            IDsmElement current = SelectedProvider;
            IDsmElement next = _application.NextSibling(current);
            return (current != null) && (next != null);
        }

        private void ZoomInExecute(object parameter)
        {
            if (ActiveMatrix != null)
            {
                ActiveMatrix.ZoomLevel *= _zoomFactor;
            }
        }

        private bool ZoomInCanExecute(object parameter)
        {
            return ActiveMatrix?.ZoomLevel < _maxZoom;
        }

        private void ZoomOutExecute(object parameter)
        {
            if (ActiveMatrix != null)
            {
                ActiveMatrix.ZoomLevel /= _zoomFactor;
            }
        }

        private bool ZoomOutCanExecute(object parameter)
        {
            return ActiveMatrix?.ZoomLevel > _minZoom;
        }

        private void ToggleElementExpandedExecute(object parameter)
        {
            ActiveMatrix.SelectTreeItem(ActiveMatrix.HoveredTreeItem);
            if ((SelectedProviderTreeItem != null) && (SelectedProviderTreeItem.IsExpandable))
            {
                SelectedProviderTreeItem.IsExpanded = !SelectedProviderTreeItem.IsExpanded;
            }

            ActiveMatrix.Reload();
        }

        private bool ToggleElementExpandedCanExecute(object parameter)
        {
            return true;
        }

        public string UndoText
        {
            get { return _undoText; }
            set { _undoText = value; OnPropertyChanged(); }
        }

        private void UndoExecute(object parameter)
        {
            _application.Undo();
        }

        private bool UndoCanExecute(object parameter)
        {
            return _application.CanUndo();
        }

        private void RedoExecute(object parameter)
        {
            _application.Redo();
        }

        public string RedoText
        {
            get { return _redoText; }
            set { _redoText = value; OnPropertyChanged(); }
        }

        private bool RedoCanExecute(object parameter)
        {
            return _application.CanRedo();
        }

        private void SelectDefaultIndicatorMode()
        {
            SelectedIndicatorViewMode = string.IsNullOrEmpty(ElementSearchViewmodel.SearchText) ? IndicatorViewMode.Default : IndicatorViewMode.Search;
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            UndoText = $"Undo {_application.GetUndoActionDescription()}";
            RedoText = $"Redo {_application.GetRedoActionDescription()}";
            ActiveMatrix?.Reload();
        }

        private void AddChildElementExecute(object parameter)
        {
            ElementEditViewmodel elementEditViewmodel = new ElementEditViewmodel(ElementEditViewmodelType.AddChild, _application, SelectedProvider);
            ElementEditStarted?.Invoke(this, elementEditViewmodel);
        }

        private bool AddChildElementCanExecute(object parameter)
        {
            return true;
        }

        private void AddSiblingElementAboveExecute(object parameter)
        {
            ElementEditViewmodel elementEditViewmodel = new ElementEditViewmodel(ElementEditViewmodelType.AddSiblingAbove, _application, SelectedProvider);
            ElementEditStarted?.Invoke(this, elementEditViewmodel);
        }

        private bool AddSiblingElementAboveCanExecute(object parameter)
        {
            return true;
        }

        private void AddSiblingElementBelowExecute(object parameter)
        {
            ElementEditViewmodel elementEditViewmodel = new ElementEditViewmodel(ElementEditViewmodelType.AddSiblingBelow, _application, SelectedProvider);
            ElementEditStarted?.Invoke(this, elementEditViewmodel);
        }

        private bool AddSiblingElementBelowCanExecute(object parameter)
        {
            return true;
        }

        private void ModifyElementExecute(object parameter)
        {
            ElementEditViewmodel elementEditViewmodel = new ElementEditViewmodel(ElementEditViewmodelType.Modify, _application, SelectedProvider);
            ElementEditStarted?.Invoke(this, elementEditViewmodel);
        }

        private bool ModifyElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void DeleteElementExecute(object parameter)
        {
            _application.DeleteElement(SelectedProvider);
        }

        private bool DeleteElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void MoveElementExecute(object parameter)
        {
            Tuple<IDsmElement, IDsmElement, int> moveParameter = parameter as Tuple<IDsmElement, IDsmElement, int>;
            if (moveParameter != null)
            {
                _application.ChangeElementParent(moveParameter.Item1, moveParameter.Item2, moveParameter.Item3);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool MoveElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void CopyElementExecute(object parameter)
        {
            _application.CopyElement(SelectedProvider);
        }

        private bool CopyElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void CutElementExecute(object parameter)
        {
            _application.CutElement(SelectedProvider);
        }

        private bool CutElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void PasteAsChildElementExecute(object parameter)
        {
            _application.PasteElement(SelectedProvider.Parent, 0);
        }

        private bool PasteAsChildElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void PasteAsSiblingElementAboveExecute(object parameter)
        {
        }

        private bool PasteAsSiblingElementAboveCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }


        private void PasteAsSiblingElementBelowExecute(object parameter)
        {
        }

        private bool PasteAsSiblingElementBelowCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void ToggleElementBookmarkExecute(object parameter)
        {
            if (SelectedProvider != null)
            {
                SelectedProvider.IsBookmarked = !SelectedProvider.IsBookmarked;
                ActiveMatrix?.Reload();
            }
        }

        private bool ToggleElementBookmarkCanExecute(object parameter)
        {
            return _selectedIndicatorViewMode == IndicatorViewMode.Bookmarks;
        }

        private void MakeSnapshotExecute(object parameter)
        {
            SnapshotMakeViewmodel Viewmodel = new SnapshotMakeViewmodel(_application);
            SnapshotMakeStarted?.Invoke(this, Viewmodel);
        }

        private bool MakeSnapshotCanExecute(object parameter)
        {
            return true;
        }

        private void ShowHistoryExecute(object parameter)
        {
            ActionListViewmodel Viewmodel = new ActionListViewmodel(_application);
            ActionsVisible?.Invoke(this, Viewmodel);
        }

        private bool ShowHistoryCanExecute(object parameter)
        {
            return true;
        }

        private void ShowSettingsExecute(object parameter)
        {
            SettingsViewmodel Viewmodel = new SettingsViewmodel(_application);
            SettingsVisible?.Invoke(this, Viewmodel);
            ActiveMatrix?.Reload();
        }

        private bool ShowSettingsCanExecute(object parameter)
        {
            return true;
        }

        private void TakeScreenshotExecute(object parameter)
        {
            ScreenshotRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ExcludeAllFromTree()
        {
            UpdateChildrenIncludeInTree(_application.RootElement, false);
        }

        private void IncludeAllInTree()
        {
            UpdateChildrenIncludeInTree(_application.RootElement, true);
        }

        private void IncludeInTree(IDsmElement element)
        {
            UpdateChildrenIncludeInTree(element, true);
            UpdateParentsIncludeInTree(element, true);
        }

        private void UpdateChildrenIncludeInTree(IDsmElement element, bool included)
        {
            element.IsIncludedInTree = included;

            foreach (IDsmElement child in element.AllChildren)
            {
                UpdateChildrenIncludeInTree(child, included);
            }
        }

        private void UpdateParentsIncludeInTree(IDsmElement element, bool included)
        {
            IDsmElement current = element;
            do
            {
                current.IsIncludedInTree = included;
                current = current.Parent;
            } while (current != null);
        }
    }
}
