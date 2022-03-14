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
    [SerializeField] private PhysicalUnit championPrefab;
    [SerializeField] private MagicalUnit elderPrefab;
    [SerializeField] private PhysicalUnit clasherPrefab;
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
                    int x2 = 4;
                    for (int k = 0; k < 3; k++) {
                        MagicalUnit arcanist = Instantiate(arcanistPrefab, new Vector3(x2+=2, 0.90f, 8), Quaternion.identity);
                        NetworkServer.Spawn(arcanist.gameObject, conn);
                    }
                    PhysicalUnit warrior = Instantiate(warriorPrefab, new Vector3(8, 0.88f, 10), Quaternion.identity);
                    NetworkServer.Spawn(warrior.gameObject, conn);
                    BoulderTerrain boulder = Instantiate(boulderPrefab, new Vector3(9, 0.85f, 17), Quaternion.identity);
                    NetworkServer.Spawn(boulder.gameObject, conn);
                    BoulderTerrain boulder2 = Instantiate(boulderPrefab, new Vector3(14, 0.85f, 14), Quaternion.identity);
                    NetworkServer.Spawn(boulder2.gameObject, conn);
                    BoulderTerrain boulder3 = Instantiate(boulderPrefab, new Vector3(7, 0.85f, 6), Quaternion.identity);
                    NetworkServer.Spawn(boulder3.gameObject, conn);
                    BoulderTerrain boulder4 = Instantiate(boulderPrefab, new Vector3(5, 0.85f, 22), Quaternion.identity);
                    NetworkServer.Spawn(boulder4.gameObject, conn);
                    // If Client (Player Two)
                } else if (i == 0) {
                    int x = 8;
                    for (int j = 0; j < 2; j++) {
                        PhysicalUnit ranger = Instantiate(rangerPrefab, new Vector3(x+=2, 0.88f, 24), Quaternion.identity);
                        NetworkServer.Spawn(ranger.gameObject, conn);
                        ranger.GetComponent<PhysicalUnit>().team = 1;
                    }
                    int x2 = 9;
                    for (int k = 0; k < 3; k++) {
                        MagicalUnit arcanist = Instantiate(arcanistPrefab, new Vector3(x2+=1, 0.88f, 25), Quaternion.identity);
                        NetworkServer.Spawn(arcanist.gameObject, conn);
                        arcanist.GetComponent<MagicalUnit>().team = 1;
                    }
                    PhysicalUnit warrior = Instantiate(warriorPrefab, new Vector3(11, 0.88f, 23), Quaternion.identity);
                    NetworkServer.Spawn(warrior.gameObject, conn);
                    warrior.GetComponent<PhysicalUnit>().team = 1;
                }
                // Map Three
            } else if (gameScene == mapThreeScene) {
                // If Host (Player One)
                if (i == 1) {

                    // FOR TESTING PURPOSES
                    // int x = 1;
                    // for (int j = 0; j < 2; j++) {
                    //     PhysicalUnit paladin = Instantiate(paladinPrefab, new Vector3(x+=2, 0.88f, 1), Quaternion.identity);
                    //     NetworkServer.Spawn(paladin.gameObject, conn);
                    // }
                    // HealingPotTerrain healingPot = Instantiate(healingPotPrefab, new Vector3(4, 0.88f, 2), Quaternion.identity);
                    // NetworkServer.Spawn(healingPot.gameObject, conn);

                    // unit locations
                    int x = 0;
                    for (int j = 0; j < 3; j++) {
                        PhysicalUnit paladin = Instantiate(paladinPrefab, new Vector3(x+=1, 0.88f, 7), Quaternion.identity);
                        NetworkServer.Spawn(paladin.gameObject, conn);
                    }
                    PhysicalUnit dragoon = Instantiate(dragoonPrefab, new Vector3(2, 0.88f, 6), Quaternion.identity);
                    NetworkServer.Spawn(dragoon.gameObject, conn);
                    int x2 = -1;
                    for (int k = 0; k < 2; k++) {
                        MagicalUnit sorcerer = Instantiate(sorcererPrefab, new Vector3(x2+=2, 0.8f, 5), Quaternion.identity);
                        NetworkServer.Spawn(sorcerer.gameObject, conn);
                    }

                    // healing pot locations
                    HealingPotTerrain healingPot = Instantiate(healingPotPrefab, new Vector3(3, 0.88f, 1), Quaternion.identity);
                    NetworkServer.Spawn(healingPot.gameObject, conn);
                    HealingPotTerrain healingPot2 = Instantiate(healingPotPrefab, new Vector3(10, 0.88f, 4), Quaternion.identity);
                    NetworkServer.Spawn(healingPot2.gameObject, conn);
                    HealingPotTerrain healingPot3 = Instantiate(healingPotPrefab, new Vector3(7, 0.88f, 8), Quaternion.identity);
                    NetworkServer.Spawn(healingPot3.gameObject, conn);
                    HealingPotTerrain healingPot4 = Instantiate(healingPotPrefab, new Vector3(1, 0.88f, 13), Quaternion.identity);
                    NetworkServer.Spawn(healingPot4.gameObject, conn);
                    HealingPotTerrain healingPot5 = Instantiate(healingPotPrefab, new Vector3(3, 0.88f, 18), Quaternion.identity);
                    NetworkServer.Spawn(healingPot5.gameObject, conn);
                    HealingPotTerrain healingPot6 = Instantiate(healingPotPrefab, new Vector3(10, 0.88f, 19), Quaternion.identity);
                    NetworkServer.Spawn(healingPot6.gameObject, conn);

                    // If Client (Player Two)
                } else if (i == 0) {

                    // FOR TESTING PURPOSES
                    // int x = 1;
                    // for (int j = 0; j < 2; j++) {
                    //     PhysicalUnit paladin = Instantiate(paladinPrefab, new Vector3(x+=2, 0.88f, 3), Quaternion.identity);
                    //     NetworkServer.Spawn(paladin.gameObject, conn);
                    //     paladin.GetComponent<PhysicalUnit>().team = 1;
                    // }

                    // unit locations
                    int x = 6;
                    for (int j = 0; j < 3; j++) {
                        PhysicalUnit paladin = Instantiate(paladinPrefab, new Vector3(x+=1, 0.88f, 15), Quaternion.identity);
                        NetworkServer.Spawn(paladin.gameObject, conn);
                        paladin.GetComponent<PhysicalUnit>().team = 1;
                    }
                    PhysicalUnit dragoon = Instantiate(dragoonPrefab, new Vector3(8, 0.88f, 16), Quaternion.identity);
                    NetworkServer.Spawn(dragoon.gameObject, conn);
                    dragoon.GetComponent<PhysicalUnit>().team = 1;
                    int x2 = 5;
                    for (int k = 0; k < 2; k++) {
                        MagicalUnit sorcerer = Instantiate(sorcererPrefab, new Vector3(x2+=2, 0.8f, 17), Quaternion.identity);
                        NetworkServer.Spawn(sorcerer.gameObject, conn);
                        sorcerer.GetComponent<MagicalUnit>().team = 1;
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