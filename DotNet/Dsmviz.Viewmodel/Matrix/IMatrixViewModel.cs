using System.Windows.Input;

namespace Dsmviz.Viewmodel.Matrix
{
    public interface IMatrixViewmodel
    {
        ICommand ToggleElementExpandedCommand { get; }
        ICommand SortElementCommand { get; }
        ICommand MoveUpElementCommand { get; }
        ICommand MoveDownElementCommand { get; }

        ICommand ToggleElementBookmarkCommand { get; }

        ICommand ChangeElementParentCommand { get; }
    }
}
