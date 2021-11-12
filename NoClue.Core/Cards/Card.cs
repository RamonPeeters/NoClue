namespace NoClue.Core.Cards {
    internal abstract class Card {
        public abstract CardType Type { get; }
        public int Id { get; }

        protected Card(int id) {
            Id = id;
        }
    }
}
