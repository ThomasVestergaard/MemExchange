using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Client.UI.Windows
{
    public class EditStopLimitOrderViewModel : INotifyPropertyChanged
    {
        private readonly IClient client;
        private readonly StopLimitOrderDto stopLimitOrder;
        private string symbol;
        private int quantity;
        private double limitPrice;
        private double triggerPrice;
        private WayEnum way;
        private uint exchangeOrderId;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Symbol
        {
            get { return symbol; }
            set
            {
                symbol = value;
                OnPropertyChanged();
            }
        }
        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }
        public double LimitPrice
        {
            get { return limitPrice; }
            set
            {
                limitPrice = value;
                OnPropertyChanged();
            }
        }
        public double TriggerPrice
        {
            get { return triggerPrice; }
            set
            {
                triggerPrice = value;
                OnPropertyChanged();
            }
        }
        public WayEnum Way
        {
            get { return way; }
            set
            {
                way = value;
                OnPropertyChanged();
            }
        }
        public uint ExchangeOrderId
        {
            get { return exchangeOrderId; }
            set
            {
                exchangeOrderId = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendOrderCommand { get; set; }

        public EditStopLimitOrderViewModel(IClient client, StopLimitOrderDto stopLimitOrder)
        {
            this.client = client;
            this.stopLimitOrder = stopLimitOrder;
            SetFields(stopLimitOrder);
            SetupCommandsAndBehaviour();
        }

        private void SetupCommandsAndBehaviour()
        {
            SendOrderCommand = new RelayCommand(() =>
            {
                client.ModifyStopLimitOrder(exchangeOrderId, triggerPrice, limitPrice, quantity);
            });
        }

        private void SetFields(StopLimitOrderDto stopLimitOrder)
        {
            symbol = stopLimitOrder.Symbol;
            quantity = stopLimitOrder.Quantity;
            limitPrice = stopLimitOrder.LimitPrice;
            triggerPrice = stopLimitOrder.TriggerPrice;
            way = stopLimitOrder.Way;
            exchangeOrderId = stopLimitOrder.ExchangeOrderId;

        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
