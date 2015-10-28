using System;
using ProtoBuf;

namespace MemExchange.Core.SharedDto.Orders
{
    [ProtoContract]
    public class LimitOrderDto
    {
        [ProtoMember(1)]
        public string Symbol { get; set; }

        [ProtoMember(2)]
        public int Quantity { get; set; }

        [ProtoMember(3)]
        public double Price { get; set; }

        [ProtoMember(4)]
        public WayEnum Way { get; set; }
        
        [ProtoMember(5)]
        public uint ExchangeOrderId { get; set; }

        [ProtoMember(6)]
        public int ClientId { get; set; }

        public void Reeset()
        {
            Symbol = String.Empty;
            Quantity = 0;
            Price = 0;
            Way = WayEnum.NotSet;
            ExchangeOrderId = 0;
            ClientId = 0;
        }

        public bool ValidatesForAdd()
        {
            if (string.IsNullOrEmpty(Symbol))
                return false;

            if (Quantity <= 0)
                return false;

            if (Price <= 0)
                return false;

            if (ClientId <= 0)
                return false;

            if (Way == WayEnum.NotSet)
                return false;

            return true;
        }

        public bool ValidateForDelete()
        {
            if (ClientId <= 0)
                return false;

            if (ExchangeOrderId <= 0)
                return false;

            return true;
        }

        public bool ValidatesForModify()
        {
            if (Quantity <= 0)
                return false;

            if (Price <= 0)
                return false;

            if (ClientId <= 0)
                return false;

            if (ExchangeOrderId <= 0)
                return false;

            return true;
        }

        public void Update(LimitOrderDto other)
        {
            Symbol = other.Symbol;
            Quantity = other.Quantity;
            Price = other.Price;
            Way = other.Way;
            ExchangeOrderId = other.ExchangeOrderId;
            ClientId = other.ClientId;
        }
    }
}
