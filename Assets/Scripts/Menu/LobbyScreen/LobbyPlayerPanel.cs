using UnityEngine;
using TMPro;

public class LobbyPlayerPanel : MonoBehaviour {

    #pragma warning disable 649
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private TextMeshProUGUI readyStatusText;
    #pragma warning restore 649
        
    public void SetDisplayName(string displayName) {
        displayNameText.text = displayName;
    }

    public void SetReadyStatus(string readyText) {
        readyStatusText.text = readyText;
    }
}