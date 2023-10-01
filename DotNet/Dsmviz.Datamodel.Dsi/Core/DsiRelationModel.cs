﻿using System.Linq;
using System.Collections.Generic;
using Dsmviz.Datamodel.Dsi.Interface;
using Dsmviz.Datamodel.Dsi.Persistency;
using Dsmviz.Util;
using System.Collections.ObjectModel;

namespace Dsmviz.Datamodel.Dsi.Core
{
    public class DsiRelationModel : IDsiRelationModelFileCallback
    {
        private readonly DsiElementModel _elementsDataModel;
        private readonly Dictionary<int, Dictionary<int, Dictionary<string, List<DsiRelation>>>> _relationsByConsumerId;
        private readonly Dictionary<string, int> _relationTypeCount;
        private int _importedRelationCount;
        private int _resolvedRelationCount;
        private int _ambiguousRelationCount;

        public DsiRelationModel(DsiElementModel elementsDataModel)
        {
            _elementsDataModel = elementsDataModel;
            _elementsDataModel.ElementRemoved += OnElementRemoved;
            _relationsByConsumerId = new Dictionary<int, Dictionary<int, Dictionary<string, List<DsiRelation>>>>();
            _relationTypeCount = new Dictionary<string, int>();
        }
        
        public void Clear()
        {
            _relationsByConsumerId.Clear();
            _relationTypeCount.Clear();
            _importedRelationCount = 0;
            _resolvedRelationCount = 0;
            _ambiguousRelationCount = 0;
        }

        public IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties)
        {
            Logger.LogDataModelMessage($"Import relation consumerId={consumerId} providerId={providerId} type={type} weight={weight}");

            _importedRelationCount++;

            DsiRelation relation = null;

            IDsiElement consumer = _elementsDataModel.FindElementById(consumerId);
            IDsiElement provider = _elementsDataModel.FindElementById(providerId);

            if ((consumer != null) && (provider != null))
            {
                _resolvedRelationCount++;
                relation = AddOrUpdateRelation(consumer.Id, provider.Id, type, weight, properties);
            }
            else
            {
                Logger.LogErrorDataModelRelationNotResolved(consumerId.ToString(), providerId.ToString());
            }
            return relation;
        }
        
