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
        [SerializeField] private NewNetworkRoomPlayer roomPlayerPrefab;
        [SerializeField] private NewNetworkGamePlayer gamePlayerPrefab;

        [SerializeField] private Unit prefab;

        [SerializeField] private GameObject knightPrefab;

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


    //public Transform PositionOne;
    //public Transform PositionTwo;

    public override void Awake()
        {
            base.Awake();
            gameManagerDependencies.NetworkManager = this;
        }

    public void RegisterMainMenu(MainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
        }

    public override void OnServerAddPlayer(NetworkConnection conn) {

        var player = SceneManager.GetActiveScene().path == menuScene
                ? Instantiate(roomPlayerPrefab).gameObject
                : Instantiate(gamePlayerPrefab).gameObject;

            NetworkServer.AddPlayerForConnection(conn, player);

            // var unit = SceneManager.GetActiveScene().path == gameScene
            //         ? Instantiate(prefab).gameObject : Instantiate(prefab).gameObject;
            //         NetworkServer.AddPlayerForConnection(conn, unit);

        // add player at correct spawn position
        // Transform start = numPlayers == 0 ? PositionOne : PositionTwo;
        // GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        // NetworkServer.AddPlayerForConnection(conn, player);
    }

    // public override void OnServerConnect(NetworkConnection conn) {
    //     Debug.Log("Player connected to server");
    // }

    // public override void OnClientConnect(NetworkConnection conn) {
    //     Debug.Log("Client connected");
    // }

    public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            mainMenu.OnClientDisconnect();
        }

    public void AddRoomPlayer(NewNetworkRoomPlayer player)
    {
            player.IsLeader = RoomPlayers.Count == 0;
            RoomPlayers.Add(player);
    }

    public void RemoveRoomPlayer(NewNetworkRoomPlayer player) => RoomPlayers.Remove(player);

    public void AddGamePlayer(NewNetworkGamePlayer player)
        {
            GamePlayers.Add(player);
        }

        public void RemoveGamePlayer(NewNetworkGamePlayer player)
        {
            GamePlayers.Remove(player);
            Game.Data.UnregisterPlayer(player);
        }

    public void ShowLobby() => mainMenu.ShowLobby();
    public bool IsLobbyReady() => RoomPlayers.Count == minPlayers && RoomPlayers.All(p => p.IsReady);
    
    public void StartGameFromLobby()
        {
            if (isStartGameActioned) return;
            
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                if (!IsLobbyReady()) return;
                ServerChangeScene(gameScene);
            }
        }

    public void ConnectToTestServer(bool connectAsHost)
        {
            if (connectAsHost)
            {
                StartCoroutine(SetupTestServerRoutine());
            }
            else
            {
                StartClient();
            }
        }

    private IEnumerator SetupTestServerRoutine()
        {
            StartHost();
            while (GamePlayers.Count < minPlayers)
            {
                Debug.Log("[Test Server] waiting for second player");
                yield return new WaitForSeconds(1f);
            }
            SetupGame();
            GamePlayers[0].SetDisplayName("Host");
            GamePlayers[0].SetPlayerId(1);
            GamePlayers[1].SetDisplayName("Client");
            GamePlayers[1].SetPlayerId(2);
        }

    public override void ServerChangeScene(string newSceneName)
        {
            if (gameScene == newSceneName)
            {
                isStartGameActioned = true;
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    var gamePlayer = Instantiate(gamePlayerPrefab);
                    gamePlayer.SetDisplayName(RoomPlayers[i].DisplayName);
                    gamePlayer.SetPlayerId(i + 1);

                    var conn = RoomPlayers[i].connectionToClient;
                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayer.gameObject);
                    
                    // Transform start = numPlayers == 0 ? PositionOne : PositionTwo;
                    // var gamePlayer2 = Instantiate(prefab, start.position, start.rotation);
                    // NetworkServer.ReplacePlayerForConnection(conn, gamePlayer2.gameObject);

                    // Transform start = numPlayers == 0 ? PositionOne : PositionTwo;
                    // GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
                    // NetworkServer.AddPlayerForConnection(conn, player);

                    // var unit = SceneManager.GetActiveScene().path == gameScene
            //         ? Instantiate(prefab).gameObject : Instantiate(prefab).gameObject;
            //         NetworkServer.AddPlayerForConnection(conn, unit);
                }
            }

            base.ServerChangeScene(newSceneName);
        }
    
    public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName == gameScene)
            {
                SetupGame();
            }
        }

    private void SetupGame() {
        for (int i = GamePlayers.Count - 1; i >= 0; i--) {
            var conn = GamePlayers[i].connectionToClient;

            GameObject knight = Instantiate(knightPrefab);
            NetworkServer.Spawn(knight.gameObject, conn);
        }

            var gameData = Instantiate(gameDataPrefab);
            NetworkServer.Spawn(gameData.gameObject);
        }

    public void RegisterGameData(GameData data)
        {
            gameManagerDependencies.Data = data;
            CheckGameManagerDependencies();
            data.RegisterPlayers(GamePlayers);
        }

    private void CheckGameManagerDependencies()
        {
            if (gameManagerDependencies.IsValid())
            {
                StartCoroutine(SetDependenciesAfterGameManagerInstantiated());
            }
        }

        [Server]
        public void UpdatePlayerReady()
        {
            if (GamePlayers.Count == minPlayers && GamePlayers.All(p => p.IsReady))
            {
                Game.StartGameServer();
            }
        }

    private IEnumerator SetDependenciesAfterGameManagerInstantiated()
        {
            while (Game == null)
            {
                Game = FindObjectOfType<GameManager>();
                yield return null;
            }
            
            foreach (var player in GamePlayers)
            {
                player.Init(Game);
            }
            
            Game.SetDependencies(gameManagerDependencies);
        }
}
