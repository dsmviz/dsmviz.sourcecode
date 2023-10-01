﻿using Dsmviz.Application.Actions.Base;
using Dsmviz.Application.Actions.Management;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;

namespace Dsmviz.Application.Actions.Element
{
    public class ElementChangeTypeAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IActionContext _actionContext;
        private readonly IDsmElement _element;
        private readonly string _old;
        private readonly string _new;

        public const ActionType RegisteredType = ActionType.ElementChangeType;

        public ElementChangeTypeAction(object[] args)
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
                    _old = attributes.GetString(nameof(_old));
                    _new = attributes.GetString(nameof(_new));
                }
            }
        }

        public ElementChangeTypeAction(IDsmModel model, IDsmElement element, string type)
        {
            _model = model;
            _element = element;
            _old = _element.Type;
            _new = type;
        }

        public ActionType Type => RegisteredType;
        public string Title => "Change element type";
        public string Description => $"element={_element.Fullname} type={_old}->{_new}";

        public object Do()
        {
            _model.ChangeElementType(_element, _new);
            return null;
        }

        public void Undo()
        {
            _model.ChangeElementType(_element, _old);
        }

        public bool IsValid()
        {
            return (_model != null) && 
                   (_element != null) && 
                   (_old != null) && 
                   (_new != null);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetString(nameof(_old), _old);
                attributes.SetString(nameof(_new), _new);
                return attributes.Data;
            }
        }
    }
}
