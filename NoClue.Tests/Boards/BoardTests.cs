using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoClue.Core.Boards;

namespace NoClue.Tests.Boards {
    [TestClass]
    public class BoardTests {
        [TestMethod]
        public void OccupyCell_ReturnsTrue() {
            Board board = new Board();
            bool successful = board.OccupyCell(1, new BoardPosition(0, 0));
            Assert.IsTrue(successful);
        }

        [TestMethod]
        public void OccupyCell_ReturnsFalse_BecausePositionIsOutOfBounds() {
            Board board = new Board();
            bool successful = board.OccupyCell(1, new BoardPosition(-1, -1));
            Assert.IsFalse(successful);
        }
    }
}
