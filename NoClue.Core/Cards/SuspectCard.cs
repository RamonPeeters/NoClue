namespace NoClue.Core.Cards {
    internal class SuspectCard : Card {
        private static readonly CardCollection<SuspectCard> Suspects = new CardCollection<SuspectCard>();

        public static readonly SuspectCard RED = new SuspectCard(0);
        public static readonly SuspectCard YELLOW = new SuspectCard(1);
        public static readonly SuspectCard GREEN = new SuspectCard(2);
        public static readonly SuspectCard CYAN = new SuspectCard(3);
        public static readonly SuspectCard PURPLE = new SuspectCard(4);
        public static readonly SuspectCard WHITE = new SuspectCard(5);

        public override CardType Type {
            get {
                return CardType.SUSPECT;
            }
        }

        private SuspectCard(int id) : base(id) {
            Suspects.Add(this);
        }

        public static CardCollection<SuspectCard> GetAllSuspectCards() {
            return new CardCollection<SuspectCard>(Suspects);
        }
    }
}
