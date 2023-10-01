using System;
using System.Collections.Generic;
using Dsmviz.Datamodel.Common.Interface;
using Dsmviz.Util;
using Dsmviz.Datamodel.Dsm.Interfaces;

namespace Dsmviz.Application.Import.Common
{
    public interface IDsmBuilder
    {
        IMetaDataItem ImportMetaDataItem(string group, string name, string value);
        IDsmElement ImportElement(string fullname, string name, string type, IDsmElement parent, IDictionary<string, string> properties);
        IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties);
        void FinalizeImport(IProgress<ProgressInfo> progress);
    }
}
