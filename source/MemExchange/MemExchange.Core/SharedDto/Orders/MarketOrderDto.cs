using ProtoBuf;

namespace MemExchange.Core.SharedDto.Orders
{
    [ProtoContract]
    public class MarketOrderDto
    {
        [ProtoMember(1)]
        public string Symbol { get; set; }

        [ProtoMember(2)]
        public int Quantity { get; set; }

        [ProtoMember(4)]
        public WayEnum Way { get; set; }

        public void Update(MarketOrderDto other)
        {
            Symbol = other.Symbol;
            Quantity = other.Quantity;
            Way = other.Way;
        }

        public void Reset()
        {
            Way = WayEnum.NotSet;
            Quantity = 0;
            Symbol = string.Empty;
        }

    }
}
