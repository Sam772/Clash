using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinScreen : MenuScreen {
    #pragma warning disable 649
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField hostIPInput;
    [SerializeField] private Button joinLobbyButton;
    #pragma warning restore 649

    private void Awake() {
        playerNameInput.text = MenuUtil.GetNameFromPlayerPrefs();
    }

    public void PlayerNameInputChanged(string newPlayerName) {
        ResetValidityStates();
    }

    public void HostIPInputChanged(string hostIP) {
        ResetValidityStates();
    }
        
    public void JoinLobbyButtonClicked() {
        if (!IsInputsValid()) return;
            
        MenuUtil.SaveNameToPlayerPrefs(playerNameInput.text);

        var networkManager = FindObjectOfType<NewNetworkManager>();
        networkManager.networkAddress = hostIPInput.text;
        networkManager.StartClient();

        joinLobbyButton.interactable = false;
            
        // TODO restore on disconnect
    }

    public void ResetValidityStates() {
        joinLobbyButton.interactable = IsInputsValid();
    }
        
    protected override void OnShow() {
        ResetValidityStates();
    }

    private bool IsInputsValid() {
        return MenuUtil.IsPlayerNameValid(playerNameInput.text)
            && MenuUtil.IsValidIPAddress(hostIPInput.text);
    }
}