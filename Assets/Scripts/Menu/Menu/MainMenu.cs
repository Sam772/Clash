using UnityEngine;

public class MainMenu : MonoBehaviour {

    #pragma warning disable 649
    [SerializeField] private AccountScreen accountScreen;
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
        accountScreen.Setup(this);
        mainScreen.Setup(this);
        playScreen.Setup(this);
        settingsScreen.Setup(this);
        profileScreen.Setup(this);
        mapSelectionScreen.Setup(this);
        hostScreen.Setup(this);
        joinScreen.Setup(this);
        lobby.Setup(this);
        extrasScreen.Setup(this);
        tacticianTips.Setup(this);
    }

    public void BeginScreen() => ShowScreen(accountScreen);
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
        if (screen != accountScreen) accountScreen.Hide();
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

        screen.Show();
        currentScreen = screen;
    }
}