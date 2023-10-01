using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Lists;

namespace Dsmviz.Viewmodel.Main
{
    public interface IMainViewmodel : INotifyPropertyChanged
    {
        void NotifyElementsReportReady(ElementListViewmodelType ViewmodelType, IDsmElement selectedConsumer, IDsmElement selectedProvider);
        void NotifyRelationsReportReady(RelationsListViewmodelType ViewmodelType, IDsmElement selectedConsumer, IDsmElement selectedProvider);

        ICommand ToggleElementExpandedCommand { get; }
        ICommand MoveUpElementCommand { get; }
        ICommand MoveDownElementCommand { get; }

        ICommand ToggleElementBookmarkCommand { get; }

        ICommand SortElementCommand { get; }
        ICommand ShowElementDetailMatrixCommand { get; }
        ICommand ShowElementContextMatrixCommand { get; }
        ICommand ShowCellDetailMatrixCommand { get; }

        ICommand AddChildElementCommand { get; }
        ICommand AddSiblingElementAboveCommand { get; }
        ICommand AddSiblingElementBelowCommand { get; }
        ICommand ModifyElementCommand { get; }
        ICommand DeleteElementCommand { get; }
        ICommand ChangeElementParentCommand { get; }

        IndicatorViewMode SelectedIndicatorViewMode { get; }
    }
}
