using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsm.Persistency
{
    public interface IDsmElementModelFileCallback
    {
        IDsmElement FindElementById(int elementId);
        IDsmElement ImportElement(int id, string name, string type, IDictionary<string, string> properties, int order, bool expanded, int? parent, bool deleted);
        IDsmElement RootElement { get; }
        int GetExportedElementCount();
    }
}
