using NoClue.Core.Boards;
using NoClue.Core.Cards;
using NoClue.Core.Players;
using NoClue.Core.WebSockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NoClue.Core.Rooms {
    public class GameRoom {
        private readonly PlayerCollection Players = new PlayerCollection();
        private readonly Guid Owner;
        private bool GameStarted;
        private NoClueAnswer Answer;
        private Guid? CurrentPlayer = null;
        private Board Board;

        private GameRoom(WebSocket creator) {
            Owner = Players.AddPlayer(new Player(creator));
        }

        public RoomJoinReason TryJoin(WebSocket webSocket, out Guid origin) {
            if (Players.Count == 6) {
                origin = Guid.Empty;
                return RoomJoinReason.ROOM_FULL;
            }

            origin = Players.AddPlayer(new Player(webSocket));
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
                Players.TryRemovePlayer(origin, out Player player);
                await player.GetWebSocket().CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
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
            if (id == 7) {
                await TryRollDice(origin);
            }
        }

        private async Task<bool> TryStartGame(Guid origin) {
            if (GameStarted) {
                return false;
            }

            WebSocket webSocket = Players[origin].GetWebSocket();
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

        private async Task TryRollDice(Guid origin) {
            if (!MayRollDice(origin)) {
                return;
            }
            Random random = new Random();
            int firstDieRoll = random.Next(6) + 1;
            int secondDieRoll = random.Next(6) + 1;

            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(8);
            writer.WriteInt(firstDieRoll);
            writer.WriteInt(secondDieRoll);
            await SendGlobalMessage(writer);
            Dictionary<BoardPosition, int> positions = Board.Traverse(firstDieRoll + secondDieRoll, new BoardPosition(6, 6));
            await SendAvailableSpaces(origin, positions);
        }

        private async Task StartGame() {
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(4);
            await SendGlobalMessage(writer);
            GameStarted = true;
            Board = new Board();

            await DivideCards();
            CurrentPlayer = Owner;
            await SelectPlayer();
        }

        private async Task DivideCards() {
            Random random = new Random();
            CardCollection<SuspectCard> suspects = SuspectCard.GetAllSuspectCards();
            CardCollection<WeaponCard> weapons = WeaponCard.GetAllWeaponCards();
            CardCollection<RoomCard> rooms = RoomCard.GetAllRoomCards();

            Answer = new NoClueAnswer(suspects.SelectRandom(random), weapons.SelectRandom(random), rooms.SelectRandom(random));
            Console.WriteLine(Answer);
            CardCollection<Card> remainingCards = new CardCollection<Card>().Merge(suspects).Merge(weapons).Merge(rooms);
            remainingCards.Shuffle(random);

            while (remainingCards.Count > 0) {
                foreach (Player player in Players) {
                    await player.GiveCard(remainingCards.SelectRandom(random));
                }
            }
        }

        private async Task SelectPlayer() {
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(6);

            WebSocket webSocket = Players[CurrentPlayer.Value].GetWebSocket();
            await Protocol.SendMessage(webSocket, writer.ToArray());
        }

        private bool MayRollDice(Guid origin) {
            return CurrentPlayer == origin;
        }

        private async Task SendAvailableSpaces(Guid origin, Dictionary<BoardPosition, int> positions) {
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(9);
            int[] actualPositions = Util.GetArrayFromPositions(positions.Keys);
            writer.WriteIntArray(actualPositions);
            WebSocket webSocket = Players[origin].GetWebSocket();
            await Protocol.SendMessage(webSocket, writer.ToArray());
        }

        private async Task SendGlobalMessage(ProtocolBinaryWriter writer) {
            byte[] data = writer.ToArray();
            foreach (Player player in Players) {
                await Protocol.SendMessage(player.GetWebSocket(), data);
            }
        }
    }
}
