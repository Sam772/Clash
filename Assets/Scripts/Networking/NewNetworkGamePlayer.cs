using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

public class NewNetworkGamePlayer : NetworkBehaviour {
    
    public bool IsReady { get; private set; }
        public PlayerData PlayerData { get; private set; }
        
        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        private string displayName;
        
        [SyncVar] public int ID;

        public bool IsCurrentPlayer = false;
        
        private NewNetworkManager room;
        private GameManager game;

        private Int16 lastActionID = 0;
        
        private readonly List<MonoBehaviour> uiObjectsInMouseover = new List<MonoBehaviour>();

        public string DisplayName => displayName;

        public override void OnStartClient()
        {
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

        public Int16 GetNewActionID() => lastActionID++;
        public void SetPlayerData(PlayerData playerData) => PlayerData = playerData;

        public override void OnStopClient()
        {
            room.RemoveGamePlayer(this);
        }

        [Server]
        public void SetPlayerId(int id)
        {
            ID = id;
        }

        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

        private void Update()
        {
            if (!hasAuthority || !game.IsRunning()) return;
            //actions.Update();
        }

        public void UpdateCurrentPlayerStatus(bool isCurrentPlayer)
        {
            IsCurrentPlayer = isCurrentPlayer;
            
            if (hasAuthority)
            {
                //actions.UpdatePlayerTurn(isCurrentPlayer);
            }
        }

        private void HandleDisplayNameChanged(string oldName, string newName)
        {
            if (game == null) return; // When created by the network manager, the main scene hasn't loaded yet
            //game.UI.UpdatePlayerInfo(room.GamePlayers);
        }

        [ClientRpc]
        public void RpcNotifyGameStart()
        {
            game.StartGameClient();
        }

        [Command]
        public void CmdSetReady()
        {
            IsReady = true;
            room.UpdatePlayerReady();
        }

        [Command]
        private void CmdEndTurn()
        {
            if (!IsCurrentPlayer) return;
            game.Data.NextTurn();
        }
}
