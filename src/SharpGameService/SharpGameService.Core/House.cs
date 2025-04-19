using Microsoft.Extensions.Options;
using SharpGameService.Core.Interfaces;
using System.Net.WebSockets;

namespace SharpGameService.Core
{
    /// <summary>
    /// Basic house implementation.
    /// </summary>
    public sealed class House<TRoomType>(uint maxPlayersPerRoom, uint maxRooms, int maxMessageSize) : IHouse where TRoomType : BaseRoom, new()
    {


        private IList<TRoomType> _rooms = new List<TRoomType>();

        public void CreateRoom(string roomId, string roomCode)
        {
            throw new NotImplementedException();
        }

        public bool DoesRoomExist(string roomId)
        {
            throw new NotImplementedException();
        }

        public void Initialise(bool initialiseRoom, int maxRooms)
        {
            throw new NotImplementedException();
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
