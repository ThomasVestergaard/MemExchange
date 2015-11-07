using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Server.Processor.Book.Orders;
using MemExchange.Server.Processor.Book.Triggers;

namespace MemExchange.Server.Processor.Book
{
    public class OrderRepository : IOrderRepository
    {
        private uint globalItemCounter { get; set; }

        private Dictionary<uint, ILimitOrder> LimitOrders { get; set; }
        private Dictionary<int, List<ILimitOrder>> ClientOrders { get; set; }

        private Dictionary<uint, IStopLimitOrder> StopLimitOrders { get; set; }
        private Dictionary<int, List<IStopLimitOrder>> ClientStopLimitOrders { get; set; } 

        public OrderRepository()
        {
            globalItemCounter = 1;
            LimitOrders = new Dictionary<uint, ILimitOrder>();
            ClientOrders = new Dictionary<int, List<ILimitOrder>>();
            StopLimitOrders = new Dictionary<uint, IStopLimitOrder>();
            ClientStopLimitOrders = new Dictionary<int, List<IStopLimitOrder>>();
        }

        public IStopLimitOrder NewStopLimitOrder(StopLimitOrderDto dtoStopLimitOrder)
        {
            return NewStopLimitOrder(dtoStopLimitOrder.Symbol, dtoStopLimitOrder.ClientId, dtoStopLimitOrder.TriggerPrice, dtoStopLimitOrder.LimitPrice, dtoStopLimitOrder.Quantity, dtoStopLimitOrder.Way);
        }

        public IStopLimitOrder NewStopLimitOrder(string symbol, int clientId, double triggerPrice, double limitPrice, int quantity, WayEnum way)
        {
            if (!ClientStopLimitOrders.ContainsKey(clientId))
                ClientStopLimitOrders.Add(clientId, new EditableList<IStopLimitOrder>());

            var trigger = new BestPriceTrigger(symbol, triggerPrice, way);
            var toReturn = new StopLimitOrder(symbol, quantity, limitPrice, triggerPrice, way, clientId, trigger );
            
            toReturn.SetExchangeOrderId(globalItemCounter);
            StopLimitOrders.Add(globalItemCounter, toReturn);
            ClientStopLimitOrders[clientId].Add(toReturn);
            toReturn.RegisterOrderRepositoryDeleteHandler(HandleDeletedStopLimitOrder);

            globalItemCounter++;
            return toReturn;
        }

        public List<ILimitOrder> GetClientLimitOrders(int clientId)
        {
            if (!ClientOrders.ContainsKey(clientId))
                return new List<ILimitOrder>();

            return ClientOrders[clientId];
        }

        public ILimitOrder NewLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            return NewLimitOrder(stopLimitOrder.Symbol, stopLimitOrder.ClientId, stopLimitOrder.LimitPrice, stopLimitOrder.Quantity, stopLimitOrder.Way);
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
            toReturn.SetExchangeOrderId(globalItemCounter);
            
            LimitOrders.Add(toReturn.ExchangeOrderId, toReturn);
            ClientOrders[clientId].Add(toReturn);
            toReturn.RegisterDeleteNotificationHandler(HandleDeletedLimitOrder);
            toReturn.RegisterFilledNotification(HandleDeletedLimitOrder);

            globalItemCounter++;
            return toReturn;
        }

        public ILimitOrder TryGetLimitOrder(uint exchangeOrderId)
        {
            if (!LimitOrders.ContainsKey(exchangeOrderId))
                return null;

            return LimitOrders[exchangeOrderId];
        }

        public IStopLimitOrder TryGetStopLimitOrder(uint exchangeOrderId)
        {
            if (!StopLimitOrders.ContainsKey(exchangeOrderId))
                return null;

            return StopLimitOrders[exchangeOrderId];
        }

        public IMarketOrder NewMarketOrder(MarketOrderDto dtoMarketOrder)
        {
            return NewMarketOrder(dtoMarketOrder.Symbol, dtoMarketOrder.ClientId, dtoMarketOrder.Quantity, dtoMarketOrder.Way);
        }

        public IMarketOrder NewMarketOrder(string symbol, int clientId, int quantity, WayEnum way)
        {
            IMarketOrder toReturn = new MarketOrder(symbol, quantity, way, clientId);
            toReturn.SetExchangeOrderId(globalItemCounter);

            globalItemCounter++;
            return toReturn;
        }

        private void HandleDeletedStopLimitOrder(IStopLimitOrder stopLimitOrder)
        {
            stopLimitOrder.UnRegisterOrderRepositoryDeleteHandler();
            StopLimitOrders.Remove(stopLimitOrder.ExchangeOrderId);
            ClientStopLimitOrders[stopLimitOrder.ClientId].Remove(stopLimitOrder);
        }

        private void HandleDeletedLimitOrder(ILimitOrder order)
        {
            order.UnRegisterDeleteNotificationHandler(HandleDeletedLimitOrder);
            order.UnRegisterFilledNotification(HandleDeletedLimitOrder);
            LimitOrders.Remove(order.ExchangeOrderId);
            ClientOrders[order.ClientId].Remove(order);
        }

        public List<IStopLimitOrder> GetClientStopLimitOrders(int clientId)
        {
            if (!ClientStopLimitOrders.ContainsKey(clientId))
                return new List<IStopLimitOrder>();

            return ClientStopLimitOrders[clientId];
        }
    }
}