using System.Collections.Generic;
using Dsmviz.Datamodel.Dsm.Interfaces;

namespace Dsmviz.Datamodel.Dsm.Persistency
{
    public interface IDsmRelationModelFileCallback
    {
        IDsmRelation ImportRelation(int id, IDsmElement consumer, IDsmElement provider, string type, int weight, IDictionary<string, string> properties, bool deleted);

        IEnumerable<IDsmRelation> GetExportedRelations();
        int GetExportedRelationCount();
    }
}
