using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoClue.Core.Rooms;

namespace NoClue.Tests.Rooms {
    [TestClass]
    public class GameRoomCollectionTests {
        [TestMethod]
        public void TryCreate_ReturnsTrue() {
            GameRoomCollection rooms = new GameRoomCollection(CreateRoomCode);
            bool successful = rooms.TryCreate(out _);
            Assert.IsTrue(successful);
        }

        [TestMethod]
        public void TryCreate_ReturnsFalse_BecauseThereAreTooManyRooms() {
            GameRoomCollection rooms = new GameRoomCollection(CreateRoomCode);
            rooms.TryCreate(out _);
            bool successful = rooms.TryCreate(out _);
            Assert.IsFalse(successful);
        }

        private static int CreateRoomCode() {
            return 1;
        }
    }
}
