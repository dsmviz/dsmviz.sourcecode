using System.Collections.Generic;
using Dsmviz.Datamodel.Dsi.Interface;

namespace Dsmviz.Datamodel.Dsi.Persistency
{
    public interface IDsiRelationModelFileCallback
    {
        IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties);
        IEnumerable<IDsiRelation> GetRelations();
        int CurrentRelationCount { get; }
    }
}
