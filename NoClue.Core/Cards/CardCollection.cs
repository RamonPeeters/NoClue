using System;
using System.Collections;
using System.Collections.Generic;

namespace NoClue.Core.Cards {
    internal class CardCollection<T> : IEnumerable<T> where T : Card {
        private readonly List<T> Cards;

        public int Count {
            get { return Cards.Count; }
        }

        public CardCollection() {
            Cards = new List<T>();
        }

        public CardCollection(CardCollection<T> other) {
            Cards = new List<T>(other.Cards);
        }

        public IEnumerator<T> GetEnumerator() {
            foreach (T card in Cards) {
                yield return card;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(T card) {
            Cards.Add(card);
        }

        public bool Contains(T card) {
            return Cards.Contains(card);
        }

        public T SelectRandom(Random random) {
            int index = random.Next(Cards.Count);
            T card = Cards[index];
            Cards.RemoveAt(index);
            return card;
        }

        public void Shuffle(Random random) {
            Cards.Shuffle(random);
        }

        public CardCollection<T> Merge(IEnumerable<T> other) {
            Cards.AddRange(other);
            return this;
        }
    }
}
