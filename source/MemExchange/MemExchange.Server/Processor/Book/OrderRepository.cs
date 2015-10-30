using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Book.Orders;

namespace MemExchange.Server.Processor.Book
{
    public class OrderRepository : IOrderRepository
    {
        private uint orderIdCounter { get; set; }

        private Dictionary<uint, ILimitOrder> LimitOrders { get; set; }
        private Dictionary<int, List<ILimitOrder>> ClientOrders { get; set; } 

        public OrderRepository()
        {
            orderIdCounter = 1;
            LimitOrders = new Dictionary<uint, ILimitOrder>();
            ClientOrders = new Dictionary<int, List<ILimitOrder>>();
        }

        public List<ILimitOrder> GetClientOrders(int clientId)
        {
            if (!ClientOrders.ContainsKey(clientId))
                return new List<ILimitOrder>();

            return ClientOrders[clientId];
        }

        public ILimitOrder NewLimitOrder(LimitOrderDto dtoLimitOrder)
        {
            return NewLimitOrder(dtoLimitOrder.Symbol, dtoLimitOrder.ClientId, dtoLimitOrder.Price, dtoLimitOrder.Quantity, dtoLimitOrder.Way);
        }

        public ILimitOrder NewLimitOrder(string symbol, int clientId, double price, int quantity, WayEnum way)
        {
            if (!ClientOrders.ContainsKey(clientId))
                ClientOrders.Add(clientId, new EditableList<ILimitOrder>());

            ILimitOrder toReturn = new LimitOrder(symbol, quantity, price, way, clientId);
            toReturn.SetExchangeOrderId(orderIdCounter);
            
            LimitOrders.Add(toReturn.ExchangeOrderId, toReturn);
            ClientOrders[clientId].Add(toReturn);
            toReturn.RegisterDeleteNotificationHandler(HandleOrderDeleted);
            toReturn.RegisterFilledNotification(HandleOrderDeleted);

            orderIdCounter++;
            return toReturn;
        }

        public ILimitOrder TryGetOrder(uint exchangeOrderId)
        {
            if (!LimitOrders.ContainsKey(exchangeOrderId))
                return null;

            return LimitOrders[exchangeOrderId];
        }



        private void HandleOrderDeleted(ILimitOrder order)
        {
            order.UnRegisterDeleteNotificationHandler(HandleOrderDeleted);
            order.UnRegisterFilledNotification(HandleOrderDeleted);
            LimitOrders.Remove(order.ExchangeOrderId);
            ClientOrders[order.ClientId].Remove(order);
        }
    }
}