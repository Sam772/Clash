using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class NewNetworkManager : NetworkManager {

    #pragma warning disable 649
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene;
    [Scene] [SerializeField] private string gameScene;

    [SerializeField] private Material playerTwoMat;
    [SerializeField] private NewNetworkRoomPlayer roomPlayerPrefab;
    [SerializeField] private NewNetworkGamePlayer gamePlayerPrefab;

    [SerializeField] private Unit captainPrefab;
    [SerializeField] private Unit knightPrefab;
    [SerializeField] private Unit archerPrefab;

    private Material unitTwoMaterial;

    //[SerializeField] private PlayerManager prefab;
    //[SerializeField] private TileMap boardPrefab;
    [SerializeField] private GameData gameDataPrefab;
    #pragma warning restore 649

    public readonly List<NewNetworkRoomPlayer> RoomPlayers = new List<NewNetworkRoomPlayer>();
    public readonly List<NewNetworkGamePlayer> GamePlayers = new List<NewNetworkGamePlayer>();
    public GameManager Game { get; private set; }
    private MainMenu mainMenu;
    private GameManager.Dependencies gameManagerDependencies;
    private bool isStartGameActioned;
    private bool isAllPlayersLoadedInGame;

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

    public void ConnectToTestServer(bool connectAsHost) {
        if (connectAsHost) {
            StartCoroutine(SetupTestServerRoutine());
        }
        else {
            StartClient();
        }
    }

    private IEnumerator SetupTestServerRoutine() {
        StartHost();
        while (GamePlayers.Count < minPlayers) {
            Debug.Log("[Test Server] waiting for second player");
            yield return new WaitForSeconds(1f);
        }
        SetupGame();
        GamePlayers[0].SetDisplayName("Host");
        GamePlayers[0].SetPlayerId(1);
        GamePlayers[1].SetDisplayName("Client");
        GamePlayers[1].SetPlayerId(2);
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
                int x = 3;
                for (int j = 0; j < 3; j++) {
                    Unit knight = Instantiate(knightPrefab, new Vector3(x++, 0.75f, 0), Quaternion.identity);
                    NetworkServer.Spawn(knight.gameObject, conn);
                }
                // If Client (Player Two)
                } else if (i == 0) {
                    int x = 3;
                    for (int j = 0; j < 3; j++) {
                        Unit knight = Instantiate(knightPrefab, new Vector3(x++, 0.75f, 9), Quaternion.identity);
                        NetworkServer.Spawn(knight.gameObject, conn);
                        knight.GetComponent<Unit>().unitMaterial = playerTwoMat;
                        knight.GetComponent<Unit>().teamNum = 1;
                        // unitTwoMaterial = new Material(knight.GetComponent<MeshRenderer>().material);
                        // unitTwoMaterial.color = knight.GetComponent<Unit>().unitTwoColour;
                        // knight.GetComponent<MeshRenderer>().material = unitTwoMaterial;
                        //knight.GetComponent<Unit>().unitTwoColour = Color.red;
                        knight.GetComponent<MeshRenderer>().material = playerTwoMat;
                        //UpdateColour(knight);
                    }
                    Unit archer = Instantiate(archerPrefab, new Vector3(3, 0.75f, 8), Quaternion.identity);
                    NetworkServer.Spawn(archer.gameObject, conn);
                    archer.GetComponent<Unit>().unitMaterial = playerTwoMat;
                    archer.GetComponent<Unit>().teamNum = 1;
                    archer.GetComponent<MeshRenderer>().material = playerTwoMat;
                }
        }
        var gameData = Instantiate(gameDataPrefab);
        NetworkServer.Spawn(gameData.gameObject);
    }

    // [ClientRpc]
    // public void UpdateColour(Unit unit){
    //     unit.GetComponent<MeshRenderer>().material = playerTwoMat;
    // }

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

    [Server]
    public void UpdatePlayerReady() {
        if (GamePlayers.Count == minPlayers && GamePlayers.All(p => p.IsReady)) {
            Game.StartGameServer();
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