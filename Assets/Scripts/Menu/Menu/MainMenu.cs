using UnityEngine;

public class MainMenu : MonoBehaviour {

    #pragma warning disable 649
    [SerializeField] private AccountScreen accountScreen;
    [SerializeField] private DisplayNameScreen displayNameScreen;
    [SerializeField] private MainScreen mainScreen;
    [SerializeField] private MapSelectionScreen mapSelectionScreen;
    [SerializeField] private SettingsScreen settingsScreen; 
    [SerializeField] private ProfileScreen profileScreen; 
    [SerializeField] private PlayScreen playScreen;
    [SerializeField] private HostScreen hostScreen;
    [SerializeField] private JoinScreen joinScreen;
    [SerializeField] private Lobby lobby;
    [SerializeField] private ExtrasScreen extrasScreen;
    [SerializeField] private TacticianTipsScreen tacticianTips;
    [SerializeField] private UnitInfoScreen unitInfoScreen;
    [SerializeField] private MapInfoScreen mapInfoScreen;
    [SerializeField] private GameCreditsScreen gameCreditsScreen;
    [SerializeField] private CaptainInfoScreen captainInfoScreen;
    [SerializeField] private KnightInfoScreen knightInfoScreen;
    [SerializeField] private ArcherInfoScreen archerInfoScreen;
    [SerializeField] private ArcanistInfoScreen arcanistInfoScreen;
    [SerializeField] private RangerInfoScreen rangerInfoScreen;
    [SerializeField] private WarriorInfoScreen warriorInfoScreen;
    [SerializeField] private PaladinInfoScreen paladinInfoScreen;
    [SerializeField] private DragoonInfoScreen dragoonInfoScreen;
    [SerializeField] private SorcerorInfoScreen sorcerorInfoScreen;
    [SerializeField] private ChampionInfoScreen championInfoScreen;
    [SerializeField] private ElderInfoScreen elderInfoScreen;
    [SerializeField] private ClasherInfoScreen clasherInfoScreen;
    [SerializeField] private LogInfoScreen logInfoScreen;
    [SerializeField] private BoulderInfoScreen boulderInfoScreen;
    [SerializeField] private HealingPotInfoScreen healingPotInfoScreen;
    [SerializeField] private StoneCrackedWallInfoScreen stoneCrackedWallInfoScreen;



    #pragma warning restore 649

    private NewNetworkManager networkManager;
    private MenuScreen currentScreen;
    public Lobby Lobby => lobby;

    private void Awake() {
        networkManager = FindObjectOfType<NewNetworkManager>();
        BeginScreen();
    }

    private void Start() {
        networkManager.RegisterMainMenu(this);
        lobby.Setup(this);
        accountScreen.Setup(this);
        displayNameScreen.Setup(this);
        mainScreen.Setup(this);
        playScreen.Setup(this);
        settingsScreen.Setup(this);
        profileScreen.Setup(this);
        mapSelectionScreen.Setup(this);
        hostScreen.Setup(this);
        joinScreen.Setup(this);
        extrasScreen.Setup(this);
        tacticianTips.Setup(this);
    }

