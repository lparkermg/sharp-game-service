using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Options;
using SharpGameService.Core;
using SharpGameService.Core.Configuration;
using SharpGameService.Core.Exceptions;
using SharpGameService.Tests.Implementations;
using System.Net.WebSockets;

namespace SharpGameService.Tests
{
    public class HouseTests
    {
        private House<TestRoom> _house;

        private MemoryStream _connectionStream;

        private Fixture _fixture;
        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());

            var options = _fixture.Create<IOptions<SharpGameServiceOptions>>();
            options.Value.Rooms.MaxPlayersPerRoom = 1;
            options.Value.House.MaxRooms = 1;
            options.Value.MaxMessageSizeKb = 4;
            options.Value.Rooms.CloseRoomsOnEmpty = true;
            options.Value.Rooms.CloseWaitTime = TimeSpan.FromSeconds(30);

            _house = new House<TestRoom>(options);
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
            string playerName = "TestPlayer";
            Assert.That(() => _house.Join(roomId, roomCode, playerName, CreateWebSocket()), Throws.ArgumentException.With.Message.EqualTo("Room Id must be populated"));
        }

        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        public void Join_GivenInvalidPlayerName_ThrowsArgumentException(string? playerName)
        {
            string roomCode = "TestRoomCode";
            string roomId = "roomId";
            Assert.That(() => _house.Join(roomId, roomCode, playerName, CreateWebSocket()), Throws.ArgumentException.With.Message.EqualTo("Player Name must be populated"));
        }

        [Test]
        public void Join_GivenNonExistingRoomId_ThrowsRoomNotFoundException()
        {
            string roomId = "NonExistingRoomId";
            string roomCode = "TestRoomCode";
            string playerName = "TestPlayer";
            Assert.That(() => _house.Join(roomId, roomCode, playerName, CreateWebSocket()), Throws.TypeOf<RoomNotFoundException>().And.With.Message.EqualTo("The room for the provided Id does not exist"));
        }

        [Test]
        public void Join_GivenExistingRoomIdWithWrongCode_ThrowsInvalidRoomCodeException()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            string playerName = "TestPlayer";
            _house.CreateRoom(roomId, roomCode);
            Assert.That(() => _house.Join(roomId, "WrongCode", playerName, CreateWebSocket()), Throws.TypeOf<InvalidRoomCodeException>().And.With.Message.EqualTo("The room code provided is invalid"));
        }

        [Test]
        public void Join_GivenExistingRoomIdWithCorrectCode_AddsPlayerToRoom()
        {
            string roomId = "TestRoomId";
            string roomCode = "TestRoomCode";
            string playerName = "TestPlayer";
            _house.CreateRoom(roomId, roomCode);
            var playerWebSocket = CreateWebSocket();
            _house.Join(roomId, roomCode, playerName, playerWebSocket);
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

        // TODO: Implement ProcessAsync tests when there's a good way to do it.

        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        public void MessageReceived_GivenInvalidRoomId_ThrowsArgumentException(string? roomId)
        {
            Assert.That(() => _house.MessageReceived(roomId, "test data"), Throws.ArgumentException.With.Message.EqualTo("Room Id must be populated"));
        }

        [TestCase("")]
        [TestCase("     ")]
        [TestCase(null)]
        public void MessageReceived_GivenInvalidMessageData_ThrowsArgumentException(string? messageData)
        {
            Assert.That(() => _house.MessageReceived("roomId", messageData), Throws.ArgumentException.With.Message.EqualTo("Message must be populated"));
        }

        [Test]
        public void MessageReceived_GivenNonExistingRoomId_ThrowsRoomNotFoundException()
        {
            string roomId = "NonExistingRoomId";
            Assert.That(() => _house.MessageReceived(roomId, "test data"), Throws.TypeOf<RoomNotFoundException>().And.With.Message.EqualTo("The room for the provided Id does not exist"));
        }

        // TODO: Implement MessageReceived valid tests when there's a good way to do it.

        // TODO: Implement CloseAll tests when there's a good way to do it.

        private WebSocket CreateWebSocket()
        {
            return WebSocket.CreateFromStream(_connectionStream, false, null, TimeSpan.FromSeconds(2));
        }
    }
}
