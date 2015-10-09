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

        public void AddLimitOrder(LimitOrder limitOrder, out LimitOrder addedOrder)
        {
            addedOrder = new LimitOrder();
            var client = clientRepository.GetOrAddClientFromId(limitOrder.ClientId);
            if (!ClientLimitOrders.ContainsKey(client))
                ClientLimitOrders.Add(client, new Dictionary<uint, LimitOrder>());

            var collectionLimitOrder = new LimitOrder();
            collectionLimitOrder.Update(limitOrder);
            collectionLimitOrder.ExchangeOrderId = orderSequenceId;
            ClientLimitOrders[client].Add(orderSequenceId, collectionLimitOrder);
            
            addedOrder.Update(collectionLimitOrder);
            orderSequenceId++;
        }

        public bool TryUpdateLimitOrder(LimitOrder limitOrder, out LimitOrder modifiedOrder)
        {
            modifiedOrder = null;
            bool toReturn = true;

            if (limitOrder.ExchangeOrderId <= 0)
                return false;

            var client = clientRepository.GetOrAddClientFromId(limitOrder.ClientId);
            if (!ClientLimitOrders.ContainsKey(client))
                return false;

            if (!ClientLimitOrders[client].ContainsKey(limitOrder.ExchangeOrderId))
                return false;

            modifiedOrder = new LimitOrder();
            var order = ClientLimitOrders[client][limitOrder.ExchangeOrderId];
            order.Price = limitOrder.Price;
            order.Quantity = limitOrder.Quantity;
            modifiedOrder.Update(order);

            return true;
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

        public void GetClientOrders(int clientId, out List<LimitOrder> clientOrders)
        {
            clientOrders = new List<LimitOrder>();
            var client = clientRepository.GetOrAddClientFromId(clientId);
            if (!ClientLimitOrders.ContainsKey(client))
                return;

            clientOrders.AddRange(ClientLimitOrders[client].Values);
        }
    }
}