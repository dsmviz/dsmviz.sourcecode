using Dsmviz.Datamodel.Dsm.Interfaces;

namespace Dsmviz.Application.Metrics
{
    public class DsmMetrics
    {
        public int GetElementSize(IDsmElement element)
        {
            int count = 0;
            CountChildren(element, ref count);
            return count;
        }

        private void CountChildren(IDsmElement element, ref int count)
        {
            count++;

            foreach (IDsmElement child in element.Children)
            {
                CountChildren(child, ref count);
            }
        }
    }
}
