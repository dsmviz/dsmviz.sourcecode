using Dsmviz.Viewmodel.Common;

namespace Dsmviz.Viewmodel.Matrix
{
    public class LegendViewmodel : ViewmodelBase
    {
        public LegendViewmodel(LegendColor color, string description)
        {
            Color = color;
            Description = description;
        }

        public LegendColor Color { get; }
        public string Description { get; }
    }
}
