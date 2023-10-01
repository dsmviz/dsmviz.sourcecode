using System.Collections.Generic;
using Dsmviz.Application.Interfaces;
using Dsmviz.Viewmodel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;

namespace Dsmviz.Viewmodel.Lists
{
    public class ActionListViewmodel : ViewmodelBase
    {
        private readonly IDsmApplication _application;
        private IEnumerable<ActionListItemViewmodel> _actions;

        public ActionListViewmodel(IDsmApplication application)
        {
            Title = "Edit history";

            _application = application;
            _application.ActionPerformed += OnActionPerformed;

            UpdateActionList();
            
            CopyToClipboardCommand =  new RelayCommand<object>(CopyToClipboardExecute);
            ClearCommand = new RelayCommand<object>(ClearExecute);
        }

        private void OnActionPerformed(object sender, System.EventArgs e)
        {
            UpdateActionList();
        }

        public string Title { get; }
        public string SubTitle { get; }

        public IEnumerable<ActionListItemViewmodel> Actions
        {
            get { return _actions; }
            set { _actions = value; OnPropertyChanged(); }
        }

        public ICommand CopyToClipboardCommand { get; }
        public ICommand ClearCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach(ActionListItemViewmodel Viewmodel in Actions)
            {
                builder.AppendLine($"{Viewmodel.Index, -5}, {Viewmodel.Action, -30}, {Viewmodel.Details}");
            }
            Clipboard.SetText(builder.ToString());
        }

        private void ClearExecute(object parameter)
        {
            _application.ClearActions();
            UpdateActionList();
        }

        private void UpdateActionList()
        {
            var actionViewmodels = new List<ActionListItemViewmodel>();
            int index = 1;
            foreach (IAction action in _application.GetActions())
            {
                actionViewmodels.Add(new ActionListItemViewmodel(index, action));
                index++;
            }

            Actions = actionViewmodels;
        }
    }
}
