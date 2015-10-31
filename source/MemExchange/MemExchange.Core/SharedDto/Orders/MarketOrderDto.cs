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

        [ProtoMember(3)]
        public WayEnum Way { get; set; }

        [ProtoMember(4)]
        public int ClientId { get; set; }

        public void Update(MarketOrderDto other)
        {
            Symbol = other.Symbol;
            Quantity = other.Quantity;
            Way = other.Way;
            ClientId = other.ClientId;
        }

        public void Reset()
        {
            Way = WayEnum.NotSet;
            Quantity = 0;
            Symbol = string.Empty;
            ClientId = 0;
        }

        public bool ValidateForExecute()
        {
            if (string.IsNullOrWhiteSpace(Symbol))
                return false;

            if (Quantity <= 0)
                return false;

            if (Way == WayEnum.NotSet)
                return false;

            return true;
        }

    }
}
