using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NoClue.Middleware {
    public class WebSocketMiddleware : IMiddleware {
        private readonly WebSocketCollection WebSockets;

        public WebSocketMiddleware(WebSocketCollection webSockets) {
            WebSockets = webSockets;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
            if (context.WebSockets.IsWebSocketRequest) {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                WebSockets.AddWebSocket(webSocket);
                await ReceiveMessage(webSocket);
            } else {
                await next(context);
            }
        }

        private async Task ReceiveMessage(WebSocket webSocket) {
            byte[] buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open) {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                await HandleMessage(result, buffer);
            }
        }

        private async Task HandleMessage(WebSocketReceiveResult result, byte[] buffer) {}
    }
}
