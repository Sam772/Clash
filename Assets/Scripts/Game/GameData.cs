using System.Collections.Generic;
using Mirror;

public class GameData : NetworkBehaviour {
    private NewNetworkManager room;
    private readonly List<PlayerData> playerData = new List<PlayerData>();

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
}