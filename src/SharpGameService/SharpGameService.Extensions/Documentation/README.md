# SharpGameService Extensions

This repository contains a collection of extensions for the SharpGameService library, which is a C# library designed to facilitate the development of game server management applications.
The extensions provided in this repository aim to enhance the functionality and usability of the SharpGameService library, making it easier for developers to create robust and feature-rich game server management solutions.

## Extensions Overview

`AddSharpGameService` is an extension method for the `IServiceCollection` interface. It configures the options, the implemented hosted service and the House that will be used.

`UseSharpGameService` is an extension method for the `IApplicationBuilder` interface. It configures the middleware that will be used to handle requests and responses in the application, currently this is `UseWebSockets`.