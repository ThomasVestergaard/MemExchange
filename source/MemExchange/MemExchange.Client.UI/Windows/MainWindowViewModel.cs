using System.ComponentModel;
using System.Runtime.CompilerServices;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Setup;
using MemExchange.Client.UI.Usercontrols;
using MemExchange.Client.UI.Usercontrols.ActiveLimitOrders;
using MemExchange.Client.UI.Usercontrols.ActiveStopLimitOrders;
using MemExchange.Client.UI.Usercontrols.Executions;
using MemExchange.Client.UI.Usercontrols.Level1;
using MemExchange.ClientApi;

namespace MemExchange.Client.UI.Windows
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public NewLimitOrderViewModel LimitOrderViewModel { get; set; }
        public ActiveOrdersViewModel ActiveOrdersViewModel { get; set; }
        public ClientExecutionsViewModel ExecutionsViewModel { get; set; }
        public Level1ViewModel L1ViewModel { get; set; }
        public NewMarketOrderViewModel MarketOrderViewModel { get; set; }
        public ActiveStopLimitOrdersViewModel StopLimitOrdersViewModel { get; set; }

        public MainWindowViewModel()
        {
            var client = DependencyInjection.Container.Resolve<IClient>();
            LimitOrderViewModel = new NewLimitOrderViewModel(client);
            ActiveOrdersViewModel = new ActiveOrdersViewModel(client);
            ExecutionsViewModel = new ClientExecutionsViewModel(client);
            L1ViewModel = new Level1ViewModel(client);
            MarketOrderViewModel = new NewMarketOrderViewModel(client);
            StopLimitOrdersViewModel = new ActiveStopLimitOrdersViewModel(client);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
