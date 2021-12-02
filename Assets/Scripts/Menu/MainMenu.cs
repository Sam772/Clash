using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    #pragma warning disable 649
    [SerializeField] private TitleScreen titleScreen;
    [SerializeField] private HostScreen hostScreen;
    [SerializeField] private JoinScreen joinScreen;
    [SerializeField] private Lobby lobby;
    #pragma warning restore 649

    private NewNetworkManager networkManager;
    private MenuScreen currentScreen;
    public Lobby Lobby => lobby;

    private void Awake() {
        networkManager = FindObjectOfType<NewNetworkManager>();
        ReturnToMainScreenClicked();
    }

    private void Start() {
        networkManager.RegisterMainMenu(this);
            
        titleScreen.Setup(this);
        hostScreen.Setup(this);
        joinScreen.Setup(this);
        lobby.Setup(this);
    }

    public void HostGameClicked() => ShowScreen(hostScreen);
    public void JoinGameClicked() => ShowScreen(joinScreen);
    public void ReturnToMainScreenClicked() => ShowScreen(titleScreen);
    public void ShowLobby() => ShowScreen(lobby);
        
    public void ExitGameClicked() {
        Application.Quit();
    }

    public void OnClientDisconnect() {
        switch (currentScreen) {
            case Lobby _:
                ReturnToMainScreenClicked();
                break;
            case HostScreen _:
                hostScreen.ResetValidityState();
                break;
            case JoinScreen _:
                joinScreen.ResetValidityStates();
                break;
        }
    }

    private void ShowScreen(MenuScreen screen) {
        if (currentScreen == screen) return;
            
        if (screen != lobby) lobby.Hide();
        if (screen != hostScreen) hostScreen.Hide();
        if (screen != titleScreen) titleScreen.Hide();
        if (screen != joinScreen) joinScreen.Hide();
            
        screen.Show();
        currentScreen = screen;
    }
}