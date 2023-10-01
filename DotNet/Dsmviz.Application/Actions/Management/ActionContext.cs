using Dsmviz.Datamodel.Dsm.Interfaces;

namespace Dsmviz.Application.Actions.Management
{
    public class ActionContext : IActionContext
    {
        private IDsmElement _element;

        public void AddElementToClipboard(IDsmElement element)
        {
            _element = element;
        }

        public void RemoveElementFromClipboard(IDsmElement element)
        {
            _element = null;
        }

        public IDsmElement GetElementOnClipboard()
        {
            return _element;
        }

        public bool IsElementOnClipboard()
        {
            return (_element == null);
        }
    }
}
