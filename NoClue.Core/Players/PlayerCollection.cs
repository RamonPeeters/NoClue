using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NoClue.Core.Players {
    internal class PlayerCollection : IEnumerable<Player> {
        private readonly ConcurrentDictionary<Guid, Player> Players = new ConcurrentDictionary<Guid, Player>();

        public int Count {
            get {
                return Players.Count;
            }
        }

        public Player this[Guid uuid] {
            get {
                return Players[uuid];
            }
        }

        public IEnumerator<Player> GetEnumerator() {
            foreach (Player player in Players.Values) {
                yield return player;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public Guid AddPlayer(Player player) {
            Guid uuid = Guid.NewGuid();
            Players.TryAdd(uuid, player);
            return uuid;
        }

        public bool TryRemovePlayer(Guid uuid, out Player result) {
            return Players.TryRemove(uuid, out result);
        }
    }
}
