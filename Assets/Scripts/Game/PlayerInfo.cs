using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {
    [SerializeField] public TextMeshProUGUI playerName;
    public NewNetworkGamePlayer player { get; private set; }

    public bool isWinner = false;
    public void SetupPlayerInfo(NewNetworkGamePlayer player) {
        this.player = player;
        playerName.text = player.DisplayName;
    }
}
