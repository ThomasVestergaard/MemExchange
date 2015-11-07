using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.Client.UI.Setup;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Client.UI.Usercontrols.ActiveLimitOrders
{
    public class ActiveOrdersViewModel : INotifyPropertyChanged
    {
        private readonly IClient client;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<LimitOrderViewModel> LimitOrders { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ActiveOrdersViewModel(IClient client)
        {
            this.client = client;
            LimitOrders = new ObservableCollection<LimitOrderViewModel>();

            this.client.LimitOrderAccepted += client_LimitOrderAccepted;
            this.client.LimitOrderDeleted += client_LimitOrderDeleted;
            this.client.LimitOrderSnapshot += client_LimitOrderSnapshot;

            SetupCommandsAndBehaviour();
        }

        private void SetupCommandsAndBehaviour()
        {
            RefreshCommand = new RelayCommand(() =>
            {
               client.RequestOpenLimitOrders();
            });
        }

        void client_LimitOrderSnapshot(object sender, System.Collections.Generic.List<LimitOrderDto> e)
        {
            UiDispatcher.Dispatcher.Invoke(() =>
            {
                LimitOrders.Clear();
                foreach (var limitOrderViewModel in LimitOrders)
                {
                    LimitOrders.Remove(limitOrderViewModel);
                    limitOrderViewModel.Dispose();
                }

                foreach (var limitOrder in e)
                    LimitOrders.Add(new LimitOrderViewModel(limitOrder, client));
            });
        }

        void client_LimitOrderDeleted(object sender, LimitOrderDto e)
        {
            UiDispatcher.Dispatcher.Invoke(() =>
            {
                var order = LimitOrders.FirstOrDefault(a => a.OrderId == e.ExchangeOrderId);
                if (order == null)
                    return;

                LimitOrders.Remove(order);
                order.Dispose();
            });
        }

        void client_LimitOrderAccepted(object sender, LimitOrderDto e)
        {
            UiDispatcher.Dispatcher.Invoke(() =>
            {
                LimitOrders.Add(new LimitOrderViewModel(e, client));
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
