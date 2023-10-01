using System.Collections.Generic;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Common;

namespace Dsmviz.Viewmodel.Matrix
{
    public class CellToolTipViewmodel : ViewmodelBase
    {
        public CellToolTipViewmodel(IDsmElement consumer, IDsmElement provider, int weight, CycleType cycleType)
        {
            Title = $"Relation {consumer.Name} - {provider.Name}";
            ConsumerId = consumer.Id;
            ConsumerName = consumer.Fullname;
            ProviderId = provider.Id;
            ProviderName = provider.Fullname; 
            Weight = weight;
            CycleType = cycleType.ToString();

            Legend = new List<LegendViewmodel>();
            Legend.Add(new LegendViewmodel(LegendColor.Cycle, "cycle"));
        }

        public string Title { get; }
        public int ConsumerId { get; }
        public string ConsumerName { get; }
        public int ProviderId { get; }
        public string ProviderName { get; }
        public int Weight { get; }
        public string CycleType { get; }

        public List<LegendViewmodel> Legend { get; }
    }
}
