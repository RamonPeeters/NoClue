using NoClue.Core.WebSockets;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task TryCreate(WebSocket webSocket) {
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(0);

            int? code = GetUniqueCode();
            if (!code.HasValue) {
                writer.WriteBoolean(false);
                await Protocol.SendClosingMessage(webSocket, writer.ToArray(), "Unable to create room");
                return;
            }

            GameRoom room = GameRoom.Create(webSocket, out int playerId);
            Rooms.Add(code.Value, room);
            writer.WriteBoolean(true);
            writer.WriteInt(code.Value);
            writer.WriteInt(playerId);
            await Protocol.SendMessage(webSocket, writer.ToArray());
            await room.ReceiveMessage(playerId, webSocket);
        }

        public async Task TryJoin(int code, WebSocket webSocket) {
            List<byte> bytes = new List<byte>() { 0, 0, 0, 1 };

            if (!Rooms.TryGetValue(code, out GameRoom room)) {
                bytes.Add((byte)RoomJoinReason.ROOM_NOT_FOUND);
                await Protocol.SendClosingMessage(webSocket, bytes.ToArray(), "Unable to join room");
                return;
            }

            RoomJoinReason reason = room.TryJoin(webSocket, out int playerId);
            bytes.Add((byte)reason);
            await Protocol.SendMessage(webSocket, bytes.ToArray());

            if (reason == RoomJoinReason.SUCCESS) {
                await room.ReceiveMessage(playerId, webSocket);
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
