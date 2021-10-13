using System;
using System.Collections.Generic;

namespace NoClue.Core.Rooms {
    public class GameRoomCollection {
        private const int MAX_ATTEMPTS = 1000;

        private readonly Func<int> RoomCodeProvider;
        private readonly Dictionary<int, GameRoom> Rooms = new Dictionary<int, GameRoom>();


        public GameRoomCollection(Func<int> roomCodeProvider) {
            RoomCodeProvider = roomCodeProvider;
        }

        public bool TryCreate(out int code) {
            int? uniqueCode = GetUniqueCode();
            if (!uniqueCode.HasValue) {
                code = 0;
                return false;
            }
            code = uniqueCode.Value;
            Rooms.Add(code, new GameRoom());
            return true;
        }

        private int? GetUniqueCode() {
            for (int attempts = 0; attempts < MAX_ATTEMPTS; attempts++) {
                int code = RoomCodeProvider();
                if (!Rooms.ContainsKey(code)) {
                    return code;
                }
            }
            return null;
        }
    }
}
