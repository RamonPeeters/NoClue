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
        private readonly int Owner;
        private bool GameStarted;
        private NoClueAnswer Answer;
        private int? CurrentPlayer = null;
        private Board Board;
        private Dictionary<BoardPosition, int> AvailablePositions;

        private GameRoom(WebSocket creator) {
            Owner = Players.AddPlayer(new Player(creator));
        }

        public RoomJoinReason TryJoin(WebSocket webSocket, out int playerId) {
            if (Players.Count == 6) {
                playerId = -1;
                return RoomJoinReason.ROOM_FULL;
            }

            playerId = Players.AddPlayer(new Player(webSocket));
            return RoomJoinReason.SUCCESS;
        }

        public async Task ReceiveMessage(int playerId, WebSocket webSocket) {
            byte[] buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open) {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                await HandleMessage(playerId, result, buffer);
            }
        }

        public static GameRoom Create(WebSocket creator, out int owner) {
            GameRoom room = new GameRoom(creator);
            owner = room.Owner;
            return room;
        }

        private async Task HandleMessage(int playerId, WebSocketReceiveResult result, byte[] buffer) {
            if (result.MessageType == WebSocketMessageType.Close) {
                Players.TryRemovePlayer(playerId, out Player player);
                await player.GetWebSocket().CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                return;
            }
            if (result.MessageType == WebSocketMessageType.Binary) {
                await HandlePacket(playerId, buffer);
            }
        }

        private async Task HandlePacket(int playerId, byte[] buffer) {
            using MemoryStream memoryStream = new MemoryStream(buffer);
            using ProtocolBinaryReader reader = new ProtocolBinaryReader(memoryStream);

            int id = reader.ReadInt();
            if (id == 2) {
                await TryStartGame(playerId);
            }
            if (id == 7) {
                await TryRollDice(playerId);
            }
            if (id == 10) {
                await TrySelectSpace(playerId, reader);
            }
        }

        private async Task<bool> TryStartGame(int playerId) {
            if (GameStarted) {
                return false;
            }

            WebSocket webSocket = Players[playerId].GetWebSocket();
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(3);

            if (Owner != playerId) {
                writer.WriteBoolean(false);
                await Protocol.SendMessage(webSocket, writer.ToArray());
                return false;
            }

            writer.WriteBoolean(true);
            await Protocol.SendMessage(webSocket, writer.ToArray());
            await StartGame();

            return true;
        }

        private async Task TryRollDice(int playerId) {
            if (!MayRollDice(playerId)) {
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
            AvailablePositions = Board.Traverse(firstDieRoll + secondDieRoll, new BoardPosition(6, 6));
            await SendAvailableSpaces(playerId);
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

        private bool MayRollDice(int playerId) {
            return CurrentPlayer == playerId;
        }

        private async Task SendAvailableSpaces(int playerId) {
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(9);
            int[] actualPositions = Util.GetArrayFromPositions(AvailablePositions.Keys);
            writer.WriteIntArray(actualPositions);
            WebSocket webSocket = Players[playerId].GetWebSocket();
            await Protocol.SendMessage(webSocket, writer.ToArray());
        }

        private async Task TrySelectSpace(int playerId, ProtocolBinaryReader reader) {
            if (playerId != CurrentPlayer) {
                return;
            }
            int x = reader.ReadInt();
            int y = reader.ReadInt();
            BoardPosition boardPosition = new BoardPosition(x, y);

            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(11);
            WebSocket webSocket = Players[playerId].GetWebSocket();
            if (!AvailablePositions.ContainsKey(boardPosition)) {
                writer.WriteBoolean(false);
                await Protocol.SendMessage(webSocket, writer.ToArray());
                return;
            }
            writer.WriteBoolean(true);
            await Protocol.SendMessage(webSocket, writer.ToArray());
            await SendSelectedSpace(boardPosition);
            await MoveToPosition(playerId, boardPosition);
        }

        private async Task SendSelectedSpace(BoardPosition position) {
            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(12);
            writer.WriteInt(position.X);
            writer.WriteInt(position.Y);
            await SendGlobalMessage(writer);
        }

        private async Task MoveToPosition(int playerId, BoardPosition position) {
            Board.OccupyCell(playerId, position);

            using MemoryStream memoryStream = new MemoryStream();
            using ProtocolBinaryWriter writer = new ProtocolBinaryWriter(memoryStream);
            writer.WriteInt(13);
            writer.WriteInt(playerId);
            writer.WriteInt(position.X);
            writer.WriteInt(position.Y);
            await SendGlobalMessage(writer);
        }

        private async Task SendGlobalMessage(ProtocolBinaryWriter writer) {
            byte[] data = writer.ToArray();
            foreach (Player player in Players) {
                await Protocol.SendMessage(player.GetWebSocket(), data);
            }
        }
    }
}
