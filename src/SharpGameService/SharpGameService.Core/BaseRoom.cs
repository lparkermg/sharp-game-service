using SharpGameService.Core.Events;
using SharpGameService.Core.Exceptions;
using SharpGameService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Core
{
    public class BaseRoom : IRoom
    {
        public uint MaxPlayers { get; private set; }

        public uint CurrentPlayers => (uint)_connections.Count;

        public string Id { get; private set; }

        public string Code { get; private set; }

        public bool RoomClosing { get; private set; } = false;

        // TODO: Have a model for that includes the player details and connection.
        private IList<WebSocket> _connections = new List<WebSocket>();

        private bool _closeOnEmpty = false;
        private TimeSpan _closeWaitTime;

        private bool _isInitialised = false;

        private DateTime? _closeConnectionTime = null;

        public event OnMessageReceivedEventHandler OnMessageReceived;
        public event OnPlayerJoinedEventHandler OnPlayerJoined;
        public event OnPlayerDisconnectedEventHandler OnPlayerDisconnected;

        public delegate void OnMessageReceivedEventHandler(object sender, OnMessageReceivedEventArgs data);
        public delegate void OnPlayerJoinedEventHandler(object sender, OnPlayerJoinedEventArgs data);
        public delegate void OnPlayerDisconnectedEventHandler(object sender, OnPlayerDisconnectedEventArgs data);

        public void Initialise(string roomId, string roomCode, uint maxPlayers, bool closeOnEmpty, TimeSpan? closeWaitTime = null)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("Room Id must be populated");
            }

            if(maxPlayers == 0)
            {
                throw new ArgumentException("Max players must be greater than 0");
            }

            Id = roomId;
            Code = roomCode;
            MaxPlayers = maxPlayers;
            

            _closeOnEmpty = closeOnEmpty;
            _closeWaitTime = closeWaitTime ?? TimeSpan.Zero;
            _isInitialised = true;
        }

        // TODO: Change to include something like player details?
        // TODO: Include room code to try and join the room.
        public void Join(WebSocket connection)
        {
            if (!_isInitialised)
            {
                throw new InvalidOperationException("The room has not been initialised");
            }

            if (_connections.Count >= MaxPlayers)
            {
                throw new RoomFullException();
            }

            _connections.Add(connection);

            // TODO: Setup data object based on player details sent with connection or the game will handle any details.
            OnPlayerJoined?.Invoke(this, new OnPlayerJoinedEventArgs(new object()));
        }

        public Task Process()
        {
            if (!_isInitialised)
            {
                throw new InvalidOperationException("The room has not been initialised");
            }

            var closedConnections = _connections
                .Where(c => c.CloseStatus.HasValue || c.State == WebSocketState.Closed)
                .ToList();

            foreach(var connection in closedConnections)
            {
                _connections.Remove(connection);
                OnPlayerDisconnected?.Invoke(this, new OnPlayerDisconnectedEventArgs(string.Empty));
            }

            if (_closeOnEmpty && CurrentPlayers == 0)
            {
                if (_closeConnectionTime == null)
                {
                    // Set the time to close the room
                    _closeConnectionTime = DateTime.UtcNow.Add(_closeWaitTime);
                }

                if (_closeConnectionTime.HasValue && _closeConnectionTime.Value <= DateTime.UtcNow)
                {
                    RoomClosing = true;
                }
            }

            return Task.CompletedTask;
        }

        public void HandleReceivedMessage(object data)
        {
            if (!_isInitialised)
            {
                throw new InvalidOperationException("The room has not been initialised");
            }

            OnMessageReceived?.Invoke(this, new OnMessageReceivedEventArgs(data));
        }
    }
}
