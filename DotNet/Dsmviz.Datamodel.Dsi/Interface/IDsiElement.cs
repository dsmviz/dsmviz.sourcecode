using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsi.Interface
{
    public interface IDsiElement
    {
        int Id { get; }
        string Name { get; }
        string Type { get; }
        IDictionary<string,string> Properties { get; }
    }
}
