using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Client.UI.Usercontrols.ActiveOrders
{
    public class EditLimitOrderViewModel : INotifyPropertyChanged
    {
        private LimitOrder limitOrder;
        private readonly IClient client;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SendOrderCommand { get; set; }

        public EditLimitOrderViewModel(LimitOrder limitOrder, IClient client)
        {
            this.limitOrder = limitOrder;
            this.client = client;

            SetupCommandsAndBehaviour();
        }

        private void SetupCommandsAndBehaviour()
        {
            SendOrderCommand = new RelayCommand(() => client.ModifyLimitOrder(limitOrder.ExchangeOrderId, limitOrder.Price, limitOrder.Quantity));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
