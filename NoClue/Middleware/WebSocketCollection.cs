using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace NoClue.Middleware {
    public class WebSocketCollection {
        private readonly ConcurrentDictionary<Guid, WebSocket> WebSockets = new ConcurrentDictionary<Guid, WebSocket>();

        public Guid AddWebSocket(WebSocket webSocket) {
            Guid uuid = Guid.NewGuid();
            WebSockets.TryAdd(uuid, webSocket);
            return uuid;
        }
    }
}
