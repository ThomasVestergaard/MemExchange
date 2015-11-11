using System;
using System.Collections.Generic;
using MemExchange.ClientApi.Commands;
using MemExchange.ClientApi.Stream;
using MemExchange.Core.SharedDto;
using MemExchange.Core.SharedDto.ClientToServer;
using MemExchange.Core.SharedDto.Level1;
using MemExchange.Core.SharedDto.Orders;
using MemExchange.Core.SharedDto.ServerToClient;

namespace MemExchange.ClientApi
{
    public class Client : IClient
    {
        public event EventHandler<LimitOrderDto> LimitOrderAccepted;
        public event EventHandler<LimitOrderDto> LimitOrderChanged;
        public event EventHandler<LimitOrderDto> LimitOrderDeleted;
        public event EventHandler<List<LimitOrderDto>> LimitOrderSnapshot;
        public event EventHandler<ExecutionDto> NewExecution;
        public event EventHandler<MarketBestBidAskDto> Level1Updated;
        public event EventHandler<StopLimitOrderDto> StopLimitOrderAccepted;
        public event EventHandler<StopLimitOrderDto> StopLimitOrderChanged;
        public event EventHandler<StopLimitOrderDto> StopLimitOrderDeleted;
        public event EventHandler<List<StopLimitOrderDto>> StopLimitOrderSnapshot;

        private IMessageConnection messageConnection;
        private readonly IServerMessageSubscriber subscriber;
        private int clientId ;
        private bool isStarted;

        public Client(IMessageConnection messageConnection, IServerMessageSubscriber subscriber)
        {
            this.messageConnection = messageConnection;
            this.subscriber = subscriber;
            isStarted = false;
        }

        public void Start(int clientId, string serverAddress, int serverCommandPort, int serverPublishPort)
        {
            this.clientId = clientId;
            subscriber.Start(serverAddress, serverPublishPort, clientId, HandleServerMessage);
            messageConnection.Start(serverAddress, serverCommandPort);
            isStarted = true;
        }

        private void HandleServerMessage(ServerToClientMessage message)
        {
            switch (message.MessageType)
            {
                case ServerToClientMessageTypeEnum.StopLimitOrderSnapshot:
                    EventHandler<List<StopLimitOrderDto>> stopLimitOrderSnapshotHandler = StopLimitOrderSnapshot;
                    if (stopLimitOrderSnapshotHandler != null)
                        stopLimitOrderSnapshotHandler(this, message.StopLimitOrderList);
                    break;

                case ServerToClientMessageTypeEnum.StopLimitOrderAccepted:
                    EventHandler<StopLimitOrderDto> stopLimitOrderAcceptedHandler = StopLimitOrderAccepted;
                    if (stopLimitOrderAcceptedHandler != null)
                        stopLimitOrderAcceptedHandler(this, message.StopLimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.StopLimitOrderChanged:
                    EventHandler<StopLimitOrderDto> stopLimitOrderChangedHandler = StopLimitOrderChanged;
                    if (stopLimitOrderChangedHandler != null)
                        stopLimitOrderChangedHandler(this, message.StopLimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.StopLimitOrderDeleted:
                    EventHandler<StopLimitOrderDto> stopLimitOrderDeletedHandler = StopLimitOrderDeleted;
                    if (stopLimitOrderDeletedHandler != null)
                        stopLimitOrderDeletedHandler(this, message.StopLimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.LimitOrderAccepted:
                    EventHandler<LimitOrderDto> acceptedHandler = LimitOrderAccepted;
                    if (acceptedHandler != null)
                        acceptedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.LimitOrderDeleted:
                    EventHandler<LimitOrderDto> deletedHandler = LimitOrderDeleted;
                    if (deletedHandler != null)
                        deletedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.LimitOrderChanged:
                    EventHandler<LimitOrderDto> changedHandler = LimitOrderChanged;
                    if (changedHandler != null)
                        changedHandler(this, message.LimitOrder);
                    break;

                case ServerToClientMessageTypeEnum.LimitOrderSnapshot:
                    EventHandler<List<LimitOrderDto>> snapshotHandler = LimitOrderSnapshot;
                    if (snapshotHandler != null)
                        snapshotHandler(this, message.LimitOrderList);
                    break;

                case ServerToClientMessageTypeEnum.Execution:
                    EventHandler<ExecutionDto> executionHandler = NewExecution;
                    if (executionHandler != null)
                        executionHandler(this, message.Execution);
                    break;

                case ServerToClientMessageTypeEnum.Level1:
                    EventHandler<MarketBestBidAskDto> level1Handler = Level1Updated;
                    if (level1Handler != null)
                        level1Handler(this, message.Level1);
                    break;
            }
        }

        public void Stop()
        {
            isStarted = false;
            subscriber.Stop();
            messageConnection.Stop();
        }

        public void SubmitStopLimitOrder(string symbol, double triggerPrice, double limitPrice, int quantity, WayEnum way)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                StopLimitOrder = new StopLimitOrderDto
                {
                    Symbol = symbol,
                    Quantity = quantity,
                    Way = way,
                    TriggerPrice = triggerPrice,
                    LimitPrice = limitPrice,
                    ClientId = clientId

                },
                MessageType = ClientToServerMessageTypeEnum.PlaceStopLimitOrder
            });
        }

