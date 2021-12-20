using Mirror;

public class NewNetworkRoomPlayer : NetworkBehaviour, Lobby.IObserver{
    public bool IsLeader = false;
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName;
    private NewNetworkManager room;
    private Lobby lobbyUI;

    private void Awake() {
        SetupReferences();
            
        lobbyUI.SetStartGameButtonVisible(false);
        lobbyUI.SetStartGameButtonEnabled(false);
    }

    public override void OnStartAuthority() {
        CmdSetDisplayName(MenuUtil.GetNameFromPlayerPrefs());
        room.ShowLobby();
    }

    public override void OnStartClient() {
        SetupLobbyCallbacks();
            
        room.AddRoomPlayer(this);
        UpdateDisplay();
    }

    public override void OnStopClient() {
        room.RemoveRoomPlayer(this);
    }

    public override void OnStopServer() {
        room.RoomPlayers.Clear();
    }

    private void HandleReadyStatusChanged(bool oldStatus, bool newStatus) => UpdateDisplay();
    private void HandleDisplayNameChanged(string oldName, string newName) => UpdateDisplay();

    [Command]
    private void CmdSetDisplayName(string displayName) {
        DisplayName = displayName;
    }

    [Command]
    private void CmdSetReadyStatus(bool newStatus) {
        IsReady = newStatus;
            
        if (IsLeader) {
            lobbyUI.SetStartGameButtonVisible(IsReady);
            lobbyUI.SetStartGameButtonEnabled(room.IsLobbyReady());
        }
    }

    private void UpdateDisplay() {
        if (!hasAuthority) {
            foreach (var player in room.RoomPlayers) {
                if (player.isLocalPlayer) {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        lobbyUI.ClearPlayers();

        for (int i = 0; i < room.RoomPlayers.Count; i++) {
            lobbyUI.SetupPlayer(i, room.RoomPlayers[i]);
        }
    }

    public void OnReadyClicked() {
        if (!hasAuthority) return;
        CmdSetReadyStatus(!IsReady);
    }

    public void OnBackClicked() {
        if (isServer) {
            room.StopHost();
        } else {
            room.StopClient();
        }
    }

    public void OnStartGameClicked() {
        room.StartGameFromLobby();
    }

    private void OnDestroy() {
        lobbyUI.UnregisterObserver(this);
    }

    private void SetupReferences() {
        room = FindObjectOfType<NewNetworkManager>();
            
        var menu = FindObjectOfType<MainMenu>();
        lobbyUI = menu.Lobby;
    }

    private void SetupLobbyCallbacks() {
        if (!hasAuthority) return;
        lobbyUI.RegisterObserver(this);
    }
}