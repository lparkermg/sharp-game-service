using AutoFixture;
using AutoFixture.AutoMoq;
using SharpGameService.Core;
using SharpGameService.Core.Exceptions;
using System.Net.WebSockets;

namespace SharpGameService.Tests
{
    public class BaseRoomTests
    {
        private Fixture _fixture;

        private MemoryStream _connectionStream;

        private BaseRoom _room;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        [SetUp]
        public void Setup()
        { 
            _room = new BaseRoom();
            _connectionStream = new MemoryStream();
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Initialise_GivenInvalidRoomId_ThrowsArgumentException(string? id)
        {
            Assert.That(() => _room.Initialise(id, "code", 4, true),
                Throws.ArgumentException.With.Message.EqualTo("Room Id must be populated"));
        }

        [Test]
        public void Initalise_GivenZeroMaxPlayers_ThrowsArgumentException()
        {
            Assert.That(() => _room.Initialise("id", "code", 0, true),
                Throws.ArgumentException.With.Message.EqualTo("Max players must be greater than 0"));
        }

        [Test]
        public void Initialise_GivenValidParameters_SetsProperties()
        {
            _room.Initialise("id", "code", 4, true);
            Assert.Multiple(() =>
            {
                Assert.That(_room.Id, Is.EqualTo("id"));
                Assert.That(_room.Code, Is.EqualTo("code"));
                Assert.That(_room.MaxPlayers, Is.EqualTo(4));
            });
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Join_GivenInvalidPlayerId_ThrowsArgumentException(string? id)
        {
            var connection = CreateWebSocket();
            _room.Initialise("id", "code", 4, true);
            Assert.That(() => _room.Join("name", id, connection),
                Throws.ArgumentException.With.Message.EqualTo("Player Id must be populated"));
        }

        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Join_GivenInvalidPlayerName_ThrowsArgumentException(string? name)
        {
            var connection = CreateWebSocket();
            _room.Initialise("id", "code", 4, true);
            Assert.That(() => _room.Join(name, "id", connection),
                Throws.ArgumentException.With.Message.EqualTo("Player Name must be populated"));
        }

        [Test]
        public void Join_GivenUninitialisedRoom_ThrowsInvalidOperationException()
        {
            var connection = CreateWebSocket();
            Assert.That(() => _room.Join("name", "id", connection),
                Throws.InvalidOperationException.With.Message.EqualTo("The room has not been initialised"));
        }

        [Test]
        public void Join_GivenConnectionOnFullRoom_ThrowsRoomFullException()
        {
            _room.Initialise("id", "code", 1, false);
            var connection1 = CreateWebSocket();
            var connection2 = CreateWebSocket();
            _room.Join("name", "id", connection1);
            Assert.That(() => _room.Join("name", "id", connection2),
                Throws.Exception.TypeOf<RoomFullException>()
                .And.With.Message.EqualTo("The room you're trying to enter is full"));
        }

        [Test]
        public void Join_GivenConnectionOnEmptyRoom_AddsConnection()
        {
            _room.Initialise("id", "code", 2, false);
            var connection = CreateWebSocket();
            _room.Join("name", "id", connection);
            Assert.That(_room.CurrentPlayers, Is.EqualTo(1));
        }

        [Test]
        public void Join_GivenConnectionOnPartiallyFullRoom_AddsConnection()
        {
            _room.Initialise("id", "code", 3, false);
            var connection = CreateWebSocket();
            _room.Join("name", "id", connection);

            var connection2 = CreateWebSocket();
            _room.Join("name", "id", connection2);
            Assert.That(_room.CurrentPlayers, Is.EqualTo(2));
        }

        [Test]
        public void Process_GivenUninitialisedRoom_ThrowsInvalidOperationException()
        {
            Assert.That(() => _room.Process(),
                Throws.InvalidOperationException.With.Message.EqualTo("The room has not been initialised"));
        }

        // TODO: Find a better way to test the connection removal logic.
        /*[Test]
        public async Task Process_GivenClosedConnection_RemovesConnection()
        {
            _room.Initialise("id", "code", 2);
            var connection = CreateWebSocket();
            _room.Join(connection);
            var connection2 = CreateWebSocket();
            _room.Join(connection2);
            Assert.That(_room.CurrentPlayers, Is.EqualTo(2));
            await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            await _room.Process();
            Assert.That(_room.CurrentPlayers, Is.EqualTo(1));
        }

        // TODO: FInd a way to test the time between last connection and room closing.
        [Test]
        public async Task Process_GivenNoConnections_ClosesRoom()
        {
            _room.Initialise("id", "code", 1, true);

            await _room.Process();

            Assert.That(_room.RoomClosing, Is.True);
        }*/

        private WebSocket CreateWebSocket()
        {
            return WebSocket.CreateFromStream(_connectionStream, false, null, TimeSpan.FromSeconds(2));
        }
    }
}