        public IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, IDictionary<string, string> properties)
        {
            Logger.LogDataModelMessage($"Add relation consumerName={consumerName} providerName={providerName} type={type} weight={weight}");

            DsiRelation relation = null;

            _importedRelationCount++;

            IDsiElement consumer = _elementsDataModel.FindElementByName(consumerName);
            IDsiElement provider = _elementsDataModel.FindElementByName(providerName);

            if ((consumer != null) && (provider != null))
            {
                _resolvedRelationCount++;
                relation = AddOrUpdateRelation(consumer.Id, provider.Id, type, weight, properties);
            }
            else
            {
                Logger.LogErrorDataModelRelationNotResolved(consumerName, providerName);
            }

            return relation;
        }

        public void IgnoreRelation(string consumerName, string providerName, string type, int weight)
        {
            Logger.LogDataModelMessage($"Ignore relation consumerName={consumerName} providerName={providerName} type={type} weight={weight}");
        }

        private DsiRelation AddOrUpdateRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties)
        {
            Dictionary<string,List<DsiRelation>> relations = GetRelations(consumerId, providerId);

            IncrementRelationTypeCount(type);

            if (!relations.ContainsKey(type))
            {
                relations[type] = new List<DsiRelation>();
            }

            DsiRelation relation = new DsiRelation(consumerId, providerId, type, weight, properties);
            relations[type].Add(relation);
            return relation;
        }

        public void IgnoreRelation(string consumerName, string providerName, string type)
        {
            Logger.LogDataModelMessage($"Ignore relation consumerName={consumerName} providerName={providerName} type={type}");
        }

        public void SkipRelation(string consumerName, string providerName, string type)
        {
            Logger.LogDataModelMessage($"Skip relation consumerName={consumerName} providerName={providerName} type={type}");

            Logger.LogErrorDataModelRelationNotResolved(consumerName, providerName);

            _importedRelationCount++;
        }

        public void AmbiguousRelation(string consumerName, string providerName, string type)
        {
            Logger.LogDataModelMessage($"Ambiguous relation consumerName={consumerName} providerName={providerName} type={type}");
            _ambiguousRelationCount++;
        }

        public ICollection<string> GetRelationTypes()
        {
            return _relationTypeCount.Keys;
        }

        public int GetRelationTypeCount(string type)
        {
            if (_relationTypeCount.ContainsKey(type))
            {
                return _relationTypeCount[type];
            }
            else
            {
                return 0;
            }
        }

        public ICollection<IDsiRelation> GetRelationsOfConsumer(int consumerId)
        {
            List<IDsiRelation> consumerRelations = new List<IDsiRelation>();
            if (_relationsByConsumerId.ContainsKey(consumerId))
            {
                foreach (Dictionary<string, List<DsiRelation>> relationsForSpecificConsumer in _relationsByConsumerId[consumerId].Values)
                {
                    foreach (List<DsiRelation> relation in relationsForSpecificConsumer.Values)
                    {
                        consumerRelations.AddRange(relation);
                    }
                }
            }
            return consumerRelations;
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            List<IDsiRelation> allRelations = new List<IDsiRelation>();
            foreach (Dictionary<int, Dictionary<string, List<DsiRelation>>> consumerRelations in _relationsByConsumerId.Values)
            {
                foreach (Dictionary<string,List<DsiRelation>> relationsForSpecificConsumer in consumerRelations.Values)
                {
                    foreach (List<DsiRelation> relation in relationsForSpecificConsumer.Values)
                    {
                        allRelations.AddRange(relation);
                    }
                }
            }
            return allRelations;
        }

        public int CurrentRelationCount => GetRelations().Count();

        public bool DoesRelationExist(int consumerId, int providerId)
        {
            return _relationsByConsumerId.ContainsKey(consumerId) &&
                   _relationsByConsumerId[consumerId].ContainsKey(providerId);
        }

        public int ImportedRelationCount => _importedRelationCount;

        public int ResolvedRelationCount => _resolvedRelationCount;

        public int AmbiguousRelationCount => _ambiguousRelationCount;

        public double ResolvedRelationPercentage
        {
            get
            {
                double resolvedRelationPercentage = 0.0;
                if (ImportedRelationCount > 0)
                {
                    resolvedRelationPercentage = (ResolvedRelationCount * 100.0) / ImportedRelationCount;
                }
                return resolvedRelationPercentage;
            }
        }

        public double AmbiguousRelationPercentage
        {
            get
            {
                double ambiguousRelationPercentage = 0.0;
                if (ImportedRelationCount > 0)
                {
                    ambiguousRelationPercentage = (AmbiguousRelationCount * 100.0) / ImportedRelationCount;
                }
                return ambiguousRelationPercentage;
            }
        }

        private void IncrementRelationTypeCount(string type)
        {
            if (!_relationTypeCount.ContainsKey(type))
            {
                _relationTypeCount[type] = 0;
            }
            _relationTypeCount[type]++;
        }

        private Dictionary<string, List<DsiRelation>> GetRelations(int consumerId, int providerId)
        {
            if (!_relationsByConsumerId.ContainsKey(consumerId))
            {
                _relationsByConsumerId[consumerId] = new Dictionary<int, Dictionary<string, List<DsiRelation>>>();
            }

            if (!_relationsByConsumerId[consumerId].ContainsKey(providerId))
            {
                _relationsByConsumerId[consumerId][providerId] = new Dictionary<string, List<DsiRelation>>();
            }

            return _relationsByConsumerId[consumerId][providerId];
        }

        private void OnElementRemoved(object sender, int elementId)
        {
            if (_relationsByConsumerId.ContainsKey(elementId))
            {
                _relationsByConsumerId.Remove(elementId);
            }

            foreach (var relationsByProviderId in _relationsByConsumerId.Values)
            {
                if (relationsByProviderId.ContainsKey(elementId))
                {
                    relationsByProviderId.Remove(elementId);
                }
            }
        }
    }
}
