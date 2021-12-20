using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

public class GameData : NetworkBehaviour {
    private NewNetworkManager room;
    private readonly List<PlayerData> playerData = new List<PlayerData>();
    private PlayerData currentPlayer;

    private enum GameStates {
        Initializing,
        Started,
    }

    [SyncVar] private GameStates state = GameStates.Initializing;

    public bool IsRunning => state == GameStates.Started;

    public override void OnStartClient() {
        room = FindObjectOfType<NewNetworkManager>();
        room.RegisterGameData(this);
    }

    public void RegisterPlayers(List<NewNetworkGamePlayer> players) {
        foreach (var player in players) {
            var newPlayerData = new PlayerData(player);
            playerData.Add(newPlayerData);
            player.SetPlayerData(newPlayerData);
        }
    }

    public void UnregisterPlayer(NewNetworkGamePlayer player) {
        playerData.RemoveAt(playerData.FindIndex(p => p.Player.ID == player.ID));
    }

    public void SetGameStarted() {
        state = GameStates.Started;
    }

    public void NextTurn() {
        if (!isServer) return;
        var player = SetNextPlayer();
        if (player.TurnNumber > 1) {
        }
    }

    private PlayerData SetNextPlayer() {
        if (currentPlayer == null) {
            ServerSetCurrentPlayer(0);
            return playerData[0];
        }

        var currentPlayerIndex = playerData.FindIndex(p => p.Player.ID == currentPlayer.Player.ID);
        currentPlayerIndex = (currentPlayerIndex + 1) % room.GamePlayers.Count;

        ServerSetCurrentPlayer(currentPlayerIndex);
            
        return playerData[currentPlayerIndex];
    }

    private void ServerSetCurrentPlayer(int playerIndex) {
        if (currentPlayer != null) {
            currentPlayer.Player.IsCurrentPlayer = false;
        }
            
        playerData[playerIndex].Player.IsCurrentPlayer = true;
        playerData[playerIndex].TurnNumber++;
        RpcSetCurrentPlayer((Int16)playerData[playerIndex].Player.ID, (Int16)playerData[playerIndex].TurnNumber);
    }

    [ClientRpc]
    private void RpcSetCurrentPlayer(Int16 playerId, Int16 turnNumber) {
        currentPlayer = playerData.First(p => p.Player.ID == playerId);
        currentPlayer.TurnNumber = turnNumber;
        foreach (var p in playerData) {
            var isCurrentPlayer = p.Player.ID == playerId;
            if (isCurrentPlayer) {
                currentPlayer = p;
                p.Player.IsCurrentPlayer = true;
            }
            p.Player.UpdateCurrentPlayerStatus(isCurrentPlayer);
        }
    }
}