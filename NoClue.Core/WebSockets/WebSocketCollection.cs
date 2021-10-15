using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace NoClue.Core.WebSockets {
    public class WebSocketCollection {
        private readonly ConcurrentDictionary<Guid, WebSocket> WebSockets = new ConcurrentDictionary<Guid, WebSocket>();

        public int Count {
            get {
                return WebSockets.Count;
            }
        }

        public Guid AddWebSocket(WebSocket webSocket) {
            Guid uuid = Guid.NewGuid();
            WebSockets.TryAdd(uuid, webSocket);
            return uuid;
        }

        public WebSocket GetWebSocket(Guid uuid) {
            return WebSockets[uuid];
        }

        public bool TryRemoveWebSocket(Guid uuid, out WebSocket result) {
            return WebSockets.TryRemove(uuid, out result);
        }
    }
}
