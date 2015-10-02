using System.Collections.Generic;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Clients;

namespace MemExchange.Server.Processor
{
    public class OrderKeep : IOrderKeep
    {
        private readonly IClientRepository clientRepository;
        public Dictionary<IClient, Dictionary<uint, LimitOrder>> ClientLimitOrders { get; private set; }
        private uint orderSequenceId { get; set; }

        public OrderKeep(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
            orderSequenceId = 1;
            ClientLimitOrders = new Dictionary<IClient, Dictionary<uint, LimitOrder>>();
        }

        public LimitOrder AddLimitOrder(LimitOrder limitOrder)
        {
            var client = clientRepository.GetOrAddClientFromId(limitOrder.ClientId);
            if (!ClientLimitOrders.ContainsKey(client))
                ClientLimitOrders.Add(client, new Dictionary<uint, LimitOrder>());
            
            ClientLimitOrders[client].Add(orderSequenceId, limitOrder);
            limitOrder.ExchangeOrderId = orderSequenceId;
            orderSequenceId++;
            
            return limitOrder;
        }

        public LimitOrder TryUpdateLimitOrder(LimitOrder limitOrder)
        {
            if (limitOrder.ExchangeOrderId <= 0)
                return null;

            var client = clientRepository.GetOrAddClientFromId(limitOrder.ClientId);
            if (!ClientLimitOrders.ContainsKey(client))
                return null;

            if (!ClientLimitOrders[client].ContainsKey(limitOrder.ExchangeOrderId))
                return null;

            var order = ClientLimitOrders[client][limitOrder.ExchangeOrderId];
            order.Price = limitOrder.Price;
            order.Quantity = limitOrder.Quantity;

            return order;
        }

        public bool DeleteLimitOrder(LimitOrder limitOrder)
        {
            if (limitOrder.ExchangeOrderId <= 0)
                return false;

            var client = clientRepository.GetOrAddClientFromId(limitOrder.ClientId);
            if (!ClientLimitOrders.ContainsKey(client))
                return false;
            
            return ClientLimitOrders[client].Remove(limitOrder.ExchangeOrderId);
        }
    }
}