using Mirror;

public class NewNetworkGamePlayer : NetworkBehaviour {
    public PlayerData PlayerData { get; private set; }

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    private string displayName;
    
    [SyncVar]
    public int ID;
    private NewNetworkManager room;
    private GameManager game;

    public override void OnStartClient() {
        DontDestroyOnLoad(gameObject);

        room = FindObjectOfType<NewNetworkManager>();
        room.AddGamePlayer(this);
    }

    public void Init(GameManager game) {
        this.game = game;
        if (hasAuthority) {
            game.SetGamePlayer(this);
        }
    }
    public void SetPlayerData(PlayerData playerData) => PlayerData = playerData;

    [Server]
    public void SetPlayerId(int id) {
        ID = id;
    }

    [Server]
    public void SetDisplayName(string displayName) {
        this.displayName = displayName;
    }

    private void HandleDisplayNameChanged(string oldName, string newName) {
        if (game == null) return;
    }
}