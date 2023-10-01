using System.Collections.Generic;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Common;

namespace Dsmviz.Viewmodel.Matrix
{
    public class ElementToolTipViewmodel : ViewmodelBase
    {
        public ElementToolTipViewmodel(IDsmElement element, IDsmApplication application)
        {
            Title = $"Element {element.Name}";
            Id = element.Id;
            Name = element.Fullname;
            Type = element.Type;

            Legend = new List<LegendViewmodel>();
            Legend.Add(new LegendViewmodel(LegendColor.Consumer, "Consumer"));
            Legend.Add(new LegendViewmodel(LegendColor.Provider, "Provider"));
            Legend.Add(new LegendViewmodel(LegendColor.Cycle, "Cycle"));
            Legend.Add(new LegendViewmodel(LegendColor.Search, "Search"));
            Legend.Add(new LegendViewmodel(LegendColor.Bookmark, "Bookmark"));
        }

        public string Title { get; }
        public int Id { get; }
        public string Name { get; }
        public string Type { get; }
        public List<LegendViewmodel> Legend { get; }
    }
}
