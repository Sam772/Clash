using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public abstract class GenericUnit : NetworkBehaviour {
    [SyncVar]
    public int team;
    [SyncVar]
    public int x;
    [SyncVar]
    public int y;
    public Queue<int> movementQueue;
    public Queue<int> combatQueue;
    public float visualMovementSpeed = .15f;
    public GameObject tileBeingOccupied;
    public string unitName;
    public int maxHealth;
    public int defence;
    public int resistance;
    public int move;
    public int range;
    [SyncVar]
    public int currentHealth;
    public Sprite unitSprite;
    [Header("Unit Health Bar")]
    public Canvas healthBarCanvas;
    public TMP_Text hitPointsText;
    public Image healthBar;
    public GenericTileMap map;
    public GameObject holder2D;
    public bool permSkill;
    public enum MovementStates {
        Unselected,
        Selected,
        Moved,
        Wait
    }
    public MovementStates unitMoveState;
    public List<Node> path = null;
    private void Awake() {
        movementQueue = new Queue<int>();
        combatQueue = new Queue<int>();
        x = (int) transform.position.x;
        y = (int) transform.position.z;
        unitMoveState = MovementStates.Unselected;
        currentHealth = maxHealth;
        hitPointsText.SetText(currentHealth.ToString());
    }

    public void LateUpdate() {
        healthBarCanvas.transform.forward = Camera.main.transform.forward;
        holder2D.transform.forward = Camera.main.transform.forward;
    }

    public void MoveNextTile() {
        if (!hasAuthority) return;
        if (path.Count == 0) { return; }
        else { StartCoroutine(UnitMoveSequence(transform.gameObject, path[path.Count - 1])); }
    }

    [Command(requiresAuthority=false)]
    public void CmdUpdateNewPosition(int newX, int newY) {
        tileBeingOccupied.GetComponent<TileClick>().unitOnTile = null;
        x = newX;
        y = newY;
    }

    [ClientRpc]
    public void RpcDeleteOldPosition() {
        if (!isServer) { tileBeingOccupied.GetComponent<TileClick>().unitOnTile = null; }
    }

    public void MoveAgain() {
        path = null;
        SetMovementState(0);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    public MovementStates GetMovementStateEnum(int i) {
        if (i == 0) { return MovementStates.Unselected; }
        else if (i == 1) { return MovementStates.Selected; }
        else if (i == 2) { return MovementStates.Moved; }
        else if (i == 3) { return MovementStates.Wait; }
        return MovementStates.Unselected;
    }

    public void SetMovementState(int i) {
        if (i == 0) { unitMoveState =  MovementStates.Unselected; }
        else if (i == 1) { unitMoveState = MovementStates.Selected; }
        else if (i == 2) { unitMoveState = MovementStates.Moved; }
        else if (i == 3) { unitMoveState = MovementStates.Wait; }
    }

    public void UpdateHealthUI() {
        healthBar.fillAmount = (float) currentHealth / maxHealth;
        hitPointsText.SetText(currentHealth.ToString());
    }

    public abstract void CmdDealDamage(int damageStat, int defendingStat);
    
    public void Wait() {
        if (!hasAuthority) return;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
    }

    public void ChangeHealthBarColour(int i) {
        if (i == 0) { healthBar.color = Color.blue; }
        else if (i == 1) { healthBar.color = Color.red; }
        else if (i == 2) { healthBar.color = Color.yellow; }
    }

    public void UnitDie() {
        if (holder2D.activeSelf) {
            StartCoroutine(CombatEnd());
            StartCoroutine(UnitDestroy());     
        }
    }

    public IEnumerator UnitDestroy() {
        while (combatQueue.Count > 0) { yield return new WaitForEndOfFrame(); }
        NetworkServer.Destroy(gameObject);
    } 

    public abstract IEnumerator CombatEnd();

    public IEnumerator UnitMoveSequence(GameObject objectToMove, Node endNode) {
        movementQueue.Enqueue(1);
        path.RemoveAt(0);
        while (path.Count != 0) {
            Vector3 endPos = map.TileCoordToWorldCoord(path[0].x, path[0].y);
            objectToMove.transform.position = Vector3.Lerp(transform.position, endPos, visualMovementSpeed);
            if ((transform.position - endPos).sqrMagnitude < 0.001) { path.RemoveAt(0); }
            yield return new WaitForEndOfFrame();
        }
        visualMovementSpeed = 0.15f;
        transform.position = map.TileCoordToWorldCoord(endNode.x, endNode.y);
        x = endNode.x;
        y = endNode.y;
        CmdUpdateNewPosition(x, y);
        tileBeingOccupied.GetComponent<TileClick>().unitOnTile = null;
        RpcDeleteOldPosition();
        tileBeingOccupied = map.tilesOnMap[x, y];
        movementQueue.Dequeue();
    }
}