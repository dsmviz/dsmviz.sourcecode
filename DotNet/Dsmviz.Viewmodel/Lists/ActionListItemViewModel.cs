using Dsmviz.Application.Interfaces;
using Dsmviz.Viewmodel.Common;

namespace Dsmviz.Viewmodel.Lists
{
    public class ActionListItemViewmodel : ViewmodelBase
    {
        public ActionListItemViewmodel(int index, IAction action)
        {
            Index = index;
            Action = action.Title;
            Details = action.Description;
        }

        public int Index { get; }
        public string Action { get; }
        public string Details { get; }
    }
}
