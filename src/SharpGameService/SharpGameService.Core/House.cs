﻿using Microsoft.Extensions.Options;
using SharpGameService.Core.Configuration;
using SharpGameService.Core.Exceptions;
using SharpGameService.Core.Interfaces;
using SharpGameService.Core.Models;
using System.Net.WebSockets;

namespace SharpGameService.Core
{
    /// <summary>
    /// Basic house implementation.
    /// </summary>
    public sealed class House<TRoomType>(IOptions<SharpGameServiceOptions> options) : IHouse where TRoomType : BaseRoom, new()
    {
        private readonly uint _maxPlayersPerRoom = options.Value.Rooms.MaxPlayersPerRoom;
        private readonly uint _maxRooms = options.Value.House.MaxRooms;
        private readonly uint _maxMessageSize = options.Value.MaxMessageSizeKb;
        private readonly bool _closeRoomsOnEmpty = options.Value.Rooms.CloseRoomsOnEmpty;
        private readonly TimeSpan? _closeWaitTime = options.Value.Rooms.CloseWaitTime;

        private IList<TRoomType> _rooms = new List<TRoomType>();

        /// <inheritdoc />
        public void CreateRoom(string roomId, string roomCode)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("Room Id must be populated");
            }

            if (_rooms.Count >= _maxRooms)
            {
                throw new HouseFullException();
            }

            var newRoom = new TRoomType();
            newRoom.Initialise(roomId, roomCode, _maxPlayersPerRoom, _closeRoomsOnEmpty, _closeWaitTime);
            _rooms.Add(newRoom);
        }

        /// <inheritdoc />
        public bool DoesRoomExist(string roomId)
        {
            return _rooms.Any(x => x.Id == roomId);
        }

        /// <inheritdoc />
        public void Join(string roomId, string code, string playerName, WebSocket connection)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("Room Id must be populated");
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentException("Player Name must be populated");
            }

            if (!DoesRoomExist(roomId))
            {
                throw new RoomNotFoundException();
            }

            var room = _rooms.SingleOrDefault(x => x.Id == roomId && x.Code == code) ?? throw new InvalidRoomCodeException();

            var id = Guid.NewGuid().ToString();
            room.Join(playerName, id, connection);
        }

        /// <inheritdoc />
        public async Task ProcessAsync()
        {
            foreach(var room in _rooms)
            {
                await room.Process();
            }
        }

        /// <inheritdoc />
        public RoomMetadata GetRoomMetadata(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("Room Id must be populated");
            }

            if (!DoesRoomExist(roomId))
            {
                throw new RoomNotFoundException();
            }

            var room = _rooms.Single(x => x.Id == roomId);
            return new RoomMetadata
            {
                MaxPlayers = room.MaxPlayers,
                CurrentPlayers = room.CurrentPlayers,
            };
        }

        /// <inheritdoc />
        public async Task CloseHouse()
        {
            foreach (var room in _rooms)
            {
                await room.Close();
            }
        }

        /// <inheritdoc />
        public void MessageReceived(string roomId, string message)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("Room Id must be populated");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message must be populated");
            }

            if (!DoesRoomExist(roomId))
            {
                throw new RoomNotFoundException();
            }
            
            var room = _rooms.Single(x => x.Id == roomId);

            room.HandleReceivedMessage(message);
        }
    }
}
