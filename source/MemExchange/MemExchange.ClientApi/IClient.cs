using System;
using System.Collections.Generic;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.Level1;
using MemExchange.Core.SharedDto.Orders;

namespace MemExchange.ClientApi
{
    public interface IClient
    {
        event EventHandler<LimitOrderDto> LimitOrderAccepted;
        event EventHandler<LimitOrderDto> LimitOrderChanged;
        event EventHandler<LimitOrderDto> LimitOrderDeleted;
        event EventHandler<List<LimitOrderDto>> LimitOrderSnapshot;
        event EventHandler<ExecutionDto> NewExecution;
        event EventHandler<MarketBestBidAskDto> Level1Updated;
        event EventHandler<StopLimitOrderDto> StopLimitOrderAccepted;
        event EventHandler<StopLimitOrderDto> StopLimitOrderChanged;
        event EventHandler<StopLimitOrderDto> StopLimitOrderDeleted;
        event EventHandler<List<StopLimitOrderDto>> StopLimitOrderSnapshot; 

        void Start(int clientId, string serverAddress, int serverCommandPort, int serverPublishPort);
        void Stop();

        void SubmitStopLimitOrder(string symbol, double triggerPrice, double limitPrice, int quantity, WayEnum way);
        void CancelStopLimitOrder(uint exchangeOrderId);

        void SubmitLimitOrder(string symbol, double price, int quantity, WayEnum way);
        void SubmitMarketOrder(string symbol, int quantity, WayEnum way);
        void ModifyLimitOrder(uint exchangeOrderId, double newPrice, int newQuantity);
        void CancelLimitOrder(uint exchangeOrderId);
        void RequestOpenLimitOrders();
        void RequestOpenStopLimitOrders();
    }
}
