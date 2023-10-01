﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dsmviz.Application.Actions.Management;
using Dsmviz.Application.Actions.Element;
using Dsmviz.Application.Actions.Relation;
using Dsmviz.Application.Actions.Snapshot;
using Dsmviz.Application.Interfaces;
using Dsmviz.Application.Queries;
using Dsmviz.Application.Sorting;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Datamodel.Dsi.Core;
using System.Reflection;
using Dsmviz.Util;
using Dsmviz.Application.Import.Common;
using Dsmviz.Application.Import.Dsi;
using Dsmviz.Application.Metrics;

namespace Dsmviz.Application.Core
{
    public class DsmApplication : IDsmApplication
    {
        private readonly IDsmModel _dsmModel;
        private readonly ActionManager _actionManager;
        private readonly ActionStore _actionStore;
        private readonly DsmQueries _queries;
        private readonly DsmMetrics _metrics;

        public event EventHandler<bool> Modified;
        public event EventHandler ActionPerformed;

        public DsmApplication(IDsmModel dsmModel)
        {
            _dsmModel = dsmModel;

            _actionManager = new ActionManager();
            _actionManager.ActionPerformed += OnActionPerformed;

            _actionStore = new ActionStore(_dsmModel, _actionManager);

            _queries = new DsmQueries(dsmModel);

            _metrics = new DsmMetrics();
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            ActionPerformed?.Invoke(this, e);
            IsModified = true;
            Modified?.Invoke(this, IsModified);
        }

        public bool CanUndo()
        {
            return _actionManager.CanUndo();
        }

        public string GetUndoActionDescription()
        {
            return _actionManager.GetCurrentUndoAction()?.Description;
        }

        public void Undo()
        {
            _actionManager.Undo();
        }

        public bool CanRedo()
        {
            return _actionManager.CanRedo();
        }

        public string GetRedoActionDescription()
        {
            return _actionManager.GetCurrentRedoAction()?.Description;
        }

        public void Redo()
        {
            _actionManager.Redo();
        }

        public async Task AsyncImportDsiModel(string dsiFilename, string dsmFilename, bool autoPartition, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            await Task.Run(() => ImportDsiModel(dsiFilename, dsmFilename, autoPartition, compressDsmFile, progress));
            _actionStore.LoadFromModel();
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public void ImportDsiModel(string dsiFilename, string dsmFilename, bool autoPartition, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            string processStep = "Builder";
            Assembly assembly = Assembly.GetEntryAssembly();
            DsiModel dsiModel = new DsiModel(processStep, new List<string>(), assembly);
            dsiModel.Load(dsiFilename, progress);

            IDsmBuilder importPolicy = new DsmBuilder(_dsmModel);
            DsiImporter importer = new DsiImporter(dsiModel, _dsmModel, importPolicy, autoPartition);
            importer.Import(progress);
            _actionStore.SaveToModel();
            _dsmModel.SaveModel(dsmFilename, compressDsmFile, progress);
        }

        public async Task OpenModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            await Task.Run(() => _dsmModel.LoadModel(dsmFilename, progress));
            _actionStore.LoadFromModel();
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public async Task SaveModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            _actionStore.SaveToModel();
            await Task.Run(() => _dsmModel.SaveModel(dsmFilename, _dsmModel.IsCompressed, progress));
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public IDsmElement RootElement => _dsmModel.RootElement;

        public bool IsModified { get; private set; }

        public IEnumerable<IDsmElement> GetElementConsumers(IDsmElement element)
        {
            return _queries.GetElementConsumers(element);
        }

        public IEnumerable<IDsmElement> GetElementProvidedElements(IDsmElement element)
        {
            return _queries.GetElementProvidedElements(element);
        }

        public IEnumerable<IDsmElement> GetElementProviders(IDsmElement element)
        {
            return _queries.GetElementProviders(element);
        }

        public IEnumerable<IDsmRelation> FindResolvedRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.FindRelations(consumer, provider);
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.FindRelations(consumer, provider);
        }

        public int GetRelationCount(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.GetRelationCount(consumer, provider);
        }

        public IEnumerable<IDsmRelation> FindIngoingRelations(IDsmElement element)
        {
            return _queries.FindIngoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindOutgoingRelations(IDsmElement element)
        {
            return _queries.FindOutgoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindInternalRelations(IDsmElement element)
        {
            return _queries.FindInternalRelations(element);
        }

        public IEnumerable<IDsmRelation> FindExternalRelations(IDsmElement element)
        {
            return _queries.FindExternalRelations(element);
        }

        public IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationProviders(consumer, provider);
        }

        public IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationConsumers(consumer, provider);
        }

        public int GetHierarchicalCycleCount(IDsmElement element)
        {
            return _dsmModel.GetHierarchicalCycleCount(element);
        }

        public int GetSystemCycleCount(IDsmElement element)
        {
            return _dsmModel.GetSystemCycleCount(element);
        }

        public IDsmElement NextSibling(IDsmElement element)
        {
            return _dsmModel.NextSibling(element);
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            return _dsmModel.PreviousSibling(element);
        }

        public bool IsFirstChild(IDsmElement element)
        {
            return _dsmModel.PreviousSibling(element) == null;
        }

