﻿using System;
using Dsmviz.Application.Actions.Element;
using Dsmviz.Application.Actions.Relation;
using Dsmviz.Application.Actions.Snapshot;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using System.Collections.Generic;

namespace Dsmviz.Application.Actions.Management
{
    public class ActionStore
    {
        private readonly IDsmModel _model;
        private readonly IActionManager _actionManager;
        private readonly Dictionary<ActionType, Type> _types;

        public ActionStore(IDsmModel model, IActionManager actionManager)
        {
            _model = model;
            _actionManager = actionManager;
            _types = new Dictionary<ActionType, Type>();

            RegisterActionTypes();
        }

        public void LoadFromModel()
        {
            foreach (IDsmAction action in _model.GetActions())
            {
                ActionType actionType;
                if (ActionType.TryParse(action.Type, out actionType))
                {
                    if (_types.ContainsKey(actionType))
                    {
                        Type type = _types[actionType];
                        object[] args = { _model, _actionManager.GetContext(), action.Data };
                        IAction instance = Activator.CreateInstance(type, args) as IAction;
 
                        if (instance != null)
                        {
                            _actionManager.Add(instance);
                        }
                    }
                }
            }

            if (!_actionManager.Validate())
            {
                _actionManager.Clear();
            }
        }

        public void SaveToModel()
        {
            if (_actionManager.Validate())
            {
                foreach (IAction action in _actionManager.GetActionsInChronologicalOrder())
                {
                    _model.AddAction(action.Type.ToString(), action.Data);
                }
            }
        }

        private void RegisterActionTypes()
        {
            _types[ElementChangeNameAction.RegisteredType] = typeof(ElementChangeNameAction);
            _types[ElementChangeTypeAction.RegisteredType] = typeof(ElementChangeTypeAction);
            _types[ElementChangeParentAction.RegisteredType] = typeof(ElementChangeParentAction);
            _types[ElementCreateAction.RegisteredType] = typeof(ElementCreateAction);
            _types[ElementDeleteAction.RegisteredType] = typeof(ElementDeleteAction);
            _types[ElementMoveDownAction.RegisteredType] = typeof(ElementMoveDownAction);
            _types[ElementMoveUpAction.RegisteredType] = typeof(ElementMoveUpAction);
            _types[ElementSortAction.RegisteredType] = typeof(ElementSortAction);

            _types[RelationChangeTypeAction.RegisteredType] = typeof(RelationChangeTypeAction);
            _types[RelationChangeWeightAction.RegisteredType] = typeof(RelationChangeWeightAction);
            _types[RelationCreateAction.RegisteredType] = typeof(RelationCreateAction);
            _types[RelationDeleteAction.RegisteredType] = typeof(RelationDeleteAction);

            _types[MakeSnapshotAction.RegisteredType] = typeof(MakeSnapshotAction);
        }
    }
}
