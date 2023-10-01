using System.Collections.Generic;
using Dsmviz.Viewmodel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Application.Interfaces;
using System.Collections.ObjectModel;
using Dsmviz.Viewmodel.Editing.Relation;
using System;
using System.Xml.Linq;

namespace Dsmviz.Viewmodel.Lists
{
    public class RelationListViewmodel : ViewmodelBase
    {
        private readonly RelationsListViewmodelType _ViewmodelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _selectedConsumer;
        private readonly IDsmElement _selectedProvider;
        private ObservableCollection<RelationListItemViewmodel> _relations;
        private RelationListItemViewmodel _selectedRelation;

        public event EventHandler<RelationEditViewmodel> RelationAddStarted;
        public event EventHandler<RelationEditViewmodel> RelationEditStarted;

        public RelationListViewmodel(RelationsListViewmodelType ViewmodelType, IDsmApplication application, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            _ViewmodelType = ViewmodelType;
            _application = application;
            _selectedConsumer = selectedConsumer;
            _selectedProvider = selectedProvider;

            Title = "Relation List";
            switch (ViewmodelType)
            {
                case RelationsListViewmodelType.ElementIngoingRelations:
                    SubTitle = $"Ingoing relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddConsumerRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewmodelType.ElementOutgoingRelations:
                    SubTitle = $"Outgoing relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddProviderRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewmodelType.ElementInternalRelations:
                    SubTitle = $"Internal relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddInternalRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewmodelType.ConsumerProviderRelations:
                    SubTitle = $"Relations between consumer {_selectedConsumer.Fullname} and provider {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddConsumerProviderRelationExecute, AddRelationCanExecute);
                    break;
                default:
                    SubTitle = "";
                    break;
            }

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
            DeleteRelationCommand = new RelayCommand<object>(DeleteRelationExecute, DeleteRelationCanExecute);
            EditRelationCommand = new RelayCommand<object>(EditRelationExecute, EditRelationCanExecute);

            UpdateRelations(null);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public ObservableCollection<RelationListItemViewmodel> Relations
        {
            get { return _relations; }
            private set { _relations = value; OnPropertyChanged(); }
        }

        public RelationListItemViewmodel SelectedRelation
        {
            get { return _selectedRelation; }
            set { _selectedRelation = value; OnPropertyChanged(); }
        }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand DeleteRelationCommand { get; }
        public ICommand EditRelationCommand { get; }
        public ICommand AddRelationCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            if (Relations.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                StringBuilder headerLine = new StringBuilder();
                headerLine.Append($"Index,");
                headerLine.Append($"ConsumerPath,");
                headerLine.Append($"ConsumerName,");
                headerLine.Append($"ProviderPath,");
                headerLine.Append($"ProviderName,");
                headerLine.Append($"Type,");
                headerLine.Append($"Weight,");
                headerLine.Append($"Cyclic,");
                foreach (string propertyName in Relations[0].DiscoveredRelationPropertyNames())
                {
                    headerLine.Append($"{propertyName},");
                }
                builder.AppendLine(headerLine.ToString());

                foreach (RelationListItemViewmodel Viewmodel in Relations)
                {
                    StringBuilder line = new StringBuilder();
                    line.Append($"{Viewmodel.Index},");
                    line.Append($"{Viewmodel.ConsumerPath},");
                    line.Append($"{Viewmodel.ConsumerName},");
                    line.Append($"{Viewmodel.ProviderPath},");
                    line.Append($"{Viewmodel.ProviderName},");
                    line.Append($"{Viewmodel.RelationType},");
                    line.Append($"{Viewmodel.RelationWeight},");
                    line.Append($"{Viewmodel.Cyclic},");
                    foreach (string propertyName in Relations[0].DiscoveredRelationPropertyNames())
                    {
                        string propertyValue = Viewmodel.Properties.ContainsKey(propertyName) ? Viewmodel.Properties[propertyName] : "";
                        line.Append($"{propertyValue},");
                    }
                    builder.AppendLine(line.ToString());
                }
                Clipboard.SetText(builder.ToString());
            }
        }

