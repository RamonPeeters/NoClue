namespace NoClue.Core.Cards {
    internal class WeaponCard : Card {
        private static readonly CardCollection<WeaponCard> Weapons = new CardCollection<WeaponCard>();

        public static readonly WeaponCard REVOLVER = new WeaponCard(0);
        public static readonly WeaponCard DAGGER = new WeaponCard(1);
        public static readonly WeaponCard LEAD_PIPE = new WeaponCard(2);
        public static readonly WeaponCard ROPE = new WeaponCard(3);
        public static readonly WeaponCard CANDLESTICK = new WeaponCard(4);
        public static readonly WeaponCard WRENCH = new WeaponCard(5);

        public override CardType Type {
            get {
                return CardType.WEAPON;
            }
        }

        private WeaponCard(int id) : base(id) {
            Weapons.Add(this);
        }

        public static CardCollection<WeaponCard> GetAllWeaponCards() {
            return new CardCollection<WeaponCard>(Weapons);
        }
    }
}
