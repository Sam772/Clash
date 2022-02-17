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
    [Scene] [SerializeField] private string mapOneScene;
    [Scene] [SerializeField] private string mapTwoScene;
    [Scene] [SerializeField] private string mapThreeScene;
    [Scene] [SerializeField] private string mapFourScene;
    [SerializeField] private NewNetworkRoomPlayer roomPlayerPrefab;
    [SerializeField] private NewNetworkGamePlayer gamePlayerPrefab;
    [SerializeField] private PhysicalUnit captainPrefab;
    [SerializeField] private PhysicalUnit knightPrefab;
    [SerializeField] private PhysicalUnit archerPrefab;
    [SerializeField] private MagicalUnit arcanistPrefab;
    [SerializeField] private PhysicalUnit rangerPrefab;
    [SerializeField] private PhysicalUnit warriorPrefab;
    [SerializeField] private PhysicalUnit paladinPrefab;
    [SerializeField] private PhysicalUnit dragoonPrefab;
    [SerializeField] private MagicalUnit sorcererPrefab;
    [SerializeField] private LogTerrain logPrefab;
    [SerializeField] private BoulderTerrain boulderPrefab;
    [SerializeField] private HealingPotTerrain healingPotPrefab;
    [SerializeField] private StoneCrackedTerrain stoneCrackedPrefab;
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

    public void MapOneClicked() {
        gameScene = mapOneScene;
    }
    public void MapTwoClicked() {
        gameScene = mapTwoScene;
    }

    public void MapThreeClicked() {
        gameScene = mapThreeScene;
    }

    public void MapFourClicked() {
        gameScene = mapFourScene;
    }

    private void SetupGame() {
        for (int i = GamePlayers.Count - 1; i >= 0; i--) {
            var conn = GamePlayers[i].connectionToClient;
            // Map One
            if (gameScene == mapOneScene) {
                // If Host (Player One)
                if (i == 1) {
                    int x = 1;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit knight = Instantiate(knightPrefab, new Vector3(x+=2, 0.88f, 1), Quaternion.identity);
                        NetworkServer.Spawn(knight.gameObject, conn);
                    }
                    int x2 = 1;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit archer = Instantiate(archerPrefab, new Vector3(x2+=2, 0.88f, 0), Quaternion.identity);
                        NetworkServer.Spawn(archer.gameObject, conn);
                    }
                    PhysicalUnit captain = Instantiate(captainPrefab, new Vector3(4, 0.88f, 1), Quaternion.identity); 
                    NetworkServer.Spawn(captain.gameObject, conn);
                    int x3 = 1;
                    for (int k = 0; k < 2; k++) {
                        LogTerrain log = Instantiate(logPrefab, new Vector3(x3+=2, 0.88f, 5), Quaternion.identity);
                        NetworkServer.Spawn(log.gameObject, conn);
                    }
                    // If Client (Player Two)
                } else if (i == 0) {
                    int x = 1;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit knight = Instantiate(knightPrefab, new Vector3(x+=2, 0.88f, 8), Quaternion.identity);
                        NetworkServer.Spawn(knight.gameObject, conn);
                        knight.GetComponent<PhysicalUnit>().team = 1;
                    }
                    int x2 = 1;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit archer = Instantiate(archerPrefab, new Vector3(x2+=2, 0.88f, 9), Quaternion.identity);
                        NetworkServer.Spawn(archer.gameObject, conn);
                        archer.GetComponent<PhysicalUnit>().team = 1;
                    }
                    PhysicalUnit captain = Instantiate(captainPrefab, new Vector3(4, 0.88f, 8), Quaternion.identity); 
                    NetworkServer.Spawn(captain.gameObject, conn);
                    captain.GetComponent<PhysicalUnit>().team = 1;
                }
                // Map Two
            } else if (gameScene == mapTwoScene) {
                // If Host (Player One)
                if (i == 1) {
                    int x = 5;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit ranger = Instantiate(rangerPrefab, new Vector3(x+=2, 0.88f, 9), Quaternion.identity);
                        NetworkServer.Spawn(ranger.gameObject, conn);
                    }
                    MagicalUnit arcanist = Instantiate(arcanistPrefab, new Vector3(8, 0.90f, 8), Quaternion.identity);
                    NetworkServer.Spawn(arcanist.gameObject, conn);
                    PhysicalUnit warrior = Instantiate(warriorPrefab, new Vector3(8, 0.88f, 10), Quaternion.identity);
                    NetworkServer.Spawn(warrior.gameObject, conn);
                    BoulderTerrain boulder = Instantiate(boulderPrefab, new Vector3(9, 0.85f, 17), Quaternion.identity);
                    NetworkServer.Spawn(boulder.gameObject, conn);
                    // If Client (Player Two)
                } else if (i == 0) {
                   int x = 8;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit ranger = Instantiate(rangerPrefab, new Vector3(x+=2, 0.88f, 24), Quaternion.identity);
                        NetworkServer.Spawn(ranger.gameObject, conn);
                        ranger.GetComponent<PhysicalUnit>().team = 1;
                    }
                    MagicalUnit arcanist = Instantiate(arcanistPrefab, new Vector3(11, 0.88f, 25), Quaternion.identity);
                    NetworkServer.Spawn(arcanist.gameObject, conn);
                    arcanist.GetComponent<MagicalUnit>().team = 1;
                    PhysicalUnit warrior = Instantiate(warriorPrefab, new Vector3(11, 0.88f, 23), Quaternion.identity);
                    NetworkServer.Spawn(warrior.gameObject, conn);
                    warrior.GetComponent<PhysicalUnit>().team = 1;
                }
                // Map Three
            } else if (gameScene == mapThreeScene) {
                // If Host (Player One)
                if (i == 1) {
                    int x = 1;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit captain = Instantiate(captainPrefab, new Vector3(x+=2, 0.88f, 1), Quaternion.identity);
                        NetworkServer.Spawn(captain.gameObject, conn);
                    }
                    HealingPotTerrain healingPot = Instantiate(healingPotPrefab, new Vector3(4, 0.88f, 2), Quaternion.identity);
                    NetworkServer.Spawn(healingPot.gameObject, conn);
                    // If Client (Player Two)
                } else if (i == 0) {
                   int x = 1;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit captain = Instantiate(captainPrefab, new Vector3(x+=2, 0.88f, 3), Quaternion.identity);
                        NetworkServer.Spawn(captain.gameObject, conn);
                        captain.GetComponent<PhysicalUnit>().team = 1;
                    } 
                }
                // Map Four
            } else if (gameScene == mapFourScene) {
                // If Host (Player One)
                if (i == 1) {
                    int z = 2;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit archer = Instantiate(archerPrefab, new Vector3(4, 0.88f, z+=3), Quaternion.identity);
                        NetworkServer.Spawn(archer.gameObject, conn);
                    }
                    float x3 = 0f;
                    for (int k = 0; k < 2; k++) {
                        StoneCrackedTerrain stoneCracked = Instantiate(stoneCrackedPrefab, new Vector3(x3+=8f, 1f, 7), Quaternion.identity);
                        NetworkServer.Spawn(stoneCracked.gameObject);
                    }
                    // If Client (Player Two)
                } else if (i == 0) {
                   int z2 = 2;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit archer = Instantiate(archerPrefab, new Vector3(20, 0.88f, z2+=3), Quaternion.identity);
                        NetworkServer.Spawn(archer.gameObject, conn);
                        archer.GetComponent<PhysicalUnit>().team = 1;
                    } 
                }
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