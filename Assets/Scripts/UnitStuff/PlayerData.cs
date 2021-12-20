using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
    public readonly NewNetworkGamePlayer Player;
    public int TurnNumber;
    public PlayerData(NewNetworkGamePlayer player) {
        Player = player;
    }
}
