using NoClue.Core.Cards;
using NoClue.Core.WebSockets;
using System.IO;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace NoClue.Core.Players {
    internal class Player {
        private readonly WebSocket Connection;
        private readonly CardCollection<Card> HeldCards = new CardCollection<Card>();

        public Player(WebSocket connection) {
            Connection = connection;
        }

        public WebSocket GetWebSocket() {
            return Connection;
        }

        public async Task GiveCard(Card card) {
            HeldCards.Add(card);
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);

            writer.WriteInt(5);
            writer.WriteInt((int)card.Type);
            writer.WriteInt(card.Id);
            await Protocol.SendMessage(Connection, writer.ToArray());
        }
    }
}
