# MemExchange
C# in-memory stock exchange. Client-Server architecture.

This is a simple server-client library that simulates a stock exchange.
The exchange supports market order, limit orders and stop-limit orders.
Theres is no authentication or other security measures build in. It's made solely for research, paper-trade and showcase purposes.

#Project state: 
Feature complete beta is released. There are probably a few issues here and there which I hope to discover over time while using it. Once these have been idntified and fixed, a stable release 1.0 will be issued.

Project website (blog): http://www.thomasvestergaard.com/blog/memexchange/what-is-memexchange/
Running locally (blog): http://thomasvestergaard.com/blog/memexchange/getting-started-with-memexchange/

#Getting started

- Get the source code
- Run the MemExchange.Server project or install it as a windows service
- Find a way to execute this code:

```
using MemExchange.ClientApi;
using MemExchange.Core.SharedDto;

// Instantiate dependencies
var logger = new SerilogLogger();
var serializer = new ProtobufSerializer();
var connection = new MessageConnection(logger, serializer);
var messageSubscriber = new ServerMessageSubscriber(logger, serializer);

// Create the client
var exchangeClient = new ClientApi.Client(connection, messageSubscriber);

// Hook up to some events
exchangeClient.Level1Updated += (sender, dto) =>
{
	Console.WriteLine("Level 1 update: {0}, Best bid: {1}, Best ask: {2}", dto.Symbol, dto.BestBidPrice, dto.BestAskPrice);
};

exchangeClient.LimitOrderAccepted += (sender, dto) =>
{
	Console.WriteLine("Limit order accepted.");
};

// Start and connecto the the server. Unless you changed something in the server, these port numbers should not be changed.
// 42 is your client id.
exchangeClient.Start(42, "localhost", 9192, 9193);

Console.WriteLine("Client started. Hit any key to submit order.");
Console.ReadKey();

// Submit a buy limit order for symbol AAPL, with e price of 50 on quantity 100
exchangeClient.SubmitLimitOrder("APPL", 50, 100, WayEnum.Buy);

Console.WriteLine("Hit any key to stop.");
Console.ReadKey();

// Stop the client
exchangeClient.Stop();

```



#High level architechture.
![alt tag](http://thomasvestergaard.com/media/1010/memexchange_high_level_architechture.jpg)

#Client side features
- Client should be able to post orders (Limit, stop-limit, market - done
- Client should be able to modify orders (done - Market orders cannot be modified)
- Client should be able to delete orders (done)
- Client should be able to reqeust snapshop of open orders (done)
- Client should receive updates to orders when they are either modified, deleted, added (done)
- Client should receive executions on own orders. (done)
- Client should receive level 1 order book data (bid/ask updates, aggregated level 1 quantities and trades made) (done)

#Notes about order matching algorithms
- Limit orders are matched on best possible price. Example: If a buy side limit order comes in at a price of $10 and the only sell liquidity is available at $9, the order will be matched in the middle at $9.5 giving both parties a better execution.

- Stop-limit has two prices: The trigger price and the limit price. When the trigger price is touched or penetrated a limit order is placed. Buy stop-limit orders are always placed above the market with a trigger price lower than the limit price. Vice versa for sell stop-limit orders.

- Market orders will execute it's way through the order book until either filled or no more liquidity is available. The market order will be cancelled after matching is complete. This is also true if the order is partially filled and liquidity runs dry or if there is no liquidity available at all.

#3rd party libraries
- Castle Windsor for dependency injection
- Disruptor.net for concurrency
- NetMQ for network communication
- NUnit for unit testing
- Rhinomocks for mocking
- Protobuf for serializing
- Serilog for logging
- Topshelf for windows services

#License
See the [LICENSE](https://github.com/ThomasVestergaard/MemExchange/blob/master/LICENSE.md) file for license rights and limitations (MIT).
