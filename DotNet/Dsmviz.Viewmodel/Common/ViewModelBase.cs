using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dsmviz.Viewmodel.Properties;

namespace Dsmviz.Viewmodel.Common
{
    public class ViewmodelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
