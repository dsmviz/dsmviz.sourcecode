using Dsmviz.Datamodel.Common.Interface;

namespace Dsmviz.Datamodel.Common.Core
{
    public class MetaDataItem : IMetaDataItem
    {
        public MetaDataItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; set; }
    }
}
