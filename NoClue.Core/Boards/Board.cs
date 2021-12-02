using System.Collections.Generic;

namespace NoClue.Core.Boards {
    internal class Board {
        private readonly BoardCell[,] Cells = new BoardCell[12, 12] {
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Door(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Door(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Door(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Door(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Door(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Door(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Door(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Door(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Door() },
            { BoardCell.Door(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Door(),  BoardCell.Wall(),  BoardCell.Wall() },
            { BoardCell.Wall(), BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Door(),  BoardCell.Floor(), BoardCell.Floor(), BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall(),  BoardCell.Wall() }
        };
        private readonly Dictionary<int, BoardPosition> PlayerLocations = new Dictionary<int, BoardPosition>();

        public BoardCell this[BoardPosition boardPosition] {
            get {
                return Cells[boardPosition.X, boardPosition.Y];
            }
        }

        public bool OccupyCell(int playerId, BoardPosition position) {
            if (!InBoundaries(position)) {
                return false;
            }
            PlayerLocations[playerId] = position;
            return true;
        }

        private IEnumerable<BoardPosition> GetNeighborPositions(BoardPosition boardPosition) {
            if (IsTraversable(boardPosition.Shift(0, -1))) {
                yield return boardPosition.Shift(0, -1);
            }
            if (IsTraversable(boardPosition.Shift(-1, 0))) {
                yield return boardPosition.Shift(-1, 0);
            }
            if (IsTraversable(boardPosition.Shift(0, 1))) {
                yield return boardPosition.Shift(0, 1);
            }
            if (IsTraversable(boardPosition.Shift(1, 0))) {
                yield return boardPosition.Shift(1, 0);
            }
        }

        private bool IsTraversable(BoardPosition boardPosition) {
            return InBoundaries(boardPosition) && Cells[boardPosition.X, boardPosition.Y].Type != BoardCellType.WALL;
        }

        private bool InBoundaries(BoardPosition boardPosition) {
            return boardPosition.X >= 0 &&
                boardPosition.X < Cells.GetLength(0) &&
                boardPosition.Y >= 0 &&
                boardPosition.Y < Cells.GetLength(1);
        }

        public Dictionary<BoardPosition, int> Traverse(int maxDistance, params BoardPosition[] startPositions) {
            Queue<BoardPosition> frontier = new Queue<BoardPosition>();
            Dictionary<BoardPosition, int> distances = new Dictionary<BoardPosition, int>();

            foreach (BoardPosition position in startPositions) {
                frontier.Enqueue(position);
                distances.Add(position, 0);
            }

            while (frontier.Count > 0) {
                BoardPosition current = frontier.Dequeue();
                foreach (BoardPosition neighbor in GetNeighborPositions(current)) {
                    if (!distances.ContainsKey(neighbor)) {
                        int newDistance = distances[current] + 1;
                        if (newDistance <= maxDistance) {
                            frontier.Enqueue(neighbor);
                            distances.Add(neighbor, newDistance);
                        }
                    }
                }
            }

            return distances;
        }
    }
}
