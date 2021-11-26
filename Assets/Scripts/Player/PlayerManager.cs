using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerManager : NetworkBehaviour {
    [SerializeField] public Text status;

    // speed of player
    public float speed = 5;
    public bool currentPlayer;

    [SyncVar(hook = nameof(OnCanPlayChanged))]
    public bool canPlay = false;

    static readonly List<PlayerManager> players = new List<PlayerManager>();

    public override void OnStartServer() {
        // add a check to set canPlay to both players when they join
        players.Add(this);
        if (players.Count == 2)
            canPlay = true;
    }

    public override void OnStopClient() {
        Debug.Log("Client stopped");
        // set status text back to loading
        status.text = "Loading";
    }

    void OnCanPlayChanged(bool _, bool newValue) {
        if (!isLocalPlayer) return;

        if (status == null)
            status = GameObject.Find("Status").GetComponent<Text>();

        status.text = "Your turn: " + newValue;
    }

    private void Update() {

        if (status == null)
            status = GameObject.Find("Status").GetComponent<Text>();
        if (status.text == "Loading")
            CmdChangeTurn();

        if (isLocalPlayer && canPlay && Input.GetKeyDown(KeyCode.Z))
        CmdChangeTurn();
        PlayerCanMove();

    }

    public void PlayerCanMove() {

        currentPlayer = !currentPlayer;

        if (!isLocalPlayer) return;

        if (canPlay) {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 dir = new Vector3(h, 0, v);
            transform.position += dir.normalized * (Time.deltaTime * speed);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeTurn() {
        foreach (PlayerManager pm in players)
            pm.canPlay = (pm != this);
    }
}
