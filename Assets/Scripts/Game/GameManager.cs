using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Mirror;

public class GameManager : NetworkBehaviour {

    [Header("Player")]
    [SerializeField] private PlayerInfo player1;
    [SerializeField] private PlayerInfo player2;
    public GameData Data {get; private set;}
    private NewNetworkManager room;
    private NewNetworkGamePlayer gamePlayer;
    public TMP_Text currentTeamUI;
    public Canvas displayWinnerUI;
    public TMP_Text UICurrentHealth;
    public TMP_Text UIStrength;
    public TMP_Text UIMagic;
    public TMP_Text UIDefence;
    public TMP_Text UIResistance;
    public TMP_Text UIRange;
    public TMP_Text UIMove;
    public TMP_Text UIUnitName;
    public UnityEngine.UI.Image UISprite;
    public UnityEngine.UI.Image strengthIcon;
    public UnityEngine.UI.Image magicIcon;
    public Canvas UIUnitCanvas;
    public GameObject playerPhaseBlock;
    private Animator playerPhaseAnim;
    private TMP_Text playerPhaseText;
    private RaycastHit hit;
    [SyncVar(hook = nameof(OnPlayChange))]
    public int currentTeam;
    private GameObject unitBeingDisplayed;
    public GameObject tileBeingDisplayed;
    private bool displayingUnitInfo;
    private GenericTileMap TMS;
    private int cursorX;
    private int cursorY;
    private int selectedXTile;
    private int selectedYTile;
    List<Node> currentPathForUnitRoute;
    List<Node> unitPathToCursor;
    private bool unitPathExists;
    public Material UIUnitRoute;
    public Material UIUnitRouteCurve;
    public Material UIUnitRouteArrow;
    public Material UICursor;
    private int routeToX;
    private int routeToY;
    public LeaderboardManager leaderboard;
    public Canvas ActionCanvas;
    public SkillManager skillManager;
    [SerializeField] private GameObject endTurnButton;
    public void Start() {
        currentTeam = 0;
        SetTeamHealthbarColour();
        displayingUnitInfo = false;
        playerPhaseAnim = playerPhaseBlock.GetComponent<Animator>();
        playerPhaseText = playerPhaseBlock.GetComponentInChildren<TextMeshProUGUI>();
        unitPathToCursor = new List<Node>();
        unitPathExists = false;       
        TMS = GetComponent<GenericTileMap>();
        room = FindObjectOfType<NewNetworkManager>();
        UpdatePlayerInfo(room.GamePlayers);
        currentTeamUI.SetText(player2.playerName.text + "'s Phase");
        playerPhaseText.SetText(player2.playerName.text + "'s Phase");
        endTurnButton.SetActive(false);
        CmdSetEndTurnButton();
    }

