using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.Client.UI.Setup;
using MemExchange.Client.UI.Windows;
using MemExchange.ClientApi;

namespace MemExchange.Client.UI.Usercontrols.ActiveStopLimitOrders
{
    public class ActiveStopLimitOrdersViewModel : INotifyPropertyChanged
    {
        private readonly IClient client;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RefreshCommand { get; set; }
        
        public ObservableCollection<StopLimitOrderViewModel> ActiveStopLimitOrders { get; set; } 

        public ActiveStopLimitOrdersViewModel(IClient client)
        {
            this.client = client;
            ActiveStopLimitOrders = new ObservableCollection<StopLimitOrderViewModel>();
            this.client.StopLimitOrderAccepted += client_StopLimitOrderAccepted;
            this.client.StopLimitOrderDeleted += client_StopLimitOrderDeleted;
            this.client.StopLimitOrderSnapshot += client_StopLimitOrderSnapshot;
            SetupCommandsAndBehaviour();
        }

        void client_StopLimitOrderSnapshot(object sender, System.Collections.Generic.List<Core.SharedDto.Orders.StopLimitOrderDto> e)
        {
            UiDispatcher.Dispatcher.Invoke(() =>
            {
              
                ActiveStopLimitOrders.Clear();

                foreach (var stopLimitOrderDto in e)
                    ActiveStopLimitOrders.Add(new StopLimitOrderViewModel(stopLimitOrderDto, client));
            });
        }

        private void SetupCommandsAndBehaviour()
        {
            RefreshCommand = new RelayCommand(() =>
            {
                client.RequestOpenStopLimitOrders();
            });

           
        }

        void client_StopLimitOrderDeleted(object sender, Core.SharedDto.Orders.StopLimitOrderDto e)
        {
            UiDispatcher.Dispatcher.Invoke(() =>
            {
                var order = ActiveStopLimitOrders.FirstOrDefault(a => a.OrderId == e.ExchangeOrderId);
                if (order == null)
                    return;

                ActiveStopLimitOrders.Remove(order);
                order.Dispose();
            });
        }

        void client_StopLimitOrderAccepted(object sender, Core.SharedDto.Orders.StopLimitOrderDto e)
        {
            UiDispatcher.Dispatcher.Invoke(() =>
            {
                ActiveStopLimitOrders.Add(new StopLimitOrderViewModel(e, client));
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
