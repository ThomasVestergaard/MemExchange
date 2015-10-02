# MemExchange
C# in-memory stock exchange. Client-Server architecture.

This is a simple server-client library that simulates a stock exchange.
The exchange supports market order, limit orders and stop-limit orders.
Theres is no authentication or other security measures build in. It's made solely for research, paper-trade and showcase purposes.

Project state: Early beta still in development.

The project utilizes the following 3rd party libraries. All from nuget.

Castle Windsor for dependency injection
Disruptor.net for cross thread communication
NetMQ for network communication
NUnit for unit testing
Rhinomocks for mocking
Protobuf for zerializing
Serilog for logging
Topshelf for windows services
