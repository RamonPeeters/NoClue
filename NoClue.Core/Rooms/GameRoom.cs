using NoClue.Core.WebSockets;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NoClue.Core.Rooms {
    public class GameRoom {
        private readonly WebSocketCollection Connections = new WebSocketCollection();

        public RoomJoinReason TryJoin(WebSocket webSocket, out Guid origin) {
            if (Connections.Count == 6) {
                origin = Guid.Empty;
                return RoomJoinReason.ROOM_FULL;
            }

            origin = Connections.AddWebSocket(webSocket);
            return RoomJoinReason.SUCCESS;
        }

        public async Task ReceiveMessage(Guid origin, WebSocket webSocket) {
            byte[] buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open) {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                await HandleMessage(origin, result, buffer);
            }
        }

        private async Task HandleMessage(Guid origin, WebSocketReceiveResult result, byte[] buffer) {
            if (result.MessageType == WebSocketMessageType.Close) {
                Connections.TryRemoveWebSocket(origin, out WebSocket webSocket);
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                return;
            }
        }
    }
}
