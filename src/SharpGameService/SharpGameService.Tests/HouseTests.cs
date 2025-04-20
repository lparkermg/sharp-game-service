using SharpGameService.Core;
using SharpGameService.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Tests
{
    public class HouseTests
    {
        private House<BaseRoom> _house;

        private MemoryStream _connectionStream;

        [SetUp]
        public void SetUp()
        {
            _house = new House<BaseRoom>(1, 1, 4, true, TimeSpan.FromSeconds(30));
            _connectionStream = new MemoryStream();
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void CreateRoom_GivenInvalidRoomId_ThrowsArgumentException(string? roomId)
        {
            string roomCode = "TestRoomCode";
            Assert.That(() => _house.CreateRoom(roomId, roomCode), Throws.ArgumentException.With.Message.EqualTo("Room Id must be populated"));
        }

        [Test]
        public void CreateRoom_GivenValidRoomId_CreatesRoom()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            _house.CreateRoom(roomId, roomCode);
            Assert.That(_house.DoesRoomExist(roomId), Is.True);
        }

        [Test]
        public void CreateRoom_GivenAlreadyMaxRooms_ThrowsMaxRoomsException()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            _house.CreateRoom(roomId, roomCode);
            Assert.That(() => _house.CreateRoom(roomId, roomCode), Throws.TypeOf<HouseFullException>().And.With.Message.EqualTo("The house you are trying to enter is full"));
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void DoesRoomExist_GivenInvalidRoomId_ReturnsFalse(string? roomId)
        {
            Assert.That(_house.DoesRoomExist(roomId), Is.False);
        }

        [Test]
        public void DoesRoomExist_GivenNonExistingRoomId_ReturnsFalse()
        {
            string roomId = "NonExistingRoomId";
            Assert.That(_house.DoesRoomExist(roomId), Is.False);
        }

        [Test]
        public void DoesRoomExist_GivenExistingRoomId_ReturnsTrue()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            _house.CreateRoom(roomId, roomCode);
            Assert.That(_house.DoesRoomExist(roomId), Is.True);
        }

        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        public void Join_GivenInvalidRoomId_ThrowsArgumentException(string? roomId)
        {
            string roomCode = "TestRoomCode";
            Assert.That(() => _house.Join(roomId, roomCode, CreateWebSocket()), Throws.ArgumentException.With.Message.EqualTo("Room Id must be populated"));
        }

        [Test]
        public void Join_GivenNonExistingRoomId_ThrowsRoomNotFoundException()
        {
            string roomId = "NonExistingRoomId";
            string roomCode = "TestRoomCode";
            Assert.That(() => _house.Join(roomId, roomCode, CreateWebSocket()), Throws.TypeOf<RoomNotFoundException>().And.With.Message.EqualTo("The room for the provided Id does not exist"));
        }

        [Test]
        public void Join_GivenExistingRoomIdWithWrongCode_ThrowsInvalidRoomCodeException()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            _house.CreateRoom(roomId, roomCode);
            Assert.That(() => _house.Join(roomId, "WrongCode", CreateWebSocket()), Throws.TypeOf<InvalidRoomCodeException>().And.With.Message.EqualTo("The room code provided is invalid"));
        }

        [Test]
        public void Join_GivenExistingRoomIdWithCorrectCode_AddsPlayerToRoom()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            _house.CreateRoom(roomId, roomCode);
            var playerWebSocket = CreateWebSocket();
            _house.Join(roomId, roomCode, playerWebSocket);
            Assert.That(_house.GetRoomMetadata(roomId).CurrentPlayers, Is.EqualTo(1));
        }

        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        public void GetRoomMetadata_GivenInvalidRoomId_ThrowsArgumentException(string? roomId)
        {
            Assert.That(() => _house.GetRoomMetadata(roomId), Throws.ArgumentException.With.Message.EqualTo("Room Id must be populated"));
        }

        [Test]
        public void GetRoomMetadata_GivenNonExistingRoomId_ThrowsRoomNotFoundException()
        {
            string roomId = "NonExistingRoomId";
            Assert.That(() => _house.GetRoomMetadata(roomId), Throws.TypeOf<RoomNotFoundException>().And.With.Message.EqualTo("The room for the provided Id does not exist"));
        }

        [Test]
        public void GetRoomMetadata_GivenExistingRoomId_ReturnsRoomMetadata()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            _house.CreateRoom(roomId, roomCode);
            var metadata = _house.GetRoomMetadata(roomId);

            Assert.Multiple(() =>
            {
                Assert.That(metadata.MaxPlayers, Is.EqualTo(1));
                Assert.That(metadata.CurrentPlayers, Is.EqualTo(0));
            });
        }

        private WebSocket CreateWebSocket()
        {
            return WebSocket.CreateFromStream(_connectionStream, false, null, TimeSpan.FromSeconds(2));
        }
    }
}
