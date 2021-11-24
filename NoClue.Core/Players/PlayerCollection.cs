using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NoClue.Core.Players {
    internal class PlayerCollection : IEnumerable<Player> {
        private int CurrentId = -1;
        private readonly ConcurrentDictionary<int, Player> Players = new ConcurrentDictionary<int, Player>();

        public int Count {
            get {
                return Players.Count;
            }
        }

        public Player this[int id] {
            get {
                return Players[id];
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

        public int AddPlayer(Player player) {
            CurrentId++;
            Players.TryAdd(CurrentId, player);
            return CurrentId;
        }

        public bool TryRemovePlayer(int id, out Player result) {
            return Players.TryRemove(id, out result);
        }
    }
}