    public void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            CursorUIUpdate();
            UnitUIUpdate();
            if (TMS.selectedUnit != null && TMS.selectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(1) == TMS.selectedUnit.GetComponent<GenericUnit>().unitMoveState) {
                if (TMS.selectedUnitMoveRange.Contains(TMS.graph[cursorX, cursorY])) {
                    if (cursorX != TMS.selectedUnit.GetComponent<GenericUnit>().x || cursorY != TMS.selectedUnit.GetComponent<GenericUnit>().y) {
                        if (!unitPathExists && TMS.selectedUnit.GetComponent<GenericUnit>().movementQueue.Count == 0) {                           
                            unitPathToCursor = GenerateCursorRouteTo(cursorX, cursorY);
                            routeToX = cursorX;
                            routeToY = cursorY;
                            if (unitPathToCursor.Count != 0) {                                                                 
                                for (int i = 0; i < unitPathToCursor.Count; i++) {
                                    int nodeX = unitPathToCursor[i].x;
                                    int nodeY = unitPathToCursor[i].y;
                                    if (i == 0) {
                                        GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
                                        quadToUpdate.GetComponent<Renderer>().material = UICursor;
                                    } else if (i != 0 && (i + 1) != unitPathToCursor.Count) { SetCorrectRouteWithInputAndOutput(nodeX, nodeY,i); }
                                    else if (i == unitPathToCursor.Count-1) { SetCorrectRouteFinalTile(nodeX, nodeY, i); }
                                    TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY].GetComponent<Renderer>().enabled = true;}}
                            unitPathExists = true;
                        } else if (routeToX != cursorX || routeToY != cursorY) {
                            if (unitPathToCursor.Count != 0) {
                                for (int i = 0; i < unitPathToCursor.Count; i++) {
                                    int nodeX = unitPathToCursor[i].x;
                                    int nodeY = unitPathToCursor[i].y;
                                    TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY].GetComponent<Renderer>().enabled = false;}}
                            unitPathExists = false;}}
                    else if (cursorX == TMS.selectedUnit.GetComponent<GenericUnit>().x && cursorY == TMS.selectedUnit.GetComponent<GenericUnit>().y) { 
                        TMS.DisableUnitUIRoute();
                        unitPathExists = false;}}}}
    }

    public void UpdatePlayerInfo(List<NewNetworkGamePlayer> players) {
        for (var i = 0; i < players.Count; i++) {
            SetPlayerInfo(players[i], i);
        }   
    }

    private void SetPlayerInfo(NewNetworkGamePlayer player, int playerNumber) {
        if (playerNumber == 0) {
            player1.SetupPlayerInfo(player);
        } else {
            player2.SetupPlayerInfo(player);
        }
    }

    // sets the end turn button for player one
    [Command(requiresAuthority=false)]
    public void CmdSetEndTurnButton() {
        endTurnButton.SetActive(true);
        RpcSetOffEndTurnButton();
    }

    // sets the end turn button for player two
    [ClientRpc]
    public void RpcSetEndTurnButton() {
        if (!isServer) {
            endTurnButton.SetActive(true);
        }
        CmdSetOffEndTurnButton();
    }

    [Command(requiresAuthority=false)]
    public void CmdSetOffEndTurnButton() {
        endTurnButton.SetActive(false);
    }

    [ClientRpc]
    public void RpcSetOffEndTurnButton() {
        if (!isServer) {
            endTurnButton.SetActive(false);
        }
    }

    [ClientRpc]
    public void RpcResetUnitsActions(int teamToReset) {
        GenericUnit[] unitsList = FindObjectsOfType<GenericUnit>();
        foreach (GenericUnit unit in unitsList) {
            unit.GetComponent<GenericUnit>().MoveAgain();}
    }

    public void SetTeamHealthbarColour() {
        GenericUnit[] unitsList = FindObjectsOfType<GenericUnit>();
        foreach (GenericUnit unit in unitsList) {
            if (unit.GetComponent<GenericUnit>().team == 0) { unit.GetComponent<GenericUnit>().ChangeHealthBarColour(0);
            } else if (unit.GetComponent<GenericUnit>().team == 1) { unit.GetComponent<GenericUnit>().ChangeHealthBarColour(1); 
            } else if (unit.GetComponent<GenericUnit>().team == 2) { unit.GetComponent<GenericUnit>().ChangeHealthBarColour(2);}}
    }

    public void OnPlayChange(int oldV, int newV) {
        SetTeamHealthbarColour();
        if (TMS.selectedUnit == null)
            if (oldV == 0) {
                playerPhaseAnim.SetTrigger("slideLeftTrigger");
                playerPhaseText.SetText(player1.playerName.text + "'s Phase");
                currentTeamUI.SetText(player1.playerName.text + "'s Phase");
                RpcSetEndTurnButton();
            } else {
                playerPhaseAnim.SetTrigger("slideRightTrigger");
                playerPhaseText.SetText(player2.playerName.text + "'s Phase");
                currentTeamUI.SetText(player2.playerName.text + "'s Phase");
                CmdSetEndTurnButton();}
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn() {
        if (TMS.selectedUnit == null) {
            CmdSwitchCurrentPlayer();
            if (currentTeam == 1) {
                playerPhaseAnim.SetTrigger("slideLeftTrigger");
                playerPhaseText.SetText(player1.playerName.text + "'s Phase");
            } else if (currentTeam == 0) {
                playerPhaseAnim.SetTrigger("slideRightTrigger");
                playerPhaseText.SetText(player2.playerName.text + "'s Phase");}
            SetTeamHealthbarColour();
        }
    }

    [Command(requiresAuthority=false)]
    public void CmdSwitchCurrentPlayer() {
        skillManager.NoSkillRanger = false;

        RpcResetUnitsActions(currentTeam);
        currentTeam++;
        if (currentTeam == 2) {
            currentTeam = 0;}
    }

    [Command(requiresAuthority = false)]
    public void CmdAttack()
    {
        print("AttackCommand");
        TMS.actionChoice = 1;
    }
    //[Command(requiresAuthority = false)]
    public void CmdSkill()
    {
        print("SkillCommand");
        TMS.actionChoice = 2;
        skillManager.SkillActivation();
    }

    [Command(requiresAuthority = false)]
    public void CmdWait()
    {
        TMS.actionChoice = 3;
    }
    public void CursorUIUpdate() {
        if (hit.transform.CompareTag("Tile")) {
            if (tileBeingDisplayed == null) {
                selectedXTile = hit.transform.gameObject.GetComponent<TileClick>().tileX;
                selectedYTile = hit.transform.gameObject.GetComponent<TileClick>().tileY;
                cursorX = selectedXTile;
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                tileBeingDisplayed = hit.transform.gameObject;   
            } else if (tileBeingDisplayed != hit.transform.gameObject) {
                selectedXTile = tileBeingDisplayed.GetComponent<TileClick>().tileX;
                selectedYTile = tileBeingDisplayed.GetComponent<TileClick>().tileY;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;
                selectedXTile = hit.transform.gameObject.GetComponent<TileClick>().tileX;
                selectedYTile = hit.transform.gameObject.GetComponent<TileClick>().tileY;
                cursorX = selectedXTile;
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                tileBeingDisplayed = hit.transform.gameObject;}
        } else if (hit.transform.CompareTag("Unit")) {
            if (tileBeingDisplayed == null) {
                selectedXTile = hit.transform.parent.gameObject.GetComponent<GenericUnit>().x;
                selectedYTile = hit.transform.parent.gameObject.GetComponent<GenericUnit>().y;
                cursorX = selectedXTile;
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                tileBeingDisplayed = hit.transform.parent.gameObject.GetComponent<GenericUnit>().tileBeingOccupied;
            } else if (tileBeingDisplayed != hit.transform.gameObject) {
                if (hit.transform.parent.gameObject.GetComponent<GenericUnit>().movementQueue.Count == 0) {
                    selectedXTile = tileBeingDisplayed.GetComponent<TileClick>().tileX;
                    selectedYTile = tileBeingDisplayed.GetComponent<TileClick>().tileY;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;
                    selectedXTile = hit.transform.parent.gameObject.GetComponent<GenericUnit>().x;
                    selectedYTile = hit.transform.parent.gameObject.GetComponent<GenericUnit>().y;
                    cursorX = selectedXTile;
                    cursorY = selectedYTile;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                    tileBeingDisplayed = hit.transform.parent.GetComponent<GenericUnit>().tileBeingOccupied;}}
        } else { TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;}
    }

    public void UnitUIUpdate() {
        if (!displayingUnitInfo) {
            if (hit.transform.CompareTag("Unit")) {
                UIUnitCanvas.enabled = true;
                displayingUnitInfo = true;
                unitBeingDisplayed = hit.transform.parent.gameObject;
                var highlightedUnit = hit.transform.parent.gameObject.GetComponent<GenericUnit>();
                var highlightedPhysicalUnitText = unitBeingDisplayed.GetComponent<PhysicalUnit>();
                var highlightedMagicalUnitText = unitBeingDisplayed.GetComponent<MagicalUnit>();
                var highlightedLogTerrainText = unitBeingDisplayed.GetComponent<LogTerrain>();
                var highlightedBoulderTerrainText = unitBeingDisplayed.GetComponent<BoulderTerrain>();
                var highlightedHealingPotTerrainText = unitBeingDisplayed.GetComponent<HealingPotTerrain>();
                var highlightedStoneCrackedTerrainText = unitBeingDisplayed.GetComponent<StoneCrackedTerrain>();
                UICurrentHealth.SetText(highlightedUnit.currentHealth.ToString());

                if (highlightedPhysicalUnitText) {
                    strengthIcon.enabled = true;
                    UIStrength.SetText(highlightedPhysicalUnitText.strength.ToString());
                    UIMagic.text = default;
                    magicIcon.enabled = false;
                } else if (highlightedMagicalUnitText) {
                    magicIcon.enabled = true;
                    UIMagic.SetText(highlightedMagicalUnitText.magic.ToString());
                    UIStrength.text = default;
                    strengthIcon.enabled = false;
                } else if (highlightedLogTerrainText) {
                    strengthIcon.enabled = true;
                    UIStrength.SetText(0.ToString());
                    UIMagic.text = default;
                    magicIcon.enabled = false;
                } else if (highlightedBoulderTerrainText) {
                    strengthIcon.enabled = true;
                    UIStrength.SetText(0.ToString());
                    UIMagic.text = default;
                    magicIcon.enabled = false;
                } else if (highlightedHealingPotTerrainText) {
                    strengthIcon.enabled = true;
                    UIStrength.SetText(0.ToString());
                    UIMagic.text = default;
                    magicIcon.enabled = false;   
                } else if (highlightedStoneCrackedTerrainText) {
                    strengthIcon.enabled = true;
                    UIStrength.SetText(0.ToString());
                    UIMagic.text = default;
                    magicIcon.enabled = false;   
                }

                UIDefence.SetText(highlightedUnit.defence.ToString());
                UIResistance.SetText(highlightedUnit.resistance.ToString());
                UIRange.SetText(highlightedUnit.range.ToString());
                UIMove.SetText(highlightedUnit.move.ToString());
                UIUnitName.SetText(highlightedUnit.unitName);
                UISprite.sprite = highlightedUnit.unitSprite;
            } else if (hit.transform.CompareTag("Tile")) {
                if (hit.transform.GetComponent<TileClick>().unitOnTile != null) {
                    unitBeingDisplayed = hit.transform.GetComponent<TileClick>().unitOnTile;
                    UIUnitCanvas.enabled = true;
                    displayingUnitInfo = true;
                    var highlightedUnitText = unitBeingDisplayed.GetComponent<GenericUnit>();
                    var highlightedPhysicalUnitText = unitBeingDisplayed.GetComponent<PhysicalUnit>();
                    var highlightedMagicalUnitText = unitBeingDisplayed.GetComponent<MagicalUnit>();
                    var highlightedLogTerrainText = unitBeingDisplayed.GetComponent<LogTerrain>();
                    var highlightedBoulderTerrainText = unitBeingDisplayed.GetComponent<BoulderTerrain>();
                    var highlightedHealingPotTerrainText = unitBeingDisplayed.GetComponent<HealingPotTerrain>();
                    var highlightedStoneCrackedTerrainText = unitBeingDisplayed.GetComponent<StoneCrackedTerrain>();
                    UICurrentHealth.SetText(highlightedUnitText.currentHealth.ToString());

                    if (highlightedPhysicalUnitText) {
                        strengthIcon.enabled = true;
                        UIStrength.SetText(highlightedPhysicalUnitText.strength.ToString());
                        UIMagic.text = default;
                        magicIcon.enabled = false;
                    } else if (highlightedMagicalUnitText) {
                        magicIcon.enabled = true;
                        UIMagic.SetText(highlightedMagicalUnitText.magic.ToString());
                        UIStrength.text = default;
                        strengthIcon.enabled = false;
                    } else if (highlightedLogTerrainText) {
                        strengthIcon.enabled = true;
                        UIStrength.SetText(0.ToString());
                        UIMagic.text = default;
                        magicIcon.enabled = false;
                    } else if (highlightedBoulderTerrainText) {
                        strengthIcon.enabled = true;
                        UIStrength.SetText(0.ToString());
                        UIMagic.text = default;
                        magicIcon.enabled = false;
                    } else if (highlightedHealingPotTerrainText) {
                        strengthIcon.enabled = true;
                        UIStrength.SetText(0.ToString());
                        UIMagic.text = default;
                        magicIcon.enabled = false;   
                    } else if (highlightedStoneCrackedTerrainText) {
                        strengthIcon.enabled = true;
                        UIStrength.SetText(0.ToString());
                        UIMagic.text = default;
                        magicIcon.enabled = false;   
                    }

                    UIDefence.SetText(highlightedUnitText.defence.ToString());
                    UIResistance.SetText(highlightedUnitText.resistance.ToString());
                    UIRange.SetText(highlightedUnitText.range.ToString());
                    UIMove.SetText(highlightedUnitText.move.ToString());
                    UIUnitName.SetText(highlightedUnitText.unitName);
                    UISprite.sprite = highlightedUnitText.unitSprite;}}
        } else if (hit.transform.gameObject.CompareTag("Tile")) {
            if (hit.transform.GetComponent<TileClick>().unitOnTile == null) {
                UIUnitCanvas.enabled = false;
                displayingUnitInfo = false;
            } else if (hit.transform.GetComponent<TileClick>().unitOnTile != unitBeingDisplayed) {
                UIUnitCanvas.enabled = false;
                displayingUnitInfo = false;}
        } else if (hit.transform.gameObject.CompareTag("Unit")) {
            if (hit.transform.parent.gameObject != unitBeingDisplayed) {
                UIUnitCanvas.enabled = false;
                displayingUnitInfo = false;}}
    }

    public List<Node> GenerateCursorRouteTo(int x, int y) {
        if (TMS.selectedUnit.GetComponent<GenericUnit>().x == x && TMS.selectedUnit.GetComponent<GenericUnit>().y == y) {
            currentPathForUnitRoute = new List<Node>();  
            return currentPathForUnitRoute;}
        if (TMS.UnitCanEnterTile(x, y) == false) {return null;}
        currentPathForUnitRoute = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        Node source = TMS.graph[TMS.selectedUnit.GetComponent<GenericUnit>().x, TMS.selectedUnit.GetComponent<GenericUnit>().y];
        Node target = TMS.graph[x, y];
        dist[source] = 0;
        prev[source] = null;
        List<Node> unvisited = new List<Node>();
        foreach (Node n in TMS.graph) {
            if (n != source) {
                dist[n] = Mathf.Infinity;
                prev[n] = null;}
            unvisited.Add(n);}
        while (unvisited.Count > 0) {
            Node u = null;
            foreach (Node possibleU in unvisited) { if (u == null || dist[possibleU] < dist[u]) { u = possibleU; }}
            if (u == target) { break; }
            unvisited.Remove(u);
            foreach (Node n in u.neighbours) {
                float alt = dist[u] + TMS.CostToEnterTile(n.x, n.y);
                if (alt < dist[n]) {
                    dist[n] = alt;
                    prev[n] = u;}}}
        if (prev[target] == null) { return null; }
        currentPathForUnitRoute = new List<Node>();
        Node curr = target;
        while (curr != null) {
            currentPathForUnitRoute.Add(curr);
            curr = prev[curr];}
        currentPathForUnitRoute.Reverse();
        return currentPathForUnitRoute;
    }

    public Vector2 DirectionBetween(Vector2 currentVector, Vector2 nextVector) {
        Vector2 vectorDirection = (nextVector - currentVector).normalized;
        if (vectorDirection == Vector2.right) { return Vector2.right; }
        else if (vectorDirection == Vector2.left) { return Vector2.left; }
        else if (vectorDirection == Vector2.up) { return Vector2.up; }
        else if (vectorDirection == Vector2.down) { return Vector2.down; }
        else {
            Vector2 vectorToReturn = new Vector2();
            return vectorToReturn;}
    }

    public void SetCorrectRouteWithInputAndOutput(int nodeX, int nodeY, int i) {
        Vector2 previousTile = new Vector2(unitPathToCursor[i - 1].x + 1, unitPathToCursor[i - 1].y + 1);
        Vector2 currentTile = new Vector2(unitPathToCursor[i].x + 1, unitPathToCursor[i].y + 1);
        Vector2 nextTile = new Vector2(unitPathToCursor[i + 1].x + 1, unitPathToCursor[i + 1].y + 1);
        Vector2 backToCurrentVector = DirectionBetween(previousTile, currentTile);
        Vector2 currentToFrontVector = DirectionBetween(currentTile, nextTile);
        if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.right) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.up) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.down) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.left) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.up) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.down) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.up) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.right) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.left) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.down) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.right) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.left) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;}
    }

    public void SetCorrectRouteFinalTile(int nodeX, int nodeY, int i) {
        Vector2 previousTile = new Vector2(unitPathToCursor[i - 1].x + 1, unitPathToCursor[i - 1].y + 1);
        Vector2 currentTile = new Vector2(unitPathToCursor[i].x + 1, unitPathToCursor[i].y + 1);
        Vector2 backToCurrentVector = DirectionBetween(previousTile, currentTile);
        if (backToCurrentVector == Vector2.right) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.left) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.up) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        } else if (backToCurrentVector == Vector2.down) {
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIUnitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;}
    }

    [ClientRpc]
    public void RpcSetWinScreenOne() {
        displayWinnerUI.enabled = true;
        displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText(player2.playerName.text + " has won!");
    }

    [ClientRpc]
    public void RpcSetWinScreenTwo() {
        displayWinnerUI.enabled = true;
        displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText(player1.playerName.text + " has won!");
    }

    [Command(requiresAuthority=false)]
    public void CmdUnitsRemainClient(GameObject attacker, GameObject receiver) {
        StartCoroutine(CombatQueueCoroutine(attacker, receiver));
        StartCoroutine(WaitForDeathsCoroutine());
    }

    [ClientRpc]
    public void RpcPlayerWinConditionOne() {
        if (!isServer) {
            Debug.Log(player1.playerName.text + " has lost!");
            leaderboard.SendLossesLeaderboard(1);
        }
    }

    [ClientRpc]
    public void RpcPlayerWinConditionTwo() {
        if (!isServer) {
            Debug.Log(player1.playerName.text + " has won!");
            leaderboard.SendLeaderboard(1);
        }
    }

    public IEnumerator CombatQueueCoroutine(GameObject attacker, GameObject receiver) {
        while (attacker.GetComponent<GenericUnit>().combatQueue.Count != 0 && receiver.GetComponent<GenericUnit>().combatQueue.Count != 0) {
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator WaitForDeathsCoroutine() {
        
        int team1 = 0;
        int team2 = 0;

        yield return new WaitForSeconds(2f);

        GenericUnit[] unitsList = FindObjectsOfType<GenericUnit>();

        foreach (GenericUnit units in unitsList) {

            if (units.GetComponent<GenericUnit>().team == 0) {
                team1++;
            } else if (units.GetComponent<GenericUnit>().team == 1) {
                team2++;
            }
        }

        if (team1 == 0) {
            RpcSetWinScreenTwo();
            Debug.Log(player2.playerName.text + " has lost!");
            leaderboard.SendLossesLeaderboard(1);
            RpcPlayerWinConditionTwo();
        }
        
        if (team2 == 0) {
            RpcSetWinScreenOne();
            Debug.Log(player2.playerName.text + " has won!");
            leaderboard.SendLeaderboard(1);
            RpcPlayerWinConditionOne();
        }
    }

    public struct Dependencies {
        public NewNetworkManager NetworkManager;
        public GameData Data;
        public bool IsValid() {
            return Data != null && NetworkManager != null;}
    }

    public void SetDependencies(Dependencies dependencies) {
        if (!dependencies.IsValid()) {
            Debug.LogError("Tried to setup with invalid dependencies");
            return;}
        room = dependencies.NetworkManager;
        Data = dependencies.Data;
    }

    public void SetGamePlayer (NewNetworkGamePlayer player) {
        gamePlayer = player;
    }

    public void LoadTitleScreen() {
        //NetworkManager.singleton.ServerChangeScene(room.menuScene);
        //SceneManager.LoadScene(0, LoadSceneMode.Single);
        foreach (var gameplayer in room.GamePlayers) {
            gameplayer.OnEndClicked();
        }
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}