using System.Collections.Generic;
using Dsmviz.Datamodel.Dsi.Interface;

namespace Dsmviz.Datamodel.Dsi.Persistency
{
    public interface IDsiElementModelFileCallback
    {
        IDsiElement ImportElement(int id, string name, string type, IDictionary<string, string> properties);
        IEnumerable<IDsiElement> GetElements();
        int CurrentElementCount { get; }
    }
}
