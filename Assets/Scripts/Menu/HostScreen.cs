using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostScreen : MenuScreen {
    #pragma warning disable 649
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button createLobbyButton;
    #pragma warning restore 649

    private void Awake() {
        playerNameInput.text = MenuUtil.GetNameFromPlayerPrefs();
        createLobbyButton.interactable = false;
    }

    public void PlayerNameInputChanged(string newPlayerName) {
        ResetValidityState();
    }

    public void CreateLobbyButtonClicked() {
        if (!MenuUtil.IsPlayerNameValid(playerNameInput.text)) return;
        MenuUtil.SaveNameToPlayerPrefs(playerNameInput.text);

        var networkManager = FindObjectOfType<NewNetworkManager>();
        networkManager.StartHost();
    }

    public void ResetValidityState() {
        createLobbyButton.interactable = MenuUtil.IsPlayerNameValid(playerNameInput.text);
    }

    protected override void OnShow() {
        base.OnShow();
        ResetValidityState();
    }
}