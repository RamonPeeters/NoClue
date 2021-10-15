using NoClue.Core.WebSockets;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NoClue.Core.Rooms {
    public class GameRoomCollection {
        private const int MAX_ATTEMPTS = 1000;

        private readonly Func<int> RoomCodeProvider;
        private readonly Dictionary<int, GameRoom> Rooms = new Dictionary<int, GameRoom>();


        public GameRoomCollection(Func<int> roomCodeProvider) {
            RoomCodeProvider = roomCodeProvider;
        }

        public async Task<int?> TryCreate(WebSocket webSocket) {
            List<byte> bytes = new List<byte>() { 0, 0, 0, 1 };

            int? code = GetUniqueCode();
            if (!code.HasValue) {
                bytes.Add(0);
                await Protocol.SendMessage(webSocket, bytes.ToArray());
                return null;
            }

            Rooms.Add(code.Value, new GameRoom());
            bytes.Add(1);
            await Protocol.SendMessage(webSocket, bytes.ToArray());
            return code;
        }

        public async Task TryJoin(int code, WebSocket webSocket) {
            List<byte> bytes = new List<byte>() { 0, 0, 0, 2 };

            if (!Rooms.TryGetValue(code, out GameRoom room)) {
                bytes.Add((byte)RoomJoinReason.ROOM_NOT_FOUND);
                await Protocol.SendMessage(webSocket, bytes.ToArray());
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unable to join room", CancellationToken.None);
                return;
            }

            RoomJoinReason reason = room.TryJoin(webSocket, out Guid origin);
            bytes.Add((byte)reason);
            await Protocol.SendMessage(webSocket, bytes.ToArray());

            if (reason == RoomJoinReason.SUCCESS) {
                await room.ReceiveMessage(origin, webSocket);
            } else {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unable to join room", CancellationToken.None);
            }
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
