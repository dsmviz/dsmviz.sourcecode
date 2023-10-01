﻿using Dsmviz.Application.Actions.Base;
using Dsmviz.Application.Actions.Management;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dsmviz.Application.Actions.Element
{
    public class ElementCopyAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IActionContext _actionContext;
        private readonly IDsmElement _element;
        private IDsmElement _elementCopy;

        public const ActionType RegisteredType = ActionType.ElementCopy;

        public ElementCopyAction(object[] args)
        {
            if (args.Length == 3)
            {
                _model = args[0] as IDsmModel;
                _actionContext = args[1] as IActionContext;
                IReadOnlyDictionary<string, string> data = args[2] as IReadOnlyDictionary<string, string>;

                if ((_model != null) && (data != null))
                {
                    ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(_model, data);

                    _element = attributes.GetElement(nameof(_element));
                    _elementCopy = attributes.GetElement(nameof(_elementCopy));
                }
            }
        }

        public ElementCopyAction(IDsmModel model, IDsmElement element, IActionContext actionContext)
        {
            _model = model;
            _element = element;
            _actionContext = actionContext;
            _elementCopy = null;
        }

        public ActionType Type => RegisteredType;
        public string Title => "Copy element";
        public string Description => $"element={_element.Fullname}";

        public object Do()
        {
            _elementCopy = _model.AddElement(_element.Name, _element.Type, null, null, null);
            _actionContext.AddElementToClipboard(_elementCopy);
            return null;
        }

        public void Undo()
        {
            _actionContext.RemoveElementFromClipboard(_elementCopy);
        }

        public bool IsValid()
        {
            return (_model != null) &&
                   (_element != null);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetInt(nameof(_elementCopy), _elementCopy.Id);
                return attributes.Data;
            }
        }
    }
}
