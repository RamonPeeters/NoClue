namespace NoClue.Core.Cards {
    internal class RoomCard : Card {
        private static readonly CardCollection<RoomCard> Rooms = new CardCollection<RoomCard>();

        public static readonly RoomCard HALL = new RoomCard(0);
        public static readonly RoomCard LOUNGE = new RoomCard(1);
        public static readonly RoomCard DINING_ROOM = new RoomCard(2);
        public static readonly RoomCard KITCHEN = new RoomCard(3);
        public static readonly RoomCard BALLROOM = new RoomCard(4);
        public static readonly RoomCard CONSERVATORY = new RoomCard(5);
        public static readonly RoomCard BILLIARD_ROOM = new RoomCard(6);
        public static readonly RoomCard LIBRARY = new RoomCard(7);
        public static readonly RoomCard STUDY = new RoomCard(8);

        public override CardType Type {
            get {
                return CardType.ROOM;
            }
        }

        private RoomCard(int id) : base(id) {
            Rooms.Add(this);
        }

        public static CardCollection<RoomCard> GetAllRoomCards() {
            return new CardCollection<RoomCard>(Rooms);
        }
    }
}
