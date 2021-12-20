using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class NewNetworkManager : NetworkManager {

    #pragma warning disable 649
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene;
    [Scene] [SerializeField] private string gameScene;
    [SerializeField] private NewNetworkRoomPlayer roomPlayerPrefab;
    [SerializeField] private NewNetworkGamePlayer gamePlayerPrefab;
    [SerializeField] private Unit captainPrefab;
    [SerializeField] private Unit knightPrefab;
    [SerializeField] private Unit archerPrefab;
    private Material unitTwoMaterial;
    [SerializeField] private GameData gameDataPrefab;
    #pragma warning restore 649
    public readonly List<NewNetworkRoomPlayer> RoomPlayers = new List<NewNetworkRoomPlayer>();
    public readonly List<NewNetworkGamePlayer> GamePlayers = new List<NewNetworkGamePlayer>();
    public GameManager Game { get; private set; }
    private MainMenu mainMenu;
    private GameManager.Dependencies gameManagerDependencies;
    private bool isStartGameActioned;

    public override void Awake() {
        base.Awake();
        gameManagerDependencies.NetworkManager = this;
    }

    public void RegisterMainMenu(MainMenu mainMenu) {
        this.mainMenu = mainMenu;
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        var player = SceneManager.GetActiveScene().path == menuScene
            ? Instantiate(roomPlayerPrefab).gameObject
            : Instantiate(gamePlayerPrefab).gameObject;

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        mainMenu.OnClientDisconnect();
    }

    public void AddRoomPlayer(NewNetworkRoomPlayer player) {
        player.IsLeader = RoomPlayers.Count == 0;
        RoomPlayers.Add(player);
    }

    public void RemoveRoomPlayer(NewNetworkRoomPlayer player) => RoomPlayers.Remove(player);

    public void AddGamePlayer(NewNetworkGamePlayer player) {
        GamePlayers.Add(player);
    }

    public void RemoveGamePlayer(NewNetworkGamePlayer player) {
        GamePlayers.Remove(player);
        Game.Data.UnregisterPlayer(player);
    }

    public void ShowLobby() => mainMenu.ShowLobby();
    public bool IsLobbyReady() => RoomPlayers.Count == minPlayers && RoomPlayers.All(p => p.IsReady);
    
    public void StartGameFromLobby() {
        if (isStartGameActioned) return;
            
        if (SceneManager.GetActiveScene().path == menuScene) {
            if (!IsLobbyReady()) return;
                ServerChangeScene(gameScene);
        }
    }

    public override void ServerChangeScene(string newSceneName) {
        if (gameScene == newSceneName) {
            isStartGameActioned = true;
            for (int i = RoomPlayers.Count - 1; i >= 0; i--) {
                var gamePlayer = Instantiate(gamePlayerPrefab);
                gamePlayer.SetDisplayName(RoomPlayers[i].DisplayName);
                gamePlayer.SetPlayerId(i + 1);

                var conn = RoomPlayers[i].connectionToClient;
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayer.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }
    
    public override void OnServerSceneChanged(string sceneName) {
        if (sceneName == gameScene) {
            SetupGame();
        }
    }

    private void SetupGame() {
        for (int i = GamePlayers.Count - 1; i >= 0; i--) {
            var conn = GamePlayers[i].connectionToClient;
            // If Host (Player One)
            if (i == 1) {
                int x = 1;
                for (int j = 0; j < 2; j++) {
                    Unit knight = Instantiate(knightPrefab, new Vector3(x+=2, 0.75f, 1), Quaternion.identity);
                    NetworkServer.Spawn(knight.gameObject, conn);
                }
                int x2 = 1;
                for (int j = 0; j < 2; j++) {
                    Unit archer = Instantiate(archerPrefab, new Vector3(x2+=2, 0.75f, 0), Quaternion.identity);
                    NetworkServer.Spawn(archer.gameObject, conn);
                }
                Unit captain = Instantiate(captainPrefab, new Vector3(4, 0.75f, 1), Quaternion.identity); 
                NetworkServer.Spawn(captain.gameObject, conn);
                // If Client (Player Two)
                } else if (i == 0) {
                    int x = 1;
                    for (int j = 0; j < 2; j++) {
                        Unit knight = Instantiate(knightPrefab, new Vector3(x+=2, 0.75f, 8), Quaternion.identity);
                        NetworkServer.Spawn(knight.gameObject, conn);
                        knight.GetComponent<Unit>().team = 1;
                    }
                    int x2 = 1;
                    for (int j = 0; j < 2; j++) {
                    Unit archer = Instantiate(archerPrefab, new Vector3(x2+=2, 0.75f, 9), Quaternion.identity);
                    NetworkServer.Spawn(archer.gameObject, conn);
                    archer.GetComponent<Unit>().team = 1;
                    }
                    Unit captain = Instantiate(captainPrefab, new Vector3(4, 0.75f, 8), Quaternion.identity); 
                    NetworkServer.Spawn(captain.gameObject, conn);
                    captain.GetComponent<Unit>().team = 1;
                }
        }
        var gameData = Instantiate(gameDataPrefab);
        NetworkServer.Spawn(gameData.gameObject);
    }

    public void RegisterGameData(GameData data) {
        gameManagerDependencies.Data = data;
        CheckGameManagerDependencies();
        data.RegisterPlayers(GamePlayers);
    }

    private void CheckGameManagerDependencies() {
        if (gameManagerDependencies.IsValid()) {
            StartCoroutine(SetDependenciesAfterGameManagerInstantiated());
        }
    }

    private IEnumerator SetDependenciesAfterGameManagerInstantiated() {
        while (Game == null) {
            Game = FindObjectOfType<GameManager>();
            yield return null;
        }

        foreach (var player in GamePlayers) {
            player.Init(Game);
        }    
        Game.SetDependencies(gameManagerDependencies);
    }
}