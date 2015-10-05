using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Client.UI.Usercontrols.ActiveOrders
{
    public class LimitOrderViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly LimitOrder limitOrder;
        private readonly IClient client;

        public event PropertyChangedEventHandler PropertyChanged;
        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            set
            {
                if (symbol == value)
                    return;

                symbol = value;
                OnPropertyChanged();
            }
        }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity == value)
                    return;

                quantity = value;
                OnPropertyChanged();
            }
        }

        private double price;
        public double Price
        {
            get { return price; }
            set
            {
                if (price == value)
                    return;

                price = value;
                OnPropertyChanged();
            }
        }

        private WayEnum way;
        public WayEnum Way
        {
            get { return way; }
            set
            {
                if (way == value)
                    return;

                way = value;
                OnPropertyChanged();
            }
        }

        private string orderType;
        public string OrderType
        {
            get { return orderType; }
            set
            {
                if (orderType == value)
                    return;

                orderType = value;
                OnPropertyChanged();
            }
        }

        private uint orderId;
        public uint OrderId
        {
            get { return orderId; }
            set
            {
                if (orderId == value)
                    return;

                orderId = value;
                OnPropertyChanged();
            }
        }

        public ICommand CancelOrderCommand { get; set; }

        public LimitOrderViewModel(LimitOrder limitOrder, IClient client)
        {
            this.limitOrder = limitOrder;
            this.client = client;
            SetFields(limitOrder);
            this.client.LimitOrderChanged += client_LimitOrderChanged;

            SetupCommandsAndBehaviour();
        }

        private void SetupCommandsAndBehaviour()
        {
            CancelOrderCommand = new RelayCommand(() =>
            {
                client.CancelLimitOrder(limitOrder.ExchangeOrderId);
            });
        }

        private void SetFields(LimitOrder limitOrder)
        {
            Symbol = limitOrder.Symbol;
            Quantity = limitOrder.Quantity;
            Price = limitOrder.Price;
            Way = limitOrder.Way;
            OrderType = "Limit order";
            OrderId = limitOrder.ExchangeOrderId;
        }

        void client_LimitOrderChanged(object sender, LimitOrder e)
        {
            if (e.ExchangeOrderId != limitOrder.ExchangeOrderId)
                return;

            SetFields(e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            client.LimitOrderChanged -= client_LimitOrderChanged;
        }
    }
}
