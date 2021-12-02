using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {

    public readonly NewNetworkGamePlayer Player;
    public int TurnNumber;
    public Unit data;
    public PlayerData(NewNetworkGamePlayer player) {
        Player = player;
    }

    public PlayerData Clone() {
        var playerDataCopy = new PlayerData(Player) {
            TurnNumber = TurnNumber,
            //data = data.Clone(), 
        };

        // playerDataCopy.CurrencyData = new CurrencyData(playerDataCopy)
        // {
        //     Supplies = CurrencyData.Supplies,
        //     Production = CurrencyData.Production,
        //     Population = CurrencyData.Population,
        // };
        return playerDataCopy;
    }
}