        public void CancelStopLimitOrder(uint exchangeOrderId)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                StopLimitOrder = new StopLimitOrderDto
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                },
                MessageType = ClientToServerMessageTypeEnum.CancelStopLimitOrder
            });
        }

        public void SubmitMarketOrder(string symbol, int quantity, WayEnum way)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                MarketOrder = new MarketOrderDto
                {
                    Quantity = quantity,
                    Symbol = symbol,
                    Way = way
                },
                MessageType = ClientToServerMessageTypeEnum.PlaceMarketOrder
            });
        }

        public void SubmitLimitOrder(string symbol, double price, int quantity, WayEnum way)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrderDto
                {
                    ClientId = clientId,
                    Price = price,
                    Quantity = quantity,
                    Symbol = symbol,
                    Way = way
                },
                MessageType = ClientToServerMessageTypeEnum.PlaceLimitOrder
            });
        }

        public void ModifyLimitOrder(uint exchangeOrderId, double newPrice, int newQuantity)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrderDto
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                    Price = newPrice,
                    Quantity = newQuantity
                },
                MessageType = ClientToServerMessageTypeEnum.ModifyLimitOrder
            });
        }

        public void ModifyStopLimitOrder(uint exchangeOrderId, double newTriggerPrice, double newLimitPrice, int newQuantity)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                StopLimitOrder = new StopLimitOrderDto()
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                    LimitPrice = newLimitPrice,
                    TriggerPrice = newTriggerPrice,
                    Quantity = newQuantity
                },
                MessageType = ClientToServerMessageTypeEnum.ModifyStopLimitOrder
            });
        }

        public void CancelLimitOrder(uint exchangeOrderId)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                LimitOrder = new LimitOrderDto
                {
                    ClientId = clientId,
                    ExchangeOrderId = exchangeOrderId,
                },
                MessageType = ClientToServerMessageTypeEnum.CancelLimitOrder
            });
        }

        public void RequestOpenLimitOrders()
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                MessageType = ClientToServerMessageTypeEnum.RequestOpenLimitOrders
            });
        }

        public void RequestOpenStopLimitOrders()
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                MessageType = ClientToServerMessageTypeEnum.RequestOpenStopLimitOrders
            });
        }

        public void ModifyDuoLimitOrders(uint order1OrderId, double order1NewPrice, int order1NewQuantity, uint order2OrderId, double order2NewPrice, int order2NewQuantity)
        {
            if (!isStarted)
                return;

            messageConnection.SendMessage(new ClientToServerMessage
            {
                ClientId = clientId,
                DuoLimitOrder = new DuoLimitOrderDto
                {
                    ClientId = clientId,
                    LimitOrder1 = new LimitOrderDto
                    {
                        ClientId = clientId,
                        ExchangeOrderId = order1OrderId,
                        Price = order1NewPrice,
                        Quantity = order1NewQuantity
                    },
                    LimitOrder2 = new LimitOrderDto
                    {
                        ClientId = clientId,
                        ExchangeOrderId = order2OrderId,
                        Price = order2NewPrice,
                        Quantity = order2NewQuantity
                    }
                },
                MessageType = ClientToServerMessageTypeEnum.DuoLimitOrderUpdate
            });
        }
    }
}