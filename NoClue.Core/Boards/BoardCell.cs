namespace NoClue.Core.Boards {
    internal class BoardCell {
        public BoardCellType Type { get; }

        public BoardCell(BoardCellType type) {
            Type = type;
        }

        public static BoardCell Floor() {
            return new BoardCell(BoardCellType.FLOOR);
        }

        public static BoardCell Wall() {
            return new BoardCell(BoardCellType.WALL);
        }

        public static BoardCell Door() {
            return new BoardCell(BoardCellType.DOOR);
        }
    }
}
