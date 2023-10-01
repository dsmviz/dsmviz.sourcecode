﻿using Dsmviz.Application.Actions.Base;
using Dsmviz.Application.Actions.Management;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Dsmviz.Application.Actions.Element
{
    public class ElementPasteAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IActionContext _actionContext;
        private readonly IDsmElement _element;
        private readonly IDsmElement _oldParent;
        private readonly int _oldIndex;
        private readonly string _oldName;
        private readonly IDsmElement _newParent;
        private readonly int _newIndex;
        private string _newName;

        public const ActionType RegisteredType = ActionType.ElementPaste;

        public ElementPasteAction(object[] args)
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
                    _oldParent = attributes.GetElement(nameof(_oldParent));
                    _oldIndex = attributes.GetInt(nameof(_oldIndex));
                    _oldName = attributes.GetString(nameof(_oldName));
                    _newParent = attributes.GetElement(nameof(_newParent));
                    _newIndex = attributes.GetInt(nameof(_newIndex));
                    _newName = attributes.GetString(nameof(_newName));
                }
            }
        }

        public ElementPasteAction(IDsmModel model, IDsmElement newParent, int index, IActionContext actionContext)
        {
            _model = model;

            _actionContext = actionContext;
            _element = _actionContext.GetElementOnClipboard();

            _oldParent = _element.Parent;
            _oldIndex = _oldParent.IndexOfChild(_element);
            _oldName = _element.Name;

            _newParent = newParent;
            _newIndex = index;
            _newName = _element.Name;
        }

        public ActionType Type => RegisteredType;
        public string Title => "Paste element";
        public string Description => $"element={_actionContext.GetElementOnClipboard().Fullname}";

        public object Do()
        {
            // Rename to avoid duplicate names
            if (_newParent.ContainsChildWithName(_oldName))
            {
                _newName += " (duplicate)";
                _model.ChangeElementName(_element, _newName);
            }

            _model.ChangeElementParent(_element, _newParent, _newIndex);
            _model.AssignElementOrder();

            _actionContext.RemoveElementFromClipboard(_element);
            return null;
        }

        public void Undo()
        {
            _model.ChangeElementParent(_element, _oldParent, _oldIndex);
            _model.AssignElementOrder();

            // Restore original name
            if (_oldName != _newName)
            {
                _model.ChangeElementName(_element, _oldName);
            }
        }

        public bool IsValid()
        {
            return (_model != null) &&
                   (_element != null) &&
                   (_actionContext != null) &&
                   (_actionContext.IsElementOnClipboard()) &&
                   (_oldParent != null) &&
                   (_oldName != null) &&
                   (_newParent != null) &&
                   (_newName != null);
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
