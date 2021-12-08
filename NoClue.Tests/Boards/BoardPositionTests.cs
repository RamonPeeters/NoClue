using NoClue.Core.Boards;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NoClue.Tests.Boards {
    [TestClass]
    public class BoardPositionTests {
        [TestMethod]
        public void Shift_AddsCoordinatesTogether() {
            BoardPosition position = new BoardPosition(2, 3);
            BoardPosition shiftedPosition = position.Shift(1, 2);
            Assert.AreEqual(new BoardPosition(3, 5), shiftedPosition);
        }
    }
}
