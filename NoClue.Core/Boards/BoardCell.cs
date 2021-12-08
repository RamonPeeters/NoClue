using NoClue.Core.Boards.Rooms;

namespace NoClue.Core.Boards {
    public class BoardCell {
        public BoardCellType Type { get; }
        public RoomType? RoomType { get; }

        public BoardCell(BoardCellType type, RoomType? roomType) {
            Type = type;
            RoomType = roomType;
        }

        public static BoardCell Floor() {
            return new BoardCell(BoardCellType.FLOOR, null);
        }

        public static BoardCell Wall() {
            return new BoardCell(BoardCellType.WALL, null);
        }

        public static BoardCell Door(RoomType roomType) {
            return new BoardCell(BoardCellType.DOOR, roomType);
        }
    }
}
