using System.Collections.Generic;
using Dsmviz.Datamodel.Dsm.Interfaces;
using Dsmviz.Viewmodel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using System.Collections.ObjectModel;
using Dsmviz.Application.Interfaces;

namespace Dsmviz.Viewmodel.Lists
{
    public class ElementListViewmodel : ViewmodelBase
    {
        private readonly ElementListViewmodelType _ViewmodelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _selectedConsumer;
        private readonly IDsmElement _selectedProvider;

        public ElementListViewmodel(ElementListViewmodelType ViewmodelType, IDsmApplication application, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            _ViewmodelType = ViewmodelType;
            _application = application;
            _selectedConsumer = selectedConsumer;
            _selectedProvider = selectedProvider;

            Title = "Element List";

            IEnumerable<IDsmElement> elements;
            switch (ViewmodelType)
            {
                case ElementListViewmodelType.RelationConsumers:
                    SubTitle = $"Consumers in relations between consumer {_selectedConsumer.Fullname} and provider {_selectedProvider.Fullname}";
                    elements = _application.GetRelationConsumers(_selectedConsumer, _selectedProvider);
                    break;
                case ElementListViewmodelType.RelationProviders:
                    SubTitle = $"Providers in relations between consumer {_selectedConsumer.Fullname} and provider {_selectedProvider.Fullname}";
                    elements = _application.GetRelationProviders(_selectedConsumer, _selectedProvider);
                    break;
                case ElementListViewmodelType.ElementConsumers:
                    SubTitle = $"Consumers of {_selectedProvider.Fullname}";
                    elements = _application.GetElementConsumers(_selectedProvider);
                    break;
                case ElementListViewmodelType.ElementProvidedInterface:
                    SubTitle = $"Provided interface of {_selectedProvider.Fullname}";
                    elements = _application.GetElementProvidedElements(_selectedProvider);
                    break;
                case ElementListViewmodelType.ElementRequiredInterface:
                    SubTitle = $"Required interface of {_selectedProvider.Fullname}";
                    elements = _application.GetElementProviders(_selectedProvider);
                    break;
                default:
                    SubTitle = "";
                    elements = new List<IDsmElement>();
                    break;
            }

            List<ElementListItemViewmodel> elementViewmodels = new List<ElementListItemViewmodel>();

            foreach (IDsmElement element in elements)
            {
                elementViewmodels.Add(new ElementListItemViewmodel(element));
            }

            elementViewmodels.Sort();

            int index = 1;
            foreach (ElementListItemViewmodel Viewmodel in elementViewmodels)
            {
                Viewmodel.Index = index;
                index++;
            }

            Elements = new ObservableCollection<ElementListItemViewmodel>(elementViewmodels);

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public ObservableCollection<ElementListItemViewmodel> Elements { get;  }

        public ElementListItemViewmodel SelectedElement { get; set; }

        public ICommand CopyToClipboardCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            if (Elements.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                StringBuilder headerLine = new StringBuilder();
                headerLine.Append($"Index,");
                headerLine.Append($"Path,");
                headerLine.Append($"Name,");
                headerLine.Append($"Type,");
                foreach (string propertyName in Elements[0].DiscoveredElementPropertyNames())
                {
                    headerLine.Append($"{propertyName},");
                }
                builder.AppendLine(headerLine.ToString());

                foreach (ElementListItemViewmodel Viewmodel in Elements)
                {
                    StringBuilder line = new StringBuilder();
                    line.Append($"{Viewmodel.Index},");
                    line.Append($"{Viewmodel.ElementPath},");
                    line.Append($"{Viewmodel.ElementName},");
                    line.Append($"{Viewmodel.ElementType},");
                    foreach (string propertyName in Elements[0].DiscoveredElementPropertyNames())
                    {
                        string propertyValue = Viewmodel.Properties.ContainsKey(propertyName) ? Viewmodel.Properties[propertyName] : "";
                        line.Append($"{propertyValue},");
                    }
                    builder.AppendLine(line.ToString());
                }
                Clipboard.SetText(builder.ToString());
            }
        }
    }
}
