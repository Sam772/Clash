using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class Unit : NetworkBehaviour {
    [SyncVar]
    public int teamNum;
    [SyncVar]
    public int x;
    [SyncVar]
    public int y;
    public Queue<int> movementQueue;
    public Queue<int> combatQueue;
    public float visualMovementSpeed = .15f;
    public Material unitWaitMaterial;
    public Material unitMaterial;
    [SyncVar(hook=nameof(ChangeColor))]
    public Color unitTwoColour = Color.red;
    public Animator animator;
    public GameObject tileBeingOccupied;
    public string unitName;
    public int moveSpeed;
    public int attackRange;
    public int attackDamage;
    public int maxHealthPoints;
    [SyncVar]
    public int currentHealthPoints;
    public Sprite unitSprite;

    [Header("UI Elements")]
    public Canvas healthBarCanvas;
    public TMP_Text hitPointsText;
    public Image healthBar;
    public Canvas damagePopupCanvas;
    public TMP_Text damagePopupText;
    public Image damageBackdrop;
    public TileMap map;
    public GameObject holder2D;
    private BattleManager BMS;

    //--------------------------------------

    public void SetupUnit(GameData data, int playerId) {

    }

    //--------------------------------------
    public enum MovementStates {
        Unselected,
        Selected,
        Moved,
        Wait
    }
    public MovementStates unitMoveState;
    public List<Node> path = null;
    public bool completedMovement = false;
    private void Awake() {
        animator = holder2D.GetComponent<Animator>();
        movementQueue = new Queue<int>();
        combatQueue = new Queue<int>();
        x = (int)transform.position.x;
        y = (int)transform.position.z;
        unitMoveState = MovementStates.Unselected;
        currentHealthPoints = maxHealthPoints;
        hitPointsText.SetText(currentHealthPoints.ToString());
    }

    public void LateUpdate() {
        healthBarCanvas.transform.forward = Camera.main.transform.forward;
        holder2D.transform.forward = Camera.main.transform.forward;
    }

    public void ChangeColor(Color oldColour, Color newColour) {
        unitTwoColour = newColour;
    }

    public void MoveNextTile() {
        if (path.Count == 0) {
            return;
        }
        else {
            StartCoroutine(MoveOverSeconds(transform.gameObject, path[path.Count - 1]));
        }
    }

    [Command(requiresAuthority=false)]
    public void CmdUpdateTileMap(int newX, int newY) {
        x = newX;
        y = newY;
    }

    public void MoveAgain() {
        path = null;
        SetMovementState(0);
        completedMovement = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    public MovementStates GetMovementStateEnum(int i) {
        if (i == 0) {
            return MovementStates.Unselected;
        }
        else if (i == 1) {
            return MovementStates.Selected;
        }
        else if (i == 2) {
            return MovementStates.Moved;
        }
        else if (i == 3) {
            return MovementStates.Wait;
        }
        return MovementStates.Unselected;
    }


    public void SetMovementState(int i) {
        if (i == 0) {
            unitMoveState =  MovementStates.Unselected;
        }
        else if (i == 1) {
            unitMoveState = MovementStates.Selected;
        }
        else if (i == 2) {
            unitMoveState = MovementStates.Moved;
        }
        else if (i == 3) {
            unitMoveState = MovementStates.Wait;
        }
    }

    public void UpdateHealthUI() {
        healthBar.fillAmount = (float)currentHealthPoints / maxHealthPoints;
        hitPointsText.SetText(currentHealthPoints.ToString());
    }

    [Command(requiresAuthority=false)]
    public void DealDamage(int damage) {
        currentHealthPoints = currentHealthPoints - damage;
        if (!isServer) {
        //currentHealthPoints = currentHealthPoints - damage;
        //Debug.Log("health: " + currentHealthPoints);
        }
        UpdateDamageToClient(damage);
        //UpdateHealthUI();
    }

    [ClientRpc]
    public void UpdateDamageToClient(int damageToClient) {
        if (!isServer) {
        currentHealthPoints = currentHealthPoints - damageToClient;
        }
        Debug.Log("damage dealt: " + damageToClient);
        Debug.Log("hp of attacked unit: " + currentHealthPoints);
        UpdateHealthUI();
    }
    
    public void Wait() {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
    }

    public void ChangeHealthBarColour(int i) {
        if (i == 0) {
            healthBar.color = Color.blue;
        }
        else if (i == 1) {
            healthBar.color = Color.red;
        }
    }

    public void UnitDie() {
        Debug.Log("sadge");
        if (holder2D.activeSelf) {
            StartCoroutine(FadeOut());
            StartCoroutine(CheckIfRoutinesRunning());     
        }
    }

    public IEnumerator CheckIfRoutinesRunning() {
        while (combatQueue.Count>0) {
            yield return new WaitForEndOfFrame();
        }
        NetworkServer.Destroy(gameObject);
    } 

    public IEnumerator FadeOut() {
        combatQueue.Enqueue(1);
        //Renderer rend = GetComponentInChildren<SpriteRenderer>();
        for (float f = 1f; f >= .05; f -= 0.01f) {
            // Color c = rend.material.color;
            // c.a = f;
            // rend.material.color = c;
            yield return new WaitForEndOfFrame();
        }
        combatQueue.Dequeue();
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Node endNode) {
        movementQueue.Enqueue(1);
        path.RemoveAt(0);
        while (path.Count != 0) {
            Vector3 endPos = map.TileCoordToWorldCoord(path[0].x, path[0].y);
            objectToMove.transform.position = Vector3.Lerp(transform.position, endPos, visualMovementSpeed);
            if ((transform.position - endPos).sqrMagnitude < 0.001) {
                path.RemoveAt(0);
            }
            yield return new WaitForEndOfFrame();
        }
        visualMovementSpeed = 0.15f;
        transform.position = map.TileCoordToWorldCoord(endNode.x, endNode.y);

        x = endNode.x;
        y = endNode.y;

        if (!isServer) {
        CmdUpdateTileMap(x, y);
        }

        tileBeingOccupied.GetComponent<TileClick>().unitOnTile = null;
        tileBeingOccupied = map.tilesOnMap[x, y];
        movementQueue.Dequeue();
    }

    public IEnumerator DisplayDamageEnum(int damageTaken) {
        combatQueue.Enqueue(1);
        damagePopupText.SetText(damageTaken.ToString());
        damagePopupCanvas.enabled = true;
        for (float f = 1f; f >=-0.01f; f -= 0.01f) {
            Color backDrop = damageBackdrop.GetComponent<Image>().color;
            Color damageValue = damagePopupText.color;

            backDrop.a = f;
            damageValue.a = f;
            damageBackdrop.GetComponent<Image>().color = backDrop;
            damagePopupText.color = damageValue;
           yield return new WaitForEndOfFrame();
        }
        combatQueue.Dequeue();
    }

    public void ResetPath() {
        path = null;
        completedMovement = false;
    }

    public void DisplayDamage(int damageTaken) {
        damagePopupCanvas.enabled = true;
        damagePopupText.SetText(damageTaken.ToString());
    }

    public void DisableDisplayDamage() {
        damagePopupCanvas.enabled = false;
    }
}