using NoClue.Core.Cards;
using NoClue.Core.WebSockets;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NoClue.Core.Rooms {
    public class GameRoom {
        private readonly WebSocketCollection Connections = new WebSocketCollection();
        private readonly Guid Owner;
        private bool GameStarted;
        private NoClueAnswer Answer;

        private GameRoom(WebSocket creator) {
            Owner = Connections.AddWebSocket(creator);
        }

        public RoomJoinReason TryJoin(WebSocket webSocket, out Guid origin) {
            if (Connections.Count == 6) {
                origin = Guid.Empty;
                return RoomJoinReason.ROOM_FULL;
            }

            origin = Connections.AddWebSocket(webSocket);
            return RoomJoinReason.SUCCESS;
        }

        public async Task ReceiveMessage(Guid origin, WebSocket webSocket) {
            byte[] buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open) {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                await HandleMessage(origin, result, buffer);
            }
        }

        public static GameRoom Create(WebSocket creator, out Guid owner) {
            GameRoom room = new GameRoom(creator);
            owner = room.Owner;
            return room;
        }

        private async Task HandleMessage(Guid origin, WebSocketReceiveResult result, byte[] buffer) {
            if (result.MessageType == WebSocketMessageType.Close) {
                Connections.TryRemoveWebSocket(origin, out WebSocket webSocket);
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                return;
            }
            if (result.MessageType == WebSocketMessageType.Binary) {
                await HandlePacket(origin, buffer);
            }
        }

        private async Task HandlePacket(Guid origin, byte[] buffer) {
            using MemoryStream memoryStream = new MemoryStream(buffer);
            using ProtocolBinaryReader reader = new ProtocolBinaryReader(memoryStream);

            int id = reader.ReadInt();
            if (id == 2) {
                await TryStartGame(origin);
            }
        }

        private async Task<bool> TryStartGame(Guid origin) {
            if (GameStarted) {
                return false;
            }

            WebSocket webSocket = Connections[origin];
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(3);

            if (Owner != origin) {
                writer.WriteBoolean(false);
                await Protocol.SendMessage(webSocket, writer.ToArray());
                return false;
            }

            writer.WriteBoolean(true);
            await Protocol.SendMessage(webSocket, writer.ToArray());
            await StartGame();

            return true;
        }

        private async Task StartGame() {
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(4);
            await SendGlobalMessage(writer);
            GameStarted = true;

            await DivideCards();
        }

        private async Task DivideCards() {
            Random random = new Random();
            CardCollection<SuspectCard> suspects = SuspectCard.GetAllSuspectCards();
            CardCollection<WeaponCard> weapons = WeaponCard.GetAllWeaponCards();
            CardCollection<RoomCard> rooms = RoomCard.GetAllRoomCards();

            Answer = new NoClueAnswer(suspects.SelectRandom(random), weapons.SelectRandom(random), rooms.SelectRandom(random));
        }

        private async Task SendGlobalMessage(ProtocolBinaryWriter writer) {
            byte[] data = writer.ToArray();
            foreach (WebSocket webSocket in Connections) {
                await Protocol.SendMessage(webSocket, data);
            }
        }
    }
}
