using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.Client.UI.Windows;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.Client.UI.Usercontrols.ActiveStopLimitOrders
{
    public class StopLimitOrderViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly StopLimitOrderDto stopLimitOrder;
        private readonly IClient client;

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

        private double limitPrice;
        public double LimitPrice
        {
            get { return limitPrice; }
            set
            {
                if (limitPrice == value)
                    return;

                limitPrice = value;
                OnPropertyChanged();
            }
        }

        private double triggerPrice;
        public double TriggerPrice
        {
            get { return triggerPrice; }
            set
            {
                if (triggerPrice == value)
                    return;

                triggerPrice = value;
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
        public ICommand ModifyOrderCommand { get; set; }

        public StopLimitOrderViewModel(StopLimitOrderDto stopLimitOrder, IClient client)
        {
            this.stopLimitOrder = stopLimitOrder;
            this.client = client;
            this.client.StopLimitOrderChanged += client_StopLimitOrderChanged;
            SetFields(stopLimitOrder);

            SetupCommandsAndBehaviour();
        }

        private void SetupCommandsAndBehaviour()
        {
            CancelOrderCommand = new RelayCommand(() =>
            {
                client.CancelStopLimitOrder(orderId);
            });

            ModifyOrderCommand = new RelayCommand(() =>
            {
                var editViewModel = new EditStopLimitOrderViewModel(client, stopLimitOrder);
                var editView = new EditStopLimitOrderWindow();
                editView.DataContext = editViewModel;
                editView.Show();

            });

        }

        void client_StopLimitOrderChanged(object sender, StopLimitOrderDto e)
        {
            if (e.ExchangeOrderId != stopLimitOrder.ExchangeOrderId)
                return;
            SetFields(e);
        }

        private void SetFields(StopLimitOrderDto stopLimitOrder)
        {
            Symbol = stopLimitOrder.Symbol;
            Quantity = stopLimitOrder.Quantity;
            TriggerPrice = stopLimitOrder.TriggerPrice;
            LimitPrice = stopLimitOrder.LimitPrice;
            OrderId = stopLimitOrder.ExchangeOrderId;
            Way = stopLimitOrder.Way;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            this.client.StopLimitOrderChanged -= client_StopLimitOrderChanged;
        }
    }
}
