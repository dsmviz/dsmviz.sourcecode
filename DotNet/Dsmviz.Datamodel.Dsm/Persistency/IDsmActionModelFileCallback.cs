using System.Collections.Generic;
using Dsmviz.Datamodel.Dsm.Interfaces;

namespace Dsmviz.Datamodel.Dsm.Persistency
{
    public interface IDsmActionModelFileCallback
    {
        IDsmAction ImportAction(int id, string type, IReadOnlyDictionary<string, string> data);
        IEnumerable<IDsmAction> GetExportedActions();
        int GetExportedActionCount();
    }
}