        private void DeleteRelationExecute(object parameter)
        {
            _application.DeleteRelation(SelectedRelation.Relation);
            UpdateRelations(SelectedRelation.Relation);
        }

        private bool DeleteRelationCanExecute(object parameter)
        {
            return SelectedRelation != null;
        }

        private void EditRelationExecute(object parameter)
        {
            RelationEditViewmodel relationEditViewmodel = new RelationEditViewmodel(RelationEditViewmodelType.Modify, _application, SelectedRelation.Relation, null, null, null, null);
            relationEditViewmodel.RelationUpdated += OnRelationUpdated;
            RelationEditStarted?.Invoke(this, relationEditViewmodel);
        }

        private bool EditRelationCanExecute(object parameter)
        {
            return SelectedRelation != null;
        }

        private void AddConsumerRelationExecute(object parameter)
        {
            RelationEditViewmodel relationEditViewmodel = new RelationEditViewmodel(RelationEditViewmodelType.Add, _application, null, _application.RootElement, null, null, _selectedProvider);
            relationEditViewmodel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewmodel);
        }

        private void AddProviderRelationExecute(object parameter)
        {
            RelationEditViewmodel relationEditViewmodel = new RelationEditViewmodel(RelationEditViewmodelType.Add, _application, null, null, _selectedProvider, _application.RootElement, null);
            relationEditViewmodel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewmodel);
        }

        private void AddInternalRelationExecute(object parameter)
        {
            RelationEditViewmodel relationEditViewmodel = new RelationEditViewmodel(RelationEditViewmodelType.Add, _application, null, _selectedProvider, null, _selectedProvider, null);
            relationEditViewmodel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewmodel);
        }

        private void AddConsumerProviderRelationExecute(object parameter)
        {
            RelationEditViewmodel relationEditViewmodel = new RelationEditViewmodel(RelationEditViewmodelType.Add, _application, null, _selectedConsumer, null, _selectedProvider, null);
            relationEditViewmodel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewmodel);
        }

        private bool AddRelationCanExecute(object parameter)
        {
            return true;
        }

        private void OnRelationUpdated(object sender, IDsmRelation updatedRelation)
        {
            UpdateRelations(updatedRelation);
        }

        private void UpdateRelations(IDsmRelation updatedRelation)
        {
            RelationListItemViewmodel selectedRelationListItemViewmodel = null;
            IEnumerable<IDsmRelation> relations;
            switch (_ViewmodelType)
            {
                case RelationsListViewmodelType.ElementIngoingRelations:
                    relations = _application.FindIngoingRelations(_selectedProvider);
                    break;
                case RelationsListViewmodelType.ElementOutgoingRelations:
                    relations = _application.FindOutgoingRelations(_selectedProvider);
                    break;
                case RelationsListViewmodelType.ElementInternalRelations:
                    relations = _application.FindInternalRelations(_selectedProvider);
                    break;
                case RelationsListViewmodelType.ConsumerProviderRelations:
                    relations = _application.FindResolvedRelations(_selectedConsumer, _selectedProvider);
                    break;
                default:
                    relations = new List<IDsmRelation>();
                    break;
            }

            List<RelationListItemViewmodel> relationViewmodels = new List<RelationListItemViewmodel>();

            foreach (IDsmRelation relation in relations)
            {
                RelationListItemViewmodel relationListItemViewmodel = new RelationListItemViewmodel(_application, relation);
                relationViewmodels.Add(relationListItemViewmodel);

                if (updatedRelation != null)
                {
                    if (relation.Id == updatedRelation.Id)
                    {
                        selectedRelationListItemViewmodel = relationListItemViewmodel;
                    }
                }
            }

            relationViewmodels.Sort();

            int index = 1;
            foreach (RelationListItemViewmodel Viewmodel in relationViewmodels)
            {
                Viewmodel.Index = index;
                index++;
            }

            Relations = new ObservableCollection<RelationListItemViewmodel>(relationViewmodels);
            SelectedRelation = selectedRelationListItemViewmodel;
        }
    }
}
