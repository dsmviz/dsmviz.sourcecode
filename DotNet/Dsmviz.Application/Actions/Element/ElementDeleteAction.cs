﻿using Dsmviz.Application.Actions.Base;
using Dsmviz.Application.Actions.Management;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;

namespace Dsmviz.Application.Actions.Element
{
    public class ElementDeleteAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IActionContext _actionContext;
        private readonly IDsmElement _element;

        public const ActionType RegisteredType = ActionType.ElementDelete;

        public ElementDeleteAction(object[] args)
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
                }
            }
        }

        public ElementDeleteAction(IDsmModel model, IDsmElement element)
        {
            _model = model;
            _element = element;
        }

        public ActionType Type => RegisteredType;
        public string Title => "Delete element";
        public string Description => $"element={_element.Fullname}";

        public object Do()
        {
            _model.RemoveElement(_element.Id);
            _model.AssignElementOrder();
            return null;
        }

        public void Undo()
        {
            _model.UnremoveElement(_element.Id);
            _model.AssignElementOrder();
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
                return attributes.Data;
            }
        }
    }
}
