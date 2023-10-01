using System;
using System.Collections.Generic;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Common;

namespace Dsmviz.Viewmodel.Lists
{
    public class ElementListItemViewmodel : ViewmodelBase, IComparable
    {
        private IDsmElement _element;

        public ElementListItemViewmodel(IDsmElement element)
        {
            _element = element;
            ElementName = element.Name;
            ElementPath = element.Parent.Fullname;
            ElementType = element.Type;
            Properties = _element.Properties;
        }

        public int Index { get; set; }
        public string ElementPath { get; }
        public string ElementName { get; }
        public string ElementType { get; }
        public IDictionary<string, string> Properties { get; }

        public IEnumerable<string> DiscoveredElementPropertyNames()
        {
            return _element.DiscoveredElementPropertyNames();
        }

        public int CompareTo(object obj)
        {
            ElementListItemViewmodel other = obj as ElementListItemViewmodel;
            return string.Compare(ElementName, other?.ElementName, StringComparison.Ordinal);
        }
    }
}
