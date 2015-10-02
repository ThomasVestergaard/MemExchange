using System.Collections.Generic;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Clients;

namespace MemExchange.Server.Processor
{
    public interface IOrderKeep
    {
        /// <summary>
        /// key in inner dictionary is order id of limit order
        /// </summary>
        Dictionary<IClient, Dictionary<uint, LimitOrder>> ClientLimitOrders { get; }

        LimitOrder AddLimitOrder(LimitOrder limitOrder);
        LimitOrder TryUpdateLimitOrder(LimitOrder limitOrder);
        bool DeleteLimitOrder(LimitOrder limitOrder);
    }
}
