using Microsoft.AspNetCore.SignalR;

namespace BlazorGallery.Hubs;

public class GameHub : Hub
{
    private static readonly Dictionary<string, GameRoom> _gameRooms = new();
    private static readonly Dictionary<string, string> _playerRooms = new();

    public async Task<GameRoomInfo> CreateRoom()
    {
        var roomId = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        var room = new GameRoom
        {
            RoomId = roomId,
            Player1Id = Context.ConnectionId,
            Player1Symbol = "X",
            CurrentTurn = "X"
        };

        _gameRooms[roomId] = room;
        _playerRooms[Context.ConnectionId] = roomId;

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        return new GameRoomInfo
        {
            RoomId = roomId,
            YourSymbol = "X",
            Status = "Waiting for opponent..."
        };
    }

    public async Task<GameRoomInfo?> JoinRoom(string roomId)
    {
        roomId = roomId.ToUpper();

        if (!_gameRooms.TryGetValue(roomId, out var room))
        {
            return null;
        }

        if (room.Player2Id != null)
        {
            return null;
        }

        room.Player2Id = Context.ConnectionId;
        room.Player2Symbol = "O";
        _playerRooms[Context.ConnectionId] = roomId;

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        await Clients.Group(roomId).SendAsync("GameStarted");

        return new GameRoomInfo
        {
            RoomId = roomId,
            YourSymbol = "O",
            Status = "Game started!"
        };
    }

    public async Task MakeMove(int index)
    {
        if (!_playerRooms.TryGetValue(Context.ConnectionId, out var roomId))
        {
            return;
        }

        if (!_gameRooms.TryGetValue(roomId, out var room))
        {
            return;
        }

        var playerSymbol = Context.ConnectionId == room.Player1Id ? room.Player1Symbol : room.Player2Symbol;

        if (room.CurrentTurn != playerSymbol || !string.IsNullOrEmpty(room.Grid[index]))
        {
            return;
        }

        room.Grid[index] = playerSymbol;

        if (CheckWinner(room.Grid, playerSymbol))
        {
            room.GameStatus = $"Player {playerSymbol} wins!";
            await Clients.Group(roomId).SendAsync("GameOver", room.GameStatus, room.Grid);
        }
        else if (IsBoardFull(room.Grid))
        {
            room.GameStatus = "It's a draw!";
            await Clients.Group(roomId).SendAsync("GameOver", room.GameStatus, room.Grid);
        }
        else
        {
            room.CurrentTurn = playerSymbol == "X" ? "O" : "X";
            await Clients.Group(roomId).SendAsync("MoveMade", index, playerSymbol, room.CurrentTurn);
        }
    }

    public async Task ResetGame()
    {
        if (!_playerRooms.TryGetValue(Context.ConnectionId, out var roomId))
        {
            return;
        }

        if (!_gameRooms.TryGetValue(roomId, out var room))
        {
            return;
        }

        room.Grid = new string[9];
        room.CurrentTurn = "X";
        room.GameStatus = "Game in progress";

        await Clients.Group(roomId).SendAsync("GameReset");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_playerRooms.TryGetValue(Context.ConnectionId, out var roomId))
        {
            if (_gameRooms.TryGetValue(roomId, out var room))
            {
                await Clients.Group(roomId).SendAsync("PlayerDisconnected");
                _gameRooms.Remove(roomId);
            }

            _playerRooms.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private bool CheckWinner(string[] grid, string player)
    {
        int[][] winPatterns = new int[][]
        {
            new int[] { 0, 1, 2 },
            new int[] { 3, 4, 5 },
            new int[] { 6, 7, 8 },
            new int[] { 0, 3, 6 },
            new int[] { 1, 4, 7 },
            new int[] { 2, 5, 8 },
            new int[] { 0, 4, 8 },
            new int[] { 2, 4, 6 }
        };

        foreach (var pattern in winPatterns)
        {
            if (grid[pattern[0]] == player &&
                grid[pattern[1]] == player &&
                grid[pattern[2]] == player)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsBoardFull(string[] grid)
    {
        return grid.All(cell => !string.IsNullOrEmpty(cell));
    }
}

public class GameRoom
{
    public string RoomId { get; set; } = string.Empty;
    public string? Player1Id { get; set; }
    public string? Player2Id { get; set; }
    public string Player1Symbol { get; set; } = "X";
    public string Player2Symbol { get; set; } = "O";
    public string[] Grid { get; set; } = new string[9];
    public string CurrentTurn { get; set; } = "X";
    public string GameStatus { get; set; } = "Game in progress";
}

public class GameRoomInfo
{
    public string RoomId { get; set; } = string.Empty;
    public string YourSymbol { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
