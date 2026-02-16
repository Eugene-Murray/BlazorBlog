# Duel Grid Game - Multiplayer with WebSockets

## Overview
The Duel Grid Game (Tic-Tac-Toe) has been converted to a real-time multiplayer game using SignalR WebSockets.

## Features
- ✅ Real-time multiplayer gameplay
- ✅ Room-based game sessions
- ✅ Unique 6-character room IDs for easy sharing
- ✅ Turn-based gameplay with visual indicators
- ✅ Automatic disconnect handling
- ✅ Game reset functionality

## How to Play

### Create a New Game
1. Navigate to `/duelgridgame`
2. Click **"Create New Game"**
3. You'll be assigned as Player X and receive a unique Room ID (e.g., `A3F7B2`)
4. Share the Room ID with your opponent
5. Wait for them to join

### Join an Existing Game
1. Navigate to `/duelgridgame`
2. Enter the Room ID provided by your opponent
3. Click **"Join Game"**
4. You'll be assigned as Player O
5. The game starts immediately

### Playing
- Player X always goes first
- Players take turns clicking on empty grid cells
- The game indicates whose turn it is
- When a player wins or the game ends in a draw, a "New Game" button appears
- You can leave the game at any time using the "Leave Game" button

## Technical Implementation

### Server Side
- **GameHub.cs**: SignalR hub managing game sessions
  - `CreateRoom()`: Creates a new game room with unique ID
  - `JoinRoom(roomId)`: Allows second player to join
  - `MakeMove(index)`: Processes player moves
  - `ResetGame()`: Resets the game board
  - Handles player disconnections automatically

### Client Side
- **DuelGridGame.razor**: Updated to use SignalR client
  - Connects to `/gamehub` endpoint
  - Listens for real-time game events
  - Manages connection state
  - Updates UI in real-time

### Real-Time Events
- `GameStarted`: Fired when both players are connected
- `MoveMade`: Broadcasts moves to both players
- `GameOver`: Notifies when game ends
- `GameReset`: Syncs game board reset
- `PlayerDisconnected`: Handles opponent leaving

## Dependencies Added
- `Microsoft.AspNetCore.SignalR.Client` (v10.0.2)

## Architecture
```
Client (Blazor WebAssembly)
    ↕ SignalR WebSocket Connection
Server (ASP.NET Core)
    ↕ GameHub
In-Memory Game State (Dictionary)
```

## Notes
- Game state is stored in-memory on the server
- If the server restarts, all active games are lost
- For production, consider implementing persistent storage
- Room IDs are 6 characters for easy sharing
- Maximum 2 players per room
