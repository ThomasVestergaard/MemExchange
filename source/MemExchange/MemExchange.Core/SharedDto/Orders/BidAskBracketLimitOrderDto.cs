using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace MemExchange.Core.SharedDto.Orders
{
    [ProtoContract]
    public class BidAskBracketLimitOrderDto
    {
        [ProtoMember(1)]
        public string Symbol { get; set; }

        [ProtoMember(2)]
        public int AskQuantity { get; set; }

        [ProtoMember(3)]
        public double AskPrice { get; set; }

        [ProtoMember(4)]
        public double BidPrice { get; set; }

        [ProtoMember(5)]
        public double BidQuantity { get; set; }

        public void Reset()
        {
            Symbol = string.Empty;
            AskQuantity = 0;
            AskPrice = 0;
            BidPrice = 0;
            BidQuantity = 0;
        }

        public void Update(BidAskBracketLimitOrderDto other)
        {
            Symbol = other.Symbol;
            AskQuantity = other.AskQuantity;
            AskPrice = other.AskPrice;
            BidPrice = other.BidPrice;
            BidQuantity = other.BidQuantity;
        }


    }
}
