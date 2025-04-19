using Microsoft.Extensions.Options;
using SharpGameService.Core.Exceptions;
using SharpGameService.Core.Interfaces;
using System.Net.WebSockets;

namespace SharpGameService.Core
{
    /// <summary>
    /// Basic house implementation.
    /// </summary>
    public sealed class House<TRoomType>(uint maxPlayersPerRoom, uint maxRooms, uint maxMessageSize, bool closeRoomsOnEmpty, TimeSpan? closeWaitTime) : IHouse where TRoomType : BaseRoom, new()
    {
        private readonly uint _maxPlayersPerRoom = maxPlayersPerRoom;
        private readonly uint _maxRooms = maxRooms;
        private readonly uint _maxMessageSize = maxMessageSize;
        private readonly bool _closeRoomsOnEmpty = closeRoomsOnEmpty;
        private readonly TimeSpan? _closeWaitTime = closeWaitTime;

        private IList<TRoomType> _rooms = new List<TRoomType>();

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

        public bool DoesRoomExist(string roomId)
        {
            return _rooms.Any(x => x.Id == roomId);
        }

        public void Join(string roomdId, string code, WebSocket connection)
        {
            throw new NotImplementedException();
        }

        public Task ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }
}
