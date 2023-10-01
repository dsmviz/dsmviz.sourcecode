using Dsmviz.Datamodel.Dsm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsmviz.Application.Actions.Management
{
    public interface IActionContext
    {
        void AddElementToClipboard(IDsmElement element);
        void RemoveElementFromClipboard(IDsmElement element);
        IDsmElement GetElementOnClipboard();
        bool IsElementOnClipboard();
    }
}
