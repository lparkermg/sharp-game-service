# SharpGameService Core

This contains the core library for SharpGameService, a C# library made to help you create game servers, include the hosting the game logic itself.

## Glossary

- `House` - A house represents a collection of rooms, which are the games that are being played. Each house can have multiple rooms, and each room can have multiple players.
- `Room` - A room represents a single game instance. Each room can have multiple players, and each player will send and receive messages to and from the room. A room is where the game logic will happen.

## Usage

To use the library you will need to implement the following interfaces to handle your own game and service logic:

- `BaseRoom` - Inheriting from this will allow you to have the connection and messaging in SharpGameService.Core along with implementing your own server side game logic.
- A Hosted Service, you will need to inherit from something like BackgroundService or IHostedService and this will be the core to firing up the server, handling the auto processing and shutting it down.

Along with the implementation of the above, in your host setup and build you will need to add the following:

`builder.Services.AddSharpGameService<YourHostedService, YourRoom>();` - This will add the hosted service and the room to the DI container with default options (you are able to setup options using a lamda function).
 and after builder.Build().

 `app.UseSharpGameService(webSocketConfig);` - This will add the middleware to the pipeline, this is required for the WebSocket connections to be handled.
