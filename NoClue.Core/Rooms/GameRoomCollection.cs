using System;
using System.Collections.Generic;

namespace NoClue.Core.Rooms {
    public class GameRoomCollection {
        private const int MAX_ATTEMPTS = 1000;
        private static readonly Random ROOM_CODE_RNG = new Random();

        private readonly int MaxRoomCount;
        private readonly Dictionary<int, GameRoom> Rooms = new Dictionary<int, GameRoom>();

        public GameRoomCollection(int maxRoomCount) {
            if (maxRoomCount < 0) {
                throw new ArgumentOutOfRangeException(nameof(maxRoomCount));
            }
            MaxRoomCount = maxRoomCount;
        }

        public bool TryCreate(out int code) {
            code = GetUniqueCode();
            if (code == -1) {
                return false;
            }
            Rooms.Add(code, new GameRoom());
            return true;
        }

        private int GetUniqueCode() {
            for (int attempts = 0; attempts < MAX_ATTEMPTS; attempts++) {
                int code = ROOM_CODE_RNG.Next(MaxRoomCount);
                if (!Rooms.ContainsKey(code)) {
                    return code;
                }
            }
            return -1;
        }
    }
}
