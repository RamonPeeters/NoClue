using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace NoClue.Core.WebSockets {
    public class WebSocketCollection : IEnumerable<WebSocket> {
        private readonly ConcurrentDictionary<Guid, WebSocket> WebSockets = new ConcurrentDictionary<Guid, WebSocket>();

        public int Count {
            get {
                return WebSockets.Count;
            }
        }

        public WebSocket this[Guid uuid] {
            get {
                return WebSockets[uuid];
            }
        }

        public IEnumerator<WebSocket> GetEnumerator() {
            foreach (WebSocket webSocket in WebSockets.Values) {
                yield return webSocket;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Guid AddWebSocket(WebSocket webSocket) {
            Guid uuid = Guid.NewGuid();
            WebSockets.TryAdd(uuid, webSocket);
            return uuid;
        }

        public bool TryRemoveWebSocket(Guid uuid, out WebSocket result) {
            return WebSockets.TryRemove(uuid, out result);
        }
    }
}
