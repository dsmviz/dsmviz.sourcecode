﻿using Dsmviz.Datamodel.Common.Interface;
using Dsmviz.Util;
using System;
using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsm.Interfaces
{
    public interface IDsmModel
    {
        // Cleanup
        void Clear();

        // Model persistency
        string ModelFilename { get; }
        bool IsCompressed { get; }
        void LoadModel(string dsmFilename, IProgress<ProgressInfo> progress);
        void SaveModel(string dsmFilename, bool compressFile, IProgress<ProgressInfo> progress);

        // Meta data
        IMetaDataItem AddMetaData(string group, string name, string value);

        // Element editing
        IDsmElement AddElement(string name, string type, int? parentId, int? index, IDictionary<string, string> properties);
        void RemoveElement(int elementId);
        void UnremoveElement(int elementId);
        void ChangeElementName(IDsmElement element, string name);
        void ChangeElementType(IDsmElement element, string type);
        bool IsChangeElementParentAllowed(IDsmElement element, IDsmElement parent);
        void ChangeElementParent(IDsmElement element, IDsmElement parent, int index);
        void ReorderChildren(IDsmElement element, ISortResult sortResult);
        IDsmElement NextSibling(IDsmElement element);
        IDsmElement PreviousSibling(IDsmElement element);
        bool Swap(IDsmElement first, IDsmElement second);
        void AssignElementOrder();

        // Element queries
        IDsmElement GetElementById(int id);
        IDsmElement GetDeletedElementById(int id);
        IDsmElement GetElementByFullname(string fullname);
        IList<IDsmElement> SearchElements(string searchText, IDsmElement searchInElement, bool caseSensitive, string elementTypeFilter, bool markMatchingElements);
        IDsmElement RootElement { get; }
        IEnumerable<string> GetElementTypes();
        IEnumerable<IDsmElement> GetElements();
        int GetElementCount();

        // Relation editing
        IDsmRelation AddRelation(IDsmElement consumer, IDsmElement provider, string type, int weight, IDictionary<string, string> properties);
        void ChangeRelationType(IDsmRelation relation, string type);
        void ChangeRelationWeight(IDsmRelation relation, int weight);
        void RemoveRelation(int relationId);
        void UnremoveRelation(int relationId);
        IEnumerable<string> GetRelationTypes();

        // Relation queries
        int GetDependencyWeight(IDsmElement consumer, IDsmElement provider);
        int GetDirectDependencyWeight(IDsmElement consumer, IDsmElement provider);
        CycleType IsCyclicDependency(IDsmElement consumer, IDsmElement provider);
        IDsmRelation GetRelationById(int relationId);
        IDsmRelation GetDeletedRelationById(int relationId);
        IEnumerable<IDsmRelation> GetRelations();
        IDsmRelation FindRelation(IDsmElement consumer, IDsmElement provider, string type);
        IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider);
        int GetRelationCount(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmRelation> FindIngoingRelations(IDsmElement element);
        IEnumerable<IDsmRelation> FindOutgoingRelations(IDsmElement element);
        IEnumerable<IDsmRelation> FindInternalRelations(IDsmElement element);
        IEnumerable<IDsmRelation> FindExternalRelations(IDsmElement element);
        int GetHierarchicalCycleCount(IDsmElement element);
        int GetSystemCycleCount(IDsmElement element);
        // Actions
        IDsmAction AddAction(string type, IReadOnlyDictionary<string, string> data);
        void ClearActions();
        IEnumerable<IDsmAction> GetActions();
    }
}
