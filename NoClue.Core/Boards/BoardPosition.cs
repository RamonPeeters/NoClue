using System;

namespace NoClue.Core.Boards {
    public struct BoardPosition : IEquatable<BoardPosition> {
        public int X { get; }
        public int Y { get; }

        public BoardPosition(int x, int y) {
            X = x;
            Y = y;
        }

        public BoardPosition Shift(int x, int y) {
            return new BoardPosition(X + x, Y + y);
        }

        public bool Equals(BoardPosition other) {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj) {
            return obj is BoardPosition position && Equals(position);
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(BoardPosition left, BoardPosition right) {
            return left.Equals(right);
        }

        public static bool operator !=(BoardPosition left, BoardPosition right) {
            return !left.Equals(right);
        }
    }
}
