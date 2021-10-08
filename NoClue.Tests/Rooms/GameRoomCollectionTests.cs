using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoClue.Core.Rooms;
using System;

namespace NoClue.Tests.Rooms {
    [TestClass]
    public class GameRoomCollectionTests {
        [TestMethod]
        public void Constructor_ThrowsException_BecauseMaxRoomCountIsNegative() {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GameRoomCollection(-10));
        }

        [TestMethod]
        public void TryCreate_ReturnsTrue() {
            GameRoomCollection rooms = new GameRoomCollection(10);
            bool successful = rooms.TryCreate(out _);
            Assert.IsTrue(successful);
        }

        [TestMethod]
        public void TryCreate_ReturnsFalse_BecauseThereAreTooManyRooms() {
            GameRoomCollection rooms = new GameRoomCollection(0);
            bool successful = rooms.TryCreate(out _);
            Assert.IsFalse(successful);
        }
    }
}
