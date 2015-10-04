using System.ComponentModel;
using System.Runtime.CompilerServices;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Setup;
using MemExchange.Client.UI.Usercontrols;
using MemExchange.ClientApi;

namespace MemExchange.Client.UI.Windows
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public NewLimitOrderViewModel LimitOrderViewModel { get; set; }

        public MainWindowViewModel()
        {
            LimitOrderViewModel = new NewLimitOrderViewModel(DependencyInjection.Container.Resolve<IClient>());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
