using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsm.Core
{
    public class DsmAction : IDsmAction
    {
        public DsmAction(int id, string type, IReadOnlyDictionary<string, string> data)
        {
            Id = id;
            Type = type;
            Data = data;
        }

        public int Id { get; }

        public string Type { get; }

        public IReadOnlyDictionary<string, string> Data { get; }
    }
}
