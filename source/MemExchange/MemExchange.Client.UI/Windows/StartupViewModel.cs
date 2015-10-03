using System.ComponentModel;
using System.Runtime.CompilerServices;
using MemExchange.Client.UI.Annotations;

namespace MemExchange.Client.UI.Windows
{
    public class StartupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
