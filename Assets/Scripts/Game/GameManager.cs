using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class GameManager : NetworkBehaviour {
    public GameData Data {get; private set;}
    private NewNetworkManager room;
    private NewNetworkGamePlayer gamePlayer;
    public TMP_Text currentTeamUI;
    public Canvas displayWinnerUI;
    public TMP_Text UICurrentHealth;
    public TMP_Text UIStrength;
    public TMP_Text UIDefence;
    public TMP_Text UIRange;
    public TMP_Text UIMove;
    public TMP_Text UIUnitName;
    public UnityEngine.UI.Image UISprite;
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
    public void Start() {
        currentTeam = 0;
        SetCurrentTeamUI();
        SetTeamHealthbarColour();
        displayingUnitInfo = false;
        playerPhaseAnim = playerPhaseBlock.GetComponent<Animator>();
        playerPhaseText = playerPhaseBlock.GetComponentInChildren<TextMeshProUGUI>();
        unitPathToCursor = new List<Node>();
        unitPathExists = false;       
        TMS = GetComponent<GenericTileMap>();
    }

    public void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            CursorUIUpdate();
            UnitUIUpdate();
            if (TMS.selectedUnit != null && TMS.selectedUnit.GetComponent<Unit>().GetMovementStateEnum(1) == TMS.selectedUnit.GetComponent<Unit>().unitMoveState) {
                if (TMS.selectedUnitMoveRange.Contains(TMS.graph[cursorX, cursorY])) {
                    if (cursorX != TMS.selectedUnit.GetComponent<Unit>().x || cursorY != TMS.selectedUnit.GetComponent<Unit>().y) {
                        if (!unitPathExists && TMS.selectedUnit.GetComponent<Unit>().movementQueue.Count == 0) {                           
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
                    else if (cursorX == TMS.selectedUnit.GetComponent<Unit>().x && cursorY == TMS.selectedUnit.GetComponent<Unit>().y) { 
                        TMS.DisableUnitUIRoute();
                        unitPathExists = false;}}}}
    }

    public void SetCurrentTeamUI() {
        currentTeamUI.SetText("Player " + (currentTeam + 1).ToString() + " Phase");
    }

    [ClientRpc]
    public void RpcResetUnitsActions(int teamToReset) {
        Unit[] unitsList = FindObjectsOfType<Unit>();
        foreach (Unit unit in unitsList) {
            unit.GetComponent<Unit>().MoveAgain();}
    }

    public void SetTeamHealthbarColour() {
        Unit[] unitsList = FindObjectsOfType<Unit>();
        foreach (Unit unit in unitsList) {
            if (unit.GetComponent<Unit>().team == 0) { unit.GetComponent<Unit>().ChangeHealthBarColour(0);
            } else if (unit.GetComponent<Unit>().team == 1) { unit.GetComponent<Unit>().ChangeHealthBarColour(1); }}
    }

    public void OnPlayChange(int oldV, int newV) {
        SetTeamHealthbarColour();
        if (TMS.selectedUnit == null)
            if (oldV == 0) {
                playerPhaseAnim.SetTrigger("slideLeftTrigger");
                playerPhaseText.SetText("Player 2 Phase");
            } else {
                playerPhaseAnim.SetTrigger("slideRightTrigger");
                playerPhaseText.SetText("Player 1 Phase");}
    }

    [Command(requiresAuthority = false)]
    public void CmdEndTurn() {
        if (TMS.selectedUnit == null) {
            CmdSwitchCurrentPlayer();
            if (currentTeam == 1) {
                playerPhaseAnim.SetTrigger("slideLeftTrigger");
                playerPhaseText.SetText("Player 2 Phase");
            } else if (currentTeam == 0) {
                playerPhaseAnim.SetTrigger("slideRightTrigger");
                playerPhaseText.SetText("Player 1 Phase");}
            SetTeamHealthbarColour();
            SetCurrentTeamUI();}
    }

    [Command(requiresAuthority=false)]
    public void CmdSwitchCurrentPlayer() {
        RpcResetUnitsActions(currentTeam);
        currentTeam++;
        if (currentTeam == 2) {
            currentTeam = 0;}
    }

    [ClientRpc]
    public void RpcCheckIfUnitsRemain(GameObject unit, GameObject enemy) {
        StartCoroutine(CheckIfUnitsRemainCoroutine(unit, enemy));
    }

    [Command(requiresAuthority=false)]
    public void CmdUnitsRemainClient(GameObject unit, GameObject enemy) {
        RpcCheckIfUnitsRemain(unit, enemy);
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
                selectedXTile = hit.transform.parent.gameObject.GetComponent<Unit>().x;
                selectedYTile = hit.transform.parent.gameObject.GetComponent<Unit>().y;
                cursorX = selectedXTile;
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                tileBeingDisplayed = hit.transform.parent.gameObject.GetComponent<Unit>().tileBeingOccupied;
            } else if (tileBeingDisplayed != hit.transform.gameObject) {
                if (hit.transform.parent.gameObject.GetComponent<Unit>().movementQueue.Count == 0) {
                    selectedXTile = tileBeingDisplayed.GetComponent<TileClick>().tileX;
                    selectedYTile = tileBeingDisplayed.GetComponent<TileClick>().tileY;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;
                    selectedXTile = hit.transform.parent.gameObject.GetComponent<Unit>().x;
                    selectedYTile = hit.transform.parent.gameObject.GetComponent<Unit>().y;
                    cursorX = selectedXTile;
                    cursorY = selectedYTile;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                    tileBeingDisplayed = hit.transform.parent.GetComponent<Unit>().tileBeingOccupied;}}
        } else {
            TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;}
    }

    public void UnitUIUpdate() {
        if (!displayingUnitInfo) {
            if (hit.transform.CompareTag("Unit")) {
                UIUnitCanvas.enabled = true;
                displayingUnitInfo = true;
                unitBeingDisplayed = hit.transform.parent.gameObject;
                var highlightedUnit = hit.transform.parent.gameObject.GetComponent<Unit>();
                UICurrentHealth.SetText(highlightedUnit.currentHealth.ToString());
                UIStrength.SetText(highlightedUnit.strength.ToString());
                UIDefence.SetText(highlightedUnit.defence.ToString());
                UIRange.SetText(highlightedUnit.range.ToString());
                UIMove.SetText(highlightedUnit.move.ToString());
                UIUnitName.SetText(highlightedUnit.unitName);
                UISprite.sprite = highlightedUnit.unitSprite;
            } else if (hit.transform.CompareTag("Tile")) {
                if (hit.transform.GetComponent<TileClick>().unitOnTile != null) {
                    unitBeingDisplayed = hit.transform.GetComponent<TileClick>().unitOnTile;
                    UIUnitCanvas.enabled = true;
                    displayingUnitInfo = true;
                    var highlightedUnitScript = unitBeingDisplayed.GetComponent<Unit>();
                    UICurrentHealth.SetText(highlightedUnitScript.currentHealth.ToString());
                    UIStrength.SetText(highlightedUnitScript.strength.ToString());
                    UIDefence.SetText(highlightedUnitScript.defence.ToString());
                    UIRange.SetText(highlightedUnitScript.range.ToString());
                    UIMove.SetText(highlightedUnitScript.move.ToString());
                    UIUnitName.SetText(highlightedUnitScript.unitName);
                    UISprite.sprite = highlightedUnitScript.unitSprite;}}
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
        if (TMS.selectedUnit.GetComponent<Unit>().x == x && TMS.selectedUnit.GetComponent<Unit>().y == y) {
            currentPathForUnitRoute = new List<Node>();  
            return currentPathForUnitRoute;}
        if (TMS.UnitCanEnterTile(x, y) == false) {return null;}
        currentPathForUnitRoute = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        Node source = TMS.graph[TMS.selectedUnit.GetComponent<Unit>().x, TMS.selectedUnit.GetComponent<Unit>().y];
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

    public IEnumerator CheckIfUnitsRemainCoroutine(GameObject unit, GameObject enemy) {
        int team1 = 0;
        int team2 = 0;
        while (unit.GetComponent<Unit>().combatQueue.Count != 0) { yield return new WaitForEndOfFrame(); }
        while (enemy.GetComponent<Unit>().combatQueue.Count != 0) { yield return new WaitForEndOfFrame(); }
        Unit[] unitsList = FindObjectsOfType<Unit>();
        foreach (Unit units in unitsList) {
            if (units.GetComponent<Unit>().team == 0) { team1++; }
            else if (units.GetComponent<Unit>().team == 1) { team2++; }}
        if (team1 == 1) {
            displayWinnerUI.enabled = true;
            displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Player 2 has won!");}
        if (team2 == 1) {
            displayWinnerUI.enabled = true;
            displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Player 1 has won!");}
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
}