    public void BeginScreen() => ShowScreen(accountScreen);
    public void DisplayNameSent() => ShowScreen(displayNameScreen);
    public void LoginButtonClicked() => ShowScreen(mainScreen);
    public void PlayGameClicked() => ShowScreen(mapSelectionScreen);
    public void SettingsScreenClicked() => ShowScreen(settingsScreen);
    public void ProfileScreenClicked() => ShowScreen(profileScreen);
    public void MapClicked() => ShowScreen(playScreen);
    public void HostGameClicked() => ShowScreen(hostScreen);
    public void JoinGameClicked() => ShowScreen(joinScreen);
    public void ReturnToMainScreenClicked() => ShowScreen(mainScreen);
    public void ReturnToMapSelectionScreenClicked() => ShowScreen(mapSelectionScreen);
    public void ReturnToPlayScreenClicked() => ShowScreen(playScreen);
    public void ShowLobby() => ShowScreen(lobby);
    public void ExtrasScreenClicked() => ShowScreen(extrasScreen);
    public void HowToPlayScreenClicked() => ShowScreen(tacticianTips);
    public void UnitInfoScreenClicked() => ShowScreen(unitInfoScreen);
    public void MapInfoScreenClicked() => ShowScreen(mapInfoScreen);
    public void GameCreditsScreenClicked() => ShowScreen(gameCreditsScreen);
    public void CaptainInfoScreenClicked() => ShowScreen(captainInfoScreen);
    public void KnightInfoScreenClicked() => ShowScreen(knightInfoScreen);
    public void ArcherInfoScreenClicked() => ShowScreen(archerInfoScreen);
    public void ArcanistInfoScreenClicked() => ShowScreen(arcanistInfoScreen);
    public void RangerInfoScreenClicked() => ShowScreen(rangerInfoScreen);
    public void WarriorInfoScreenClicked() => ShowScreen(warriorInfoScreen);
    public void PaladinInfoScreenClicked() => ShowScreen(paladinInfoScreen);
    public void DragoonInfoScreenClicked() => ShowScreen(dragoonInfoScreen);
    public void SorcerorInfoScreenClicked() => ShowScreen(sorcerorInfoScreen);
    public void ChampionInfoScreenClicked() => ShowScreen(championInfoScreen);
    public void ElderInfoScreenClicked() => ShowScreen(elderInfoScreen);
    public void ClasherInfoScreenClicked() => ShowScreen(clasherInfoScreen);
    public void LogInfoScreenClicked() => ShowScreen(logInfoScreen);
    public void BoulderInfoScreenClicked() => ShowScreen(boulderInfoScreen);
    public void HealingPotInfoScreenClicked() => ShowScreen(healingPotInfoScreen);
    public void StoneCrackedWallInfoScreenClicked() => ShowScreen(stoneCrackedWallInfoScreen);
            
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
            
        if (screen != accountScreen) accountScreen.Hide();
        if (screen != displayNameScreen) displayNameScreen.Hide();
        if (screen != lobby) lobby.Hide();
        if (screen != hostScreen) hostScreen.Hide();
        if (screen != settingsScreen) settingsScreen.Hide();
        if (screen != profileScreen) profileScreen.Hide();
        if (screen != playScreen) playScreen.Hide();
        if (screen != mapSelectionScreen) mapSelectionScreen.Hide();
        if (screen != mainScreen) mainScreen.Hide();
        if (screen != joinScreen) joinScreen.Hide();
        if (screen != extrasScreen) extrasScreen.Hide();   
        if (screen != tacticianTips) tacticianTips.Hide();
        if (screen != unitInfoScreen) unitInfoScreen.Hide();
        if (screen != mapInfoScreen) mapInfoScreen.Hide();
        if (screen != gameCreditsScreen) gameCreditsScreen.Hide();
        if (screen != captainInfoScreen) captainInfoScreen.Hide();
        if (screen != knightInfoScreen) knightInfoScreen.Hide();
        if (screen != archerInfoScreen) archerInfoScreen.Hide();
        if (screen != arcanistInfoScreen) arcanistInfoScreen.Hide();
        if (screen != rangerInfoScreen) rangerInfoScreen.Hide();
        if (screen != warriorInfoScreen) warriorInfoScreen.Hide();
        if (screen != paladinInfoScreen) paladinInfoScreen.Hide();
        if (screen != dragoonInfoScreen) dragoonInfoScreen.Hide();
        if (screen != sorcerorInfoScreen) sorcerorInfoScreen.Hide();
        if (screen != championInfoScreen) championInfoScreen.Hide();
        if (screen != elderInfoScreen) elderInfoScreen.Hide();
        if (screen != clasherInfoScreen) clasherInfoScreen.Hide();
        if (screen != logInfoScreen) logInfoScreen.Hide();
        if (screen != boulderInfoScreen) boulderInfoScreen.Hide();
        if (screen != healingPotInfoScreen) healingPotInfoScreen.Hide();
        if (screen != stoneCrackedWallInfoScreen) stoneCrackedWallInfoScreen.Hide();

        screen.Show();
        currentScreen = screen;
    }
}