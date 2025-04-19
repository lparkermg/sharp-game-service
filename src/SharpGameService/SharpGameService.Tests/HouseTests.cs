using SharpGameService.Core;
using SharpGameService.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGameService.Tests
{
    public class HouseTests
    {
        private House<BaseRoom> _house;

        [SetUp]
        public void SetUp()
        {
            _house = new House<BaseRoom>(1, 1, 4, true, TimeSpan.FromSeconds(30));
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
    }
}