        public bool IsLastChild(IDsmElement element)
        {
            return _dsmModel.NextSibling(element) == null;
        }

        public bool HasChildren(IDsmElement element)
        {
            return element?.Children.Count > 0;
        }

        public void Sort(IDsmElement element, string algorithm)
        {
            ElementSortAction action = new ElementSortAction(_dsmModel, element, algorithm);
            _actionManager.Execute(action);
        }

        public IEnumerable<string> GetSupportedSortAlgorithms()
        {
            return SortAlgorithmFactory.GetSupportedAlgorithms();
        }

        public void MoveUp(IDsmElement element)
        {
            ElementMoveUpAction action = new ElementMoveUpAction(_dsmModel, element);
            _actionManager.Execute(action);
        }

        public void MoveDown(IDsmElement element)
        {
            ElementMoveDownAction action = new ElementMoveDownAction(_dsmModel, element);
            _actionManager.Execute(action);
        }
        
        public IEnumerable<string> GetElementTypes()
        {
            return _dsmModel.GetElementTypes();
        }

        public int GetDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.GetDependencyWeight(consumer, provider);
        }

        public int GetDirectDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.GetDirectDependencyWeight(consumer, provider);
        }

        public CycleType IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.IsCyclicDependency(consumer, provider);
        }

        public IList<IDsmElement> SearchElements(string searchText, IDsmElement searchInElement, bool caseSensitive, string elementTypeFilter, bool markMatchingElements)
        {
            return _dsmModel.SearchElements(searchText, searchInElement, caseSensitive, elementTypeFilter, markMatchingElements);
        }

        public IDsmElement GetElementByFullname(string text)
        {
            return _dsmModel.GetElementByFullname(text);
        }

        public IDsmElement CreateElement(string name, string type, IDsmElement parent, int index)
        {
            ElementCreateAction action = new ElementCreateAction(_dsmModel, name, type, parent, index);
            return _actionManager.Execute(action) as IDsmElement;
        }

        public void DeleteElement(IDsmElement element)
        {
            ElementDeleteAction action = new ElementDeleteAction(_dsmModel, element);
            _actionManager.Execute(action);
        }

        public void ChangeElementName(IDsmElement element, string name)
        {
            ElementChangeNameAction action = new ElementChangeNameAction(_dsmModel, element, name);
            _actionManager.Execute(action);
        }

        public void ChangeElementType(IDsmElement element, string type)
        {
            ElementChangeTypeAction action = new ElementChangeTypeAction(_dsmModel, element, type);
            _actionManager.Execute(action);
        }

        public void ChangeElementParent(IDsmElement element, IDsmElement newParent, int index)
        {
            if (_dsmModel.IsChangeElementParentAllowed(element, newParent))
            {
                ElementChangeParentAction action = new ElementChangeParentAction(_dsmModel, element, newParent, index);
                _actionManager.Execute(action);
            }
        }

        public void CutElement(IDsmElement element)
        {
            ElementCutAction action = new ElementCutAction(_dsmModel, element, _actionManager.GetContext());
            _actionManager.Execute(action);
        }

        public void CopyElement(IDsmElement element)
        {
            ElementCopyAction action = new ElementCopyAction(_dsmModel, element, _actionManager.GetContext());
            _actionManager.Execute(action);
        }

        public void PasteElement(IDsmElement newParent, int index)
        {
            ElementPasteAction action = new ElementPasteAction(_dsmModel, newParent, index, _actionManager.GetContext());
            _actionManager.Execute(action);
        }


        public IDsmRelation CreateRelation(IDsmElement consumer, IDsmElement provider, string type, int weight)
        {
            RelationCreateAction action = new RelationCreateAction(_dsmModel, consumer.Id, provider.Id, type, weight);
            return _actionManager.Execute(action) as IDsmRelation;
        }

        public void DeleteRelation(IDsmRelation relation)
        {
            RelationDeleteAction action = new RelationDeleteAction(_dsmModel, relation);
            _actionManager.Execute(action);
        }

        public void ChangeRelationType(IDsmRelation relation, string type)
        {
            RelationChangeTypeAction action = new RelationChangeTypeAction(_dsmModel, relation, type);
            _actionManager.Execute(action);
        }

        public void ChangeRelationWeight(IDsmRelation relation, int weight)
        {
            RelationChangeWeightAction action = new RelationChangeWeightAction(_dsmModel, relation, weight);
            _actionManager.Execute(action);
        }

        public IEnumerable<string> GetRelationTypes()
        {
            return _dsmModel.GetRelationTypes();
        }

        public void MakeSnapshot(string description)
        {
            MakeSnapshotAction action = new MakeSnapshotAction(_dsmModel, description);
            _actionManager.Execute(action);
        }

        public IEnumerable<IAction> GetActions()
        {
            return _actionManager.GetActionsInReverseChronologicalOrder();
        }

        public void ClearActions()
        {
            _actionManager.Clear();
            _dsmModel.ClearActions();
            IsModified = true;
            Modified?.Invoke(this, IsModified);
        }

        public int GetElementSize(IDsmElement element)
        {
            return _metrics.GetElementSize(element);
        }

        public int GetElementCount()
        {
            return _dsmModel.GetElementCount();
        }
    }
}
