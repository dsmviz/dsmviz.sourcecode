using System.Windows.Input;
using Dsmviz.Application.Interfaces;
using Dsmviz.Viewmodel.Common;

namespace Dsmviz.Viewmodel.Editing.Snapshot
{
    public class SnapshotMakeViewmodel : ViewmodelBase
    {
        private readonly IDsmApplication _application;
        private string _description;
        private string _help;

        public ICommand AcceptChangeCommand { get; }

        public SnapshotMakeViewmodel(IDsmApplication application)
        {
            _application = application;

            Title = "Make snapshot";
            Help = "";

            Description = "";
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged();  }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.MakeSnapshot(Description);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Description.Length > 0;
        }
    }
}
