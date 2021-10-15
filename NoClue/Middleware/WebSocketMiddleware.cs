using Microsoft.AspNetCore.Http;
using NoClue.Core.Rooms;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace NoClue.Middleware {
    public class WebSocketMiddleware : IMiddleware {
        private static readonly Random ROOM_CODE_RNG = new Random();

        private readonly GameRoomCollection Rooms = new GameRoomCollection(CreateRoomCode);

        public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
            if (context.Request.Path == "/game") {
                await Initiate(context);
            } else {
                await next(context);
            }
        }

        private async Task Initiate(HttpContext context) {
            if (!context.WebSockets.IsWebSocketRequest) {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await TryJoin(webSocket);
        }

        private async Task TryJoin(WebSocket webSocket) {
            int? code = await Rooms.TryCreate(webSocket);
            if (!code.HasValue) {
                return;
            }
            await Rooms.TryJoin(code.Value, webSocket);
        }

        private static int CreateRoomCode() {
            return ROOM_CODE_RNG.Next(100000);
        }
    }
}
