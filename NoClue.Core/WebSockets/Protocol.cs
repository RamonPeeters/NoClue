using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NoClue.Core.WebSockets {
    public static class Protocol {
        public static async Task SendMessage(WebSocket webSocket, byte[] content) {
            await webSocket.SendAsync(content, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public static async Task SendClosingMessage(WebSocket webSocket, byte[] content, string closingMessage) {
            await SendMessage(webSocket, content);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, closingMessage, CancellationToken.None);
        }
    }
}
