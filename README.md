# MemExchange
C# in-memory stock exchange. Client-Server architecture.

This is a simple server-client library that simulates a stock exchange.
The exchange supports market order, limit orders and stop-limit orders.
Theres is no authentication or other security measures build in. It's made solely for research, paper-trade and showcase purposes.

Project state: Early beta still in development.

Project website (blog): http://www.thomasvestergaard.com/blog/memexchange/what-is-memexchange/

High level architechture.
![alt tag](http://thomasvestergaard.com/media/1010/memexchange_high_level_architechture.jpg)


The project utilizes the following 3rd party libraries. All from nuget.

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
