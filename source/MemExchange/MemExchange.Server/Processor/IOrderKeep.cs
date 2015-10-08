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

        void AddLimitOrder(LimitOrder limitOrder, out LimitOrder addedOrder);
        bool TryUpdateLimitOrder(LimitOrder limitOrder, out LimitOrder modifiedOrder);
        bool DeleteLimitOrder(LimitOrder limitOrder);
    }
}
