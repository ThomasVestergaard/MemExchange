using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Setup;
using MemExchange.ClientApi;

namespace MemExchange.Client.UI.Usercontrols.ActiveOrders
{
    public class ActiveOrdersViewModel : INotifyPropertyChanged
    {
        private readonly IClient client;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<LimitOrderViewModel> LimitOrders { get; set; }

        public ActiveOrdersViewModel(IClient client)
        {
            this.client = client;
            LimitOrders = new ObservableCollection<LimitOrderViewModel>();

            this.client.LimitOrderAccepted += client_LimitOrderAccepted;
        }

        void client_LimitOrderAccepted(object sender, Core.SharedDto.Orders.LimitOrder e)
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
