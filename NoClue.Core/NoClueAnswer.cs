using NoClue.Core.Cards;

namespace NoClue.Core {
    internal class NoClueAnswer {
        private readonly SuspectCard Suspect;
        private readonly WeaponCard Weapon;
        private readonly RoomCard Room;

        public NoClueAnswer(SuspectCard suspect, WeaponCard weapon, RoomCard room) {
            Suspect = suspect;
            Weapon = weapon;
            Room = room;
        }

        public override string ToString() {
            return $"{{ Suspect: {Suspect.Id}, Weapon: {Weapon.Id}, Room: {Room.Id} }}";
        }
    }
}
