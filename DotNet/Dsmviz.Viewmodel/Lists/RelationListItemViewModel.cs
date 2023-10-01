using System;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Common;
using Dsmviz.Application.Interfaces;
using System.Collections.Generic;

namespace Dsmviz.Viewmodel.Lists
{
    public class RelationListItemViewmodel : ViewmodelBase, IComparable
    {
        public RelationListItemViewmodel(IDsmApplication application, IDsmRelation relation)
        {
            Relation = relation;

            ConsumerPath = relation.Consumer.Parent.Fullname;
            ConsumerName = relation.Consumer.Name;
            ConsumerType = relation.Consumer.Type;
            ProviderPath = relation.Provider.Parent.Fullname;
            ProviderName = relation.Provider.Name;
            ProviderType = relation.Provider.Type;
            RelationType = relation.Type;
            RelationWeight = relation.Weight;
            Properties = relation.Properties;

            CycleType cycleType = application.IsCyclicDependency(relation.Consumer, relation.Provider);
            switch (cycleType)
            {
                case CycleType.Hierarchical:
                    Cyclic = "Hierarchical";
                    break;
                case CycleType.System:
                    Cyclic = "System";
                    break;
                case CycleType.None:
                default:
                    Cyclic = "";
                    break;
            }
        }

        public IDsmRelation Relation { get; set; }

        public int Index { get; set; }

        public string ConsumerPath { get; }
        public string ConsumerName { get; }
        public string ConsumerType { get; }
        public string ProviderPath { get; }
        public string ProviderName { get; }
        public string ProviderType { get; }
        public string RelationType { get; }
        public int RelationWeight { get; }
        public string Cyclic { get; }
        public IDictionary<string, string> Properties { get; }

        public IEnumerable<string> DiscoveredRelationPropertyNames()
        {
            return Relation.DiscoveredRelationPropertyNames();
        }

        public int CompareTo(object obj)
        {
            RelationListItemViewmodel other = obj as RelationListItemViewmodel;

            int compareConsumer = string.Compare(ConsumerName, other?.ConsumerName, StringComparison.Ordinal);
            int compareProvider = string.Compare(ProviderName, other?.ProviderName, StringComparison.Ordinal);

            if (compareConsumer != 0)
            {
                return compareConsumer;
            }
            else
            {
                return compareProvider;
            }
        }
    }
}
