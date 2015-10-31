using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MemExchange.Client.UI.Annotations;
using MemExchange.Client.UI.Resources;
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto;

namespace MemExchange.Client.UI.Usercontrols
{
    public class NewMarketOrderViewModel : INotifyPropertyChanged
    {
        private readonly IClient client;
        private string symbol;
        private int quantity;
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

        public ICommand BuyCommand { get; set; }
        public ICommand SellCommand { get; set; }

        public NewMarketOrderViewModel(IClient client)
        {
            this.client = client;
            SetupCommandsAndBehaviour();
        }

        private void SetupCommandsAndBehaviour()
        {
            BuyCommand = new RelayCommand(() => client.SubmitMarketOrder(Symbol, Quantity, WayEnum.Buy));
            SellCommand = new RelayCommand(() => client.SubmitMarketOrder(Symbol, Quantity, WayEnum.Sell));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
