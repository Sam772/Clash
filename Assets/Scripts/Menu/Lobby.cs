using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Lobby : MenuScreen {

    #pragma warning disable 649
    [SerializeField] private LobbyPlayerPanel player1;
    [SerializeField] private LobbyPlayerPanel player2;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    #pragma warning restore 649
        
    public interface IObserver {
        void OnBackClicked();
        void OnReadyClicked();
        void OnStartGameClicked();
    }
        
    private readonly List<IObserver> observers = new List<IObserver>();

    public void RegisterObserver(IObserver observer) => observers.Add(observer);
    public void UnregisterObserver(IObserver observer) => observers.Remove(observer);
        
    public void ReadyButtonClicked() {
        foreach (var observer in observers) {
            observer.OnReadyClicked();
        }
    }

    public void BackButtonClicked() {
        foreach (var observer in observers) {
            observer.OnBackClicked();
        }
        Menu.ReturnToPlayScreenClicked();
    }

    public void StartGameButtonClicked() {
        foreach (var observer in observers) {
            observer.OnStartGameClicked();
        }
    }

    public void SetStartGameButtonEnabled(bool isEnabled) {
        startGameButton.interactable = isEnabled;
    }

    public void SetStartGameButtonVisible(bool isVisible) {
        startGameButton.gameObject.SetActive(isVisible);
        readyButton.gameObject.SetActive(!isVisible);
    }

    public void ClearPlayers() {
        player1.SetDisplayName("Waiting...");
        player1.SetReadyStatus("");
        player2.SetDisplayName("Waiting...");
        player2.SetReadyStatus("");
    }

    public void SetupPlayer(int playerIndex, NewNetworkRoomPlayer player) {
        if (playerIndex == 0) {
            SetupPlayer(player1, player);
        } else {
            SetupPlayer(player2, player);
        }
    }

    private static void SetupPlayer(LobbyPlayerPanel panel, NewNetworkRoomPlayer player) {
        panel.SetDisplayName(player.DisplayName);
        var readyText = player.IsReady
            ? "<color=blue>Ready</color>"
            : "<color=red>Not Ready</color>";
                
         panel.SetReadyStatus(readyText);
    }
}