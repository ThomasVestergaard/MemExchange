using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Castle.Components.DictionaryAdapter;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto;

namespace MemExchange.Client.UI.Usercontrols
{
    public class NewLimitOrderViewModel : INotifyPropertyChanged
    {
        private readonly IClient exchangeApi;

        public event PropertyChangedEventHandler PropertyChanged;

        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            set
            {
                symbol = value;
                OnPropertyChanged();
            }
        }

        private string price;
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        private string triggerPrice;
        public string TriggerPrice
        {
            get { return triggerPrice; }
            set
            {
                triggerPrice = value;
                OnPropertyChanged();
            }
        }

        private string quantity;
        public string Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }

        public List<string> LimitOrderTypes { get; set; }

        private string selectedLimitOrderType;
        public string SelectedLimitOrderType
        {
            get { return selectedLimitOrderType; }
            set
            {
                selectedLimitOrderType = value;
                OnPropertyChanged();
            }
        }

        public List<WayEnum> Ways { get; set; }

        private WayEnum selectedWay;
        public WayEnum SelectedWay
        {
            get { return selectedWay; }
            set { selectedWay = value; }
        }

        public ICommand SendOrderCommand { get; set; }

        public NewLimitOrderViewModel(IClient exchangeApi)
        {
            this.exchangeApi = exchangeApi;
            LimitOrderTypes = new List<string> {"Limit order", "Stop limit order"};
            Ways = new List<WayEnum> { WayEnum.Buy, WayEnum.Sell };
            selectedLimitOrderType = LimitOrderTypes[0];
            selectedWay = WayEnum.Buy;

            SetupCommandsAndBehaviour();
        }

        private void SetupCommandsAndBehaviour()
        {
            SendOrderCommand = new RelayCommand(() =>
            {
                if (selectedLimitOrderType == "Limit order")
                    SendLimitOrder();
                else if (selectedLimitOrderType == "Stop limit order")
                    SendStopLimitOrder();

                
            });
        }

        private void SendStopLimitOrder()
        {
            double parsedLimitPrice;
            double parsedTriggerPrice;
            int parsedQuantity;

            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Symbol must be entered.");
                return;
            }

            if (!double.TryParse(price, out parsedLimitPrice))
            {
                MessageBox.Show("Price must be a double value.");
                return;
            }

            if (!double.TryParse(triggerPrice, out parsedTriggerPrice))
            {
                MessageBox.Show("Trigger price must be a double value.");
                return;
            }

            if (!int.TryParse(quantity, out parsedQuantity))
            {
                MessageBox.Show("Quantity must be a integer value.");
                return;
            }

            exchangeApi.SubmitStopLimitOrder(symbol, parsedTriggerPrice, parsedLimitPrice, parsedQuantity, selectedWay);
            Console.WriteLine("Order sent.");
        }

        private void SendLimitOrder()
        {
            double parsedPrice;
            int parsedQuantity;

            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Symbol must be entered.");
                return;
            }

            if (!double.TryParse(price, out parsedPrice))
            {
                MessageBox.Show("Price must be a double value.");
                return;
            }

            if (!int.TryParse(quantity, out parsedQuantity))
            {
                MessageBox.Show("Quantity must be a integer value.");
                return;
            }

            exchangeApi.SubmitLimitOrder(symbol, parsedPrice, parsedQuantity, selectedWay);
            Console.WriteLine("Order sent.");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
