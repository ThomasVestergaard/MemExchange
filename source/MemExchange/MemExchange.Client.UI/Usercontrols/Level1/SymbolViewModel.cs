using System.ComponentModel;
using System.Runtime.CompilerServices;
using MemExchange.Client.UI.Annotations;
using MemExchange.Core.SharedDto.Level1;

namespace MemExchange.Client.UI.Usercontrols.Level1
{
    public class SymbolViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string symbol;
        private int bidQuantity;
        private int askQuantity;
        private string askPrice;
        private string bidPrice;

        public string Symbol
        {
            get { return symbol; }
            set
            {
                if (value == symbol)
                    return;

                symbol = value;
                OnPropertyChanged();
            }
        }
        public int BidQuantity
        {
            get { return bidQuantity; }
            set
            {
                if (bidQuantity == value)
                    return;

                bidQuantity = value;
                OnPropertyChanged();
            }
        }
        public int AskQuantity
        {
            get { return askQuantity; }
            set
            {
                if (askQuantity == value)
                    return;

                askQuantity = value;
                OnPropertyChanged();
            }
        }
        public string BidPrice
        {
            get { return bidPrice; }
            set
            {
                if (value == bidPrice)
                    return;

                bidPrice = value;
                OnPropertyChanged();
            }
        }
        public string AskPrice
        {
            get { return askPrice; }
            set
            {
                if (askPrice == value)
                    return;

                askPrice = value;
                OnPropertyChanged();
            }
        }

        public SymbolViewModel(string symbol)
        {
            this.symbol = symbol;
            bidPrice = "-";
            askPrice = "-";
            bidQuantity = 0;
            askQuantity = 0;
        }
        

        public void Update(MarketBestBidAskDto marketBestBidAskDto)
        {
            if (marketBestBidAskDto.BestBidPrice.HasValue)
                BidPrice = marketBestBidAskDto.BestBidPrice.Value.ToString("N4");
            else
                BidPrice = "-";

            if (marketBestBidAskDto.BestAskPrice.HasValue)
                AskPrice = marketBestBidAskDto.BestAskPrice.Value.ToString("N4");
            else
                AskPrice = "-";

            BidQuantity = marketBestBidAskDto.BestBidQuantity;
            AskQuantity = marketBestBidAskDto.BestAskQuantity;
        }



        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
