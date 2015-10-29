# MemExchange
C# in-memory stock exchange. Client-Server architecture.

This is a simple server-client library that simulates a stock exchange.
The exchange supports market order, limit orders and stop-limit orders.
Theres is no authentication or other security measures build in. It's made solely for research, paper-trade and showcase purposes.

Project state: Early beta still in development.

Project website (blog): http://www.thomasvestergaard.com/blog/memexchange/what-is-memexchange/

Running locally (blog): http://thomasvestergaard.com/blog/memexchange/getting-started-with-memexchange/

#High level architechture.
![alt tag](http://thomasvestergaard.com/media/1010/memexchange_high_level_architechture.jpg)

#Client side features
- Client should be able to post orders (Limit, stop-limit, market - Limit order is done)
- Client should be able to modify orders (Limit, stop-limit - Limit order is done)
- Client should be able to delete orders (Limit, stop-limit - Limit order is done)
- Client should be able to reqeust snapshop of open orders (done)
- Client should receive updates to orders when they are either modified, deleted, added (done)
- Client should receive executions on own orders. (done)
- Client should receive level 1 order book data (bid/ask updates, aggregated level 1 quantities and trades made) (done)

#Notes about order matching algorithms
- Limit orders are matched on best possible price. Example: If a buy side limit order comes in at a price of $10 and the only sell liquidity is available at $9, the order will be matched in the middle at $9.5 giving both parties a better execution.

- Stop-limit has two prices: The trigger price and the limit price. When the trigger price is touched or penetrated a limit order is placed. Buy stop-limit orders are always placed above the market with a trigger price lower than the limit price. Vice versa for sell stop-limit orders.

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
