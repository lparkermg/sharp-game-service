using SharpGameService.Core.Events;
using SharpGameService.Core.Exceptions;
using SharpGameService.Core.Interfaces;
using SharpGameService.Core.Messaging.DataModels;
using System.Net.WebSockets;

namespace SharpGameService.Core
{
    /// <summary>
    /// The base room implementation.
    /// 
    /// This handles the basic functionality of a room, such as joining, leaving and processing. Along with the non-specific validation needed for each.
    /// </summary>
    public abstract class BaseRoom : IRoom
    {
        /// <inheritdoc />
        public uint MaxPlayers { get; private set; }

        /// <inheritdoc />
        public uint CurrentPlayers => (uint)_connections.Count;

        /// <inheritdoc />
        public string Id { get; private set; }

        /// <inheritdoc />
        public string Code { get; private set; }

        /// <inheritdoc />
        public bool RoomClosing { get; private set; } = false;

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

        /// <inheritdoc />
        public virtual void Initialise(string roomId, string roomCode, uint maxPlayers, bool closeOnEmpty, TimeSpan? closeWaitTime = null)
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

        /// <inheritdoc />
        public virtual void Join(string playerName, string playerId, WebSocket connection)
        {
            if (!_isInitialised)
            {
                throw new InvalidOperationException("The room has not been initialised");
            }

            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("Player Id must be populated");
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentException("Player Name must be populated");
            }

            if (_connections.Count >= MaxPlayers)
            {
                throw new RoomFullException();
            }

            _connections.Add(connection);

            OnPlayerJoined?.Invoke(this, new OnPlayerJoinedEventArgs(new PlayerJoinedModel { PlayerId = playerId, PlayerName = playerName}));
        }

        /// <inheritdoc />
        public virtual Task Process()
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

        public virtual async Task Close()
        {
            if (!_isInitialised)
            {
                throw new InvalidOperationException("The room has not been initialised");
            }

            foreach (var connection in _connections)
            {
                await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Room is being closed", CancellationToken.None);
            }
            _connections.Clear();
        }

        /// <inheritdoc />
        public virtual void HandleReceivedMessage(string data)
        {
            if (!_isInitialised)
            {
                throw new InvalidOperationException("The room has not been initialised");
            }

            OnMessageReceived?.Invoke(this, new OnMessageReceivedEventArgs(data));
        }
    }
}
