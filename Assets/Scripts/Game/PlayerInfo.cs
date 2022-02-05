using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI playerName;
    public NewNetworkGamePlayer player { get; private set; }

    public void SetPlayerInfo(NewNetworkGamePlayer player) {
        this.player = player;
        playerName.text = player.DisplayName;
    }
}
