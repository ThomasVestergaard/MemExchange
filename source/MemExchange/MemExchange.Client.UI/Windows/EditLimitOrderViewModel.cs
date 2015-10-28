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
    public class EditLimitOrderViewModel : INotifyPropertyChanged
    {
        private LimitOrderDto limitOrder;
        private readonly IClient client;
        
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

        private WayEnum way;
        public WayEnum Way
        {
            get { return way; }
            set
            {
                way = value;
                OnPropertyChanged();
            }
        }

        private double price;
        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand SendOrderCommand { get; set; }

        public EditLimitOrderViewModel(LimitOrderDto limitOrder, IClient client)
        {
            this.limitOrder = limitOrder;
            this.client = client;

            SetupCommandsAndBehaviour();
            SetFields();
        }

        private void SetFields()
        {
            Symbol = limitOrder.Symbol;
            Quantity = limitOrder.Quantity;
            Price = limitOrder.Price;
            Way = limitOrder.Way;
        }

        private void SetupCommandsAndBehaviour()
        {
            SendOrderCommand = new RelayCommand(() => 
                client.ModifyLimitOrder(limitOrder.ExchangeOrderId, price, quantity));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
