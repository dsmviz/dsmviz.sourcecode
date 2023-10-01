using Dsmviz.Application.Interfaces;
using System.Collections.Generic;

namespace Dsmviz.Application.Actions.Management
{
    public interface IActionManager
    {
        bool Validate();
        void Clear();
        void Add(IAction action);
        object Execute(IAction action);
        IEnumerable<IAction> GetActionsInChronologicalOrder();
        IActionContext GetContext();
    }
}
