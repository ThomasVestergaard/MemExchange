using ProtoBuf;

namespace MemExchange.Core.SharedDto.Level1
{
    [ProtoContract]
    public class MarketBestBidAskDto
    {
        [ProtoMember(1)]
        public string Symbol { get; set; }
        [ProtoMember(2)]
        public double? BestBidPrice { get; set; }
        [ProtoMember(3)]
        public double? BestAskPrice { get; set; }
        [ProtoMember(4)]
        public int BestBidQuantity { get; set; }
        [ProtoMember(5)]
        public int BestAskQuantity { get; set; }

        public void Update(MarketBestBidAskDto other)
        {
            Symbol = other.Symbol;
            BestBidPrice = other.BestBidPrice;
            BestAskPrice = other.BestAskPrice;
            BestBidQuantity = other.BestBidQuantity;
            BestAskQuantity = other.BestAskQuantity;
        }
        
        public void Reset()
        {
            Symbol = "";
            BestBidPrice = null;
            BestAskPrice = null;
            BestBidQuantity = 0;
            BestAskQuantity = 0;
        }
    }
}
