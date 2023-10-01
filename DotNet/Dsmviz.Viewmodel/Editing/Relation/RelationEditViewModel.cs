using System;
using System.Collections.Generic;
using System.Windows.Input;
using Dsmviz.Application.Interfaces;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Common;
using Dsmviz.Viewmodel.Main;

namespace Dsmviz.Viewmodel.Editing.Relation
{
    public class RelationEditViewmodel : ViewmodelBase
    {
        private readonly RelationEditViewmodelType _ViewmodelType;
        private readonly IDsmApplication _application;
        private readonly IDsmRelation _selectedRelation;
        private readonly IDsmElement _searchPathConsumer;
        private IDsmElement _selectedConsumer;
        private readonly IDsmElement _searchPathProvider;
        private IDsmElement _selectedProvider;
        private string _selectedRelationType;
        private int _weight;
        private string _help;

        private static string _lastSelectedRelationType = "";
        private static string _lastSelectedConsumerElementType = "";
        private static string _lastSelectedProviderElementType = "";

        public event EventHandler<IDsmRelation> RelationUpdated;

        public RelationEditViewmodel(RelationEditViewmodelType ViewmodelType, IDsmApplication application, IDsmRelation selectedRelation, IDsmElement searchPathConsumer, IDsmElement selectedConsumer, IDsmElement searchPathProvider, IDsmElement selectedProvider)
        {
            _ViewmodelType = ViewmodelType;
            _application = application;

            switch (_ViewmodelType)
            {
                case RelationEditViewmodelType.Modify:
                    Title = "Modify relation";
                    _selectedRelation = selectedRelation;
                    _searchPathConsumer = null;
                    _selectedConsumer = _selectedRelation.Consumer;
                    _searchPathProvider = null;
                    _selectedProvider = _selectedRelation.Provider;
                    SelectedRelationType = _selectedRelation.Type;
                    Weight = _selectedRelation.Weight;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptModifyExecute, AcceptCanExecute);
                    break;
                case RelationEditViewmodelType.Add:
                    Title = "Add relation";
                    _selectedRelation = null;
                    _searchPathConsumer = searchPathConsumer;
                    _selectedConsumer = selectedConsumer;
                    _searchPathProvider = searchPathProvider;
                    _selectedProvider = selectedProvider;
                    SelectedRelationType = _lastSelectedRelationType;
                    Weight = 1;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptAddExecute, AcceptCanExecute);
                    break;
                default:
                    break;
            }

            ConsumerSearchViewmodel = new ElementSearchViewmodel(application, _searchPathConsumer, _selectedConsumer, _lastSelectedConsumerElementType, false);
            ProviderSearchViewmodel = new ElementSearchViewmodel(application, _searchPathProvider, _selectedProvider, _lastSelectedProviderElementType, false);
            RelationTypes = new List<string>(application.GetRelationTypes());
        }

        public string Title { get; }

        public ElementSearchViewmodel ConsumerSearchViewmodel { get; }

        public ElementSearchViewmodel ProviderSearchViewmodel { get; }

        public List<string> RelationTypes { get; }

        public string SelectedRelationType
        {
            get { return _selectedRelationType; }
            set { _selectedRelationType = value; _lastSelectedRelationType = value;  OnPropertyChanged(); }
        }

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; OnPropertyChanged(); }
        }

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

        public ICommand AcceptChangeCommand { get; }

        private void AcceptModifyExecute(object parameter)
        {
            bool relationUpdated = false;
            if (_selectedRelation.Type != SelectedRelationType)
            {
                _application.ChangeRelationType(_selectedRelation, SelectedRelationType);
                relationUpdated = true;
            }

            if (_selectedRelation.Weight != Weight)
            {
                _application.ChangeRelationWeight(_selectedRelation, Weight);
                relationUpdated = true;
            }

            if (relationUpdated)
            {
                InvokeRelationUpdated(_selectedRelation);
            }
        }

        private void AcceptAddExecute(object parameter)
        {
            IDsmRelation createdRelation = _application.CreateRelation(ConsumerSearchViewmodel.SelectedElement, ProviderSearchViewmodel.SelectedElement, SelectedRelationType, Weight);
            InvokeRelationUpdated(createdRelation);
        }

        private bool AcceptCanExecute(object parameter)
        {
            if (ConsumerSearchViewmodel.SelectedElement == null)
            {
                Help = "No consumer selected";
                return false;
            }
            else if (ProviderSearchViewmodel.SelectedElement == null)
            {
                Help = "No provider selected";
                return false;
            }
            else if (ConsumerSearchViewmodel.SelectedElement == ProviderSearchViewmodel.SelectedElement)
            {
                Help = "Can not connect to itself";
                return false;
            }
            else if (ConsumerSearchViewmodel.SelectedElement.IsRecursiveChildOf(ProviderSearchViewmodel.SelectedElement))
            {
                Help = "Can not connect to child";
                return false;
            }
            else if (ProviderSearchViewmodel.SelectedElement.IsRecursiveChildOf(ConsumerSearchViewmodel.SelectedElement))
            {
                Help = "Can not connect to child";
                return false;
            }
            else if (SelectedRelationType == null)
            {
                Help = "No relation type selected";
                return false;
            }
            else if (Weight < 0)
            {
                Help = "Weight can not be negative";
                return false;
            }
            else if (Weight == 0)
            {
                Help = "Weight can not be zero";
                return false;
            }
            else
            {
                Help = "";
                return true;
            }
        }
        
        private void InvokeRelationUpdated(IDsmRelation updateRelation)
        {
            _lastSelectedConsumerElementType = ConsumerSearchViewmodel.SelectedElementType;
            _lastSelectedProviderElementType = ProviderSearchViewmodel.SelectedElementType;
            RelationUpdated?.Invoke(this, updateRelation);
        }
    }
}
