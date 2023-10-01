using System.Collections.Generic;
using Dsmviz.Datamodel.Common.Interface;

namespace Dsmviz.Datamodel.Common.Persistency
{
    public interface IMetaDataModelFileCallback
    {
        IMetaDataItem ImportMetaDataItem(string group, string name, string value);

        IEnumerable<string> GetExportedMetaDataGroups();
        IEnumerable<IMetaDataItem> GetExportedMetaDataGroupItems(string group);
    }
}
