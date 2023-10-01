using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsm.Interfaces
{
    public interface IDsmAction
    {
        int Id { get; }

        string Type { get; }

        IReadOnlyDictionary<string,string> Data { get; }
    }
}
