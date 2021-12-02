using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Mirror;

public class GameManager : NetworkBehaviour {

    public GameData Data { get; private set; }
    public int PlayerID => gamePlayer.ID;
    private NewNetworkManager room;
    private NewNetworkGamePlayer gamePlayer;
    //------------------------------------------------------

    [Header("UI GameObjects")]
    public TMP_Text currentTeamUI;
    public Canvas displayWinnerUI;
    public TMP_Text UIunitCurrentHealth;
    public TMP_Text UIunitAttackDamage;
    public TMP_Text UIunitAttackRange;
    public TMP_Text UIunitMoveSpeed;
    public TMP_Text UIunitName;
    public UnityEngine.UI.Image UIunitSprite;
    public Canvas UIunitCanvas;
    public GameObject playerPhaseBlock;
    private Animator playerPhaseAnim;
    private TMP_Text playerPhaseText;
    private Ray ray;
    private RaycastHit hit;
    public int numberOfTeams = 2;

    [SyncVar(hook = nameof(OnPlayChange))]
    public int currentTeam;
    [SerializeField] private GameObject team1;
    [SerializeField] private GameObject team2;
    public GameObject unitRefresh;
    public GameObject unitBeingDisplayed;
    public GameObject tileBeingDisplayed;
    public bool displayingUnitInfo;
    public TileMap TMS;
    public int cursorX;
    public int cursorY;
    public int selectedXTile;
    public int selectedYTile;
    List<Node> currentPathForUnitRoute;
    List<Node> unitPathToCursor;
    public bool unitPathExists;
    public Material UIunitRoute;
    public Material UIunitRouteCurve;
    public Material UIunitRouteArrow;
    public Material UICursor;
    public int routeToX;
    public int routeToY;
    public void Start() {
        currentTeam = 0;
        SetCurrentTeamUI();
        TeamHealthbarColorUpdate();
        playerPhaseAnim = playerPhaseBlock.GetComponent<Animator>();
        playerPhaseText = playerPhaseBlock.GetComponentInChildren<TextMeshProUGUI>();
        unitPathToCursor = new List<Node>();
        unitPathExists = false;       
        TMS = GetComponent<TileMap>();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.X)){
            Debug.Log(unitRefresh);
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            CursorUIUpdate();
            UnitUIUpdate();
            if (TMS.selectedUnit != null && TMS.selectedUnit.GetComponent<Unit>().GetMovementStateEnum(1) == TMS.selectedUnit.GetComponent<Unit>().unitMoveState) {
                if (TMS.selectedUnitMoveRange.Contains(TMS.graph[cursorX, cursorY])) {
                    if (cursorX != TMS.selectedUnit.GetComponent<Unit>().x || cursorY != TMS.selectedUnit.GetComponent<Unit>().y) {
                        if (!unitPathExists&&TMS.selectedUnit.GetComponent<Unit>().movementQueue.Count==0) {                           
                            unitPathToCursor = generateCursorRouteTo(cursorX, cursorY);
                            routeToX = cursorX;
                            routeToY = cursorY;
                            if (unitPathToCursor.Count != 0) {                                                                 
                                for(int i = 0; i < unitPathToCursor.Count; i++) {
                                    int nodeX = unitPathToCursor[i].x;
                                    int nodeY = unitPathToCursor[i].y;
                                    if (i == 0) {
                                        GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
                                        quadToUpdate.GetComponent<Renderer>().material = UICursor;
                                    }
                                    else if (i!=0 && (i+1)!=unitPathToCursor.Count) {
                                        //This is used to set the indicator for tiles excluding the first/last tile
                                        setCorrectRouteWithInputAndOutput(nodeX, nodeY,i);
                                    }
                                    else if (i == unitPathToCursor.Count-1) {
                                        //This is used to set the indicator for the final tile;
                                        setCorrectRouteFinalTile(nodeX, nodeY, i);
                                    }
                                    TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY].GetComponent<Renderer>().enabled = true;
                                } 
                            }
                            unitPathExists = true;
                        } else if (routeToX != cursorX || routeToY != cursorY) {
                            if (unitPathToCursor.Count != 0) {
                                for (int i = 0; i < unitPathToCursor.Count; i++) {
                                    int nodeX = unitPathToCursor[i].x;
                                    int nodeY = unitPathToCursor[i].y;
                                    TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY].GetComponent<Renderer>().enabled = false;
                                }
                            }
                            unitPathExists = false;
                        }
                    }
                    else if(cursorX == TMS.selectedUnit.GetComponent<Unit>().x && cursorY == TMS.selectedUnit.GetComponent<Unit>().y) { 
                        TMS.DisableUnitUIRoute();
                        unitPathExists = false;
                    }
                }               
            }
        }
    }

    public void SetCurrentTeamUI() {
        currentTeamUI.SetText("Player " + (currentTeam+1).ToString() + " Phase");
    }

    // has two gameobjects
    // iterates through team gameobjects

    // need to use gameobject of unit to check unit team
    // set unit team to return team
    // mapunits gameobject is irrelevant

    // uses i to fit units into their teams
    public GameObject returnTeam(int i) {
        GameObject teamToReturn = null;
        if (i == 0) {
            teamToReturn = team1;
        }
        else if (i == 1) {
            teamToReturn = team2;
        }
        return teamToReturn;
    }
    
    public void resetUnitsMovements(GameObject teamToReset) {
        // this is checking the child gameobjects of the team object
        foreach (Transform unit in teamToReset.transform) {
            //if (unit.GetComponent<Unit>().teamNum == 0)
            unit.GetComponent<Unit>().MoveAgain();
            // unitRefresh = GameObject.FindGameObjectWithTag("Unit");
            // for (int i=0; i < unitRefresh.length; i++)
            // Debug.Log(unitRefresh(i));
        }
    }

    public void TeamHealthbarColorUpdate() {
        for(int i = 0; i < numberOfTeams; i++) {
            GameObject team = returnTeam(i);
            if(team == returnTeam(currentTeam)) {
                // iterating children
                foreach (Transform unit in team.transform) {
                    unit.GetComponent<Unit>().ChangeHealthBarColour(0);
                }
            }
            else {
                // iterating children
                foreach (Transform unit in team.transform) {
                    unit.GetComponent<Unit>().ChangeHealthBarColour(1);
                }
            }
        }
    }

    public void OnPlayChange(int oldV, int newV) {
        TeamHealthbarColorUpdate();
        if (TMS.selectedUnit == null)
            if (oldV == 0) {
                playerPhaseAnim.SetTrigger("slideLeftTrigger");
                playerPhaseText.SetText("Player 2 Phase");
                Debug.Log("Banner slides left");
            }
            else {
                playerPhaseAnim.SetTrigger("slideRightTrigger");
                playerPhaseText.SetText("Player 1 Phase");
                Debug.Log("Banner slides right");
            }
    }

    [Command(requiresAuthority = false)]
    public void endTurn() {
        if (TMS.selectedUnit == null) {
            SwitchCurrentPlayer();
            if (currentTeam == 1) {
                playerPhaseAnim.SetTrigger("slideLeftTrigger");
                playerPhaseText.SetText("Player 2 Phase");
            }
            else if (currentTeam == 0) {
                playerPhaseAnim.SetTrigger("slideRightTrigger");
                playerPhaseText.SetText("Player 1 Phase");
            }
            TeamHealthbarColorUpdate();
            SetCurrentTeamUI();
        }
    }

    [Command(requiresAuthority=false)]
    public void SwitchCurrentPlayer() {
        resetUnitsMovements(returnTeam(currentTeam));
        currentTeam++;
        if (currentTeam == numberOfTeams) {
            currentTeam = 0;
        } 
    }

    public void checkIfUnitsRemain(GameObject unit, GameObject enemy) {
        //  Debug.Log(team1.transform.childCount);
        //  Debug.Log(team2.transform.childCount);
        StartCoroutine(checkIfUnitsRemainCoroutine(unit,enemy));
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
            }
            else if (tileBeingDisplayed != hit.transform.gameObject) {
                selectedXTile = tileBeingDisplayed.GetComponent<TileClick>().tileX;
                selectedYTile = tileBeingDisplayed.GetComponent<TileClick>().tileY;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;

                selectedXTile = hit.transform.gameObject.GetComponent<TileClick>().tileX;
                selectedYTile = hit.transform.gameObject.GetComponent<TileClick>().tileY;
                cursorX = selectedXTile;
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                tileBeingDisplayed = hit.transform.gameObject;  
            }
        }
        else if (hit.transform.CompareTag("Unit")) {
            if (tileBeingDisplayed == null) {
                selectedXTile = hit.transform.parent.gameObject.GetComponent<Unit>().x;
                selectedYTile = hit.transform.parent.gameObject.GetComponent<Unit>().y;
                cursorX = selectedXTile;
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                tileBeingDisplayed = hit.transform.parent.gameObject.GetComponent<Unit>().tileBeingOccupied;
            }
            else if (tileBeingDisplayed != hit.transform.gameObject) {
                if (hit.transform.parent.gameObject.GetComponent<Unit>().movementQueue.Count == 0) {
                    selectedXTile = tileBeingDisplayed.GetComponent<TileClick>().tileX;
                    selectedYTile = tileBeingDisplayed.GetComponent<TileClick>().tileY;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;

                    selectedXTile = hit.transform.parent.gameObject.GetComponent<Unit>().x;
                    selectedYTile = hit.transform.parent.gameObject.GetComponent<Unit>().y;
                    cursorX = selectedXTile;
                    cursorY = selectedYTile;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                    tileBeingDisplayed = hit.transform.parent.GetComponent<Unit>().tileBeingOccupied; 
                }
            }
        }
        else {
            TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void UnitUIUpdate() {
        if (!displayingUnitInfo) {
            if (hit.transform.CompareTag("Unit")) {
                UIunitCanvas.enabled = true;
                displayingUnitInfo = true;
                unitBeingDisplayed = hit.transform.parent.gameObject;
                var highlightedUnitScript = hit.transform.parent.gameObject.GetComponent<Unit>();

                UIunitCurrentHealth.SetText(highlightedUnitScript.currentHealthPoints.ToString());
                UIunitAttackDamage.SetText(highlightedUnitScript.attackDamage.ToString());
                UIunitAttackRange.SetText(highlightedUnitScript.attackRange.ToString());
                UIunitMoveSpeed.SetText(highlightedUnitScript.moveSpeed.ToString());
                UIunitName.SetText(highlightedUnitScript.unitName);
                UIunitSprite.sprite = highlightedUnitScript.unitSprite;
            }
            else if (hit.transform.CompareTag("Tile")) {
                if (hit.transform.GetComponent<TileClick>().unitOnTile != null) {
                    unitBeingDisplayed = hit.transform.GetComponent<TileClick>().unitOnTile;
                    UIunitCanvas.enabled = true;
                    displayingUnitInfo = true;
                    var highlightedUnitScript = unitBeingDisplayed.GetComponent<Unit>();

                    UIunitCurrentHealth.SetText(highlightedUnitScript.currentHealthPoints.ToString());
                    UIunitAttackDamage.SetText(highlightedUnitScript.attackDamage.ToString());
                    UIunitAttackRange.SetText(highlightedUnitScript.attackRange.ToString());
                    UIunitMoveSpeed.SetText(highlightedUnitScript.moveSpeed.ToString());
                    UIunitName.SetText(highlightedUnitScript.unitName);
                    UIunitSprite.sprite = highlightedUnitScript.unitSprite;
                }
            }
        }
        else if (hit.transform.gameObject.CompareTag("Tile")) {
            if (hit.transform.GetComponent<TileClick>().unitOnTile == null) {
                UIunitCanvas.enabled = false;
                displayingUnitInfo = false;
            }
            else if (hit.transform.GetComponent<TileClick>().unitOnTile != unitBeingDisplayed) {
                UIunitCanvas.enabled = false;
                displayingUnitInfo = false;
            }
        }
        else if (hit.transform.gameObject.CompareTag("Unit")) {
            if (hit.transform.parent.gameObject != unitBeingDisplayed) {
                UIunitCanvas.enabled = false;
                displayingUnitInfo = false;
            }
        }
    }

    public List<Node> generateCursorRouteTo(int x, int y) {
        if (TMS.selectedUnit.GetComponent<Unit>().x == x && TMS.selectedUnit.GetComponent<Unit>().y == y) {
            Debug.Log("clicked the same tile that the unit is standing on");
            currentPathForUnitRoute = new List<Node>();  
            return currentPathForUnitRoute;
        }
        if (TMS.unitCanEnterTile(x, y) == false) {
            //cant move into something so we can probably just return
            //cant set this endpoint as our goal
            return null;
        }

        //TMS.selectedUnit.GetComponent<UnitScript>().path = null;
        currentPathForUnitRoute = null;
        //from wiki dijkstra's
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        Node source = TMS.graph[TMS.selectedUnit.GetComponent<Unit>().x, TMS.selectedUnit.GetComponent<Unit>().y];
        Node target = TMS.graph[x, y];
        dist[source] = 0;
        prev[source] = null;
        //Unchecked nodes
        List<Node> unvisited = new List<Node>();

        //Initialize
        foreach (Node n in TMS.graph) {
            //Initialize to infite distance as we don't know the answer
            //Also some places are infinity
            if (n != source) {
                dist[n] = Mathf.Infinity;
                prev[n] = null;
            }
            unvisited.Add(n);
        }
        //if there is a node in the unvisited list lets check it
        while (unvisited.Count > 0) {
            //u will be the unvisited node with the shortest distance
            Node u = null;
            foreach (Node possibleU in unvisited) {
                if (u == null || dist[possibleU] < dist[u]) {
                    u = possibleU;
                }
            }

            if (u == target) {
                break;
            }

            unvisited.Remove(u);

            foreach (Node n in u.neighbours) {
                //float alt = dist[u] + u.DistanceTo(n);
                float alt = dist[u] + TMS.costToEnterTile(n.x, n.y);
                if (alt < dist[n]) {
                    dist[n] = alt;
                    prev[n] = u;
                }
            }
        }
        //if were here we found shortest path, or no path exists
        if (prev[target] == null) {
            //No route;
            return null;
        }
        currentPathForUnitRoute = new List<Node>();
        Node curr = target;

        //Step through the current path and add it to the chain
        while (curr != null) {
            currentPathForUnitRoute.Add(curr);
            curr = prev[curr];
        }
        //Now currPath is from target to our source, we need to reverse it from source to target.
        currentPathForUnitRoute.Reverse();
        return currentPathForUnitRoute;
    }

    public void resetQuad(GameObject quadToReset) {
        quadToReset.GetComponent<Renderer>().material = UICursor;
        quadToReset.transform.eulerAngles = new Vector3(90, 0, 0); 
    }

    public void UIunitRouteArrowDisplay(Vector2 cursorPos,Vector3 arrowRotationVector) {
        GameObject quadToManipulate = TMS.quadOnMapForUnitMovementDisplay[(int)cursorPos.x, (int)cursorPos.y];
        quadToManipulate.transform.eulerAngles = arrowRotationVector;
        quadToManipulate.GetComponent<Renderer>().material = UIunitRouteArrow;
        quadToManipulate.GetComponent<Renderer>().enabled = true;
    }

    public Vector2 directionBetween(Vector2 currentVector, Vector2 nextVector) {
        Vector2 vectorDirection = (nextVector - currentVector).normalized;
        if (vectorDirection == Vector2.right) {
            return Vector2.right;
        }
        else if (vectorDirection == Vector2.left) {
            return Vector2.left;
        }
        else if (vectorDirection == Vector2.up) {
            return Vector2.up;
        }
        else if (vectorDirection == Vector2.down) {
            return Vector2.down;
        }
        else {
            Vector2 vectorToReturn = new Vector2();
            return vectorToReturn;
        }
    }

    public void setCorrectRouteWithInputAndOutput(int nodeX,int nodeY,int i) {
        Vector2 previousTile = new Vector2(unitPathToCursor[i - 1].x + 1, unitPathToCursor[i - 1].y + 1);
        Vector2 currentTile = new Vector2(unitPathToCursor[i].x + 1, unitPathToCursor[i].y + 1);
        Vector2 nextTile = new Vector2(unitPathToCursor[i + 1].x + 1, unitPathToCursor[i + 1].y + 1);
        Vector2 backToCurrentVector = directionBetween(previousTile, currentTile);
        Vector2 currentToFrontVector = directionBetween(currentTile, nextTile);

        //Right (UP/DOWN/RIGHT)
        if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.right) {
            //Debug.Log("[IN[R]]->[Out[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.up) {
            //Debug.Log("[IN[R]]->[Out[UP]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.down) {
            //Debug.Log("[IN[R]]->[Out[DOWN]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        //Left (UP/DOWN/LEFT)
        else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.left) {
            //Debug.Log("[IN[L]]->[Out[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.up) {
            //Debug.Log("[IN[L]]->[Out[UP]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.down) {
            //Debug.Log("[IN[L]]->[Out[DOWN]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        //UP (UP/RIGHT/LEFT)
        else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.up) {
            //Debug.Log("[IN[UP]]->[Out[UP]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.right) {
            //Debug.Log("[IN[UP]]->[Out[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.left) {
            //Debug.Log("[IN[UP]]->[Out[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        //DOWN (DOWN/RIGHT/LEFT)
        else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.down) {
            //Debug.Log("[IN[DOWN]]->[Out[DOWN]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.right) {
            //Debug.Log("[IN[DOWN]]->[Out[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.left) {
            //Debug.Log("[IN[DOWN]]->[Out[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
    }

    public void setCorrectRouteFinalTile(int nodeX,int nodeY,int i) {
        Vector2 previousTile = new Vector2(unitPathToCursor[i - 1].x + 1, unitPathToCursor[i - 1].y + 1);
        Vector2 currentTile = new Vector2(unitPathToCursor[i].x + 1, unitPathToCursor[i].y + 1);
        Vector2 backToCurrentVector = directionBetween(previousTile, currentTile);

        if (backToCurrentVector == Vector2.right) {
            //Debug.Log("[IN[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.left) {
            //Debug.Log("[IN[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.up) {
            //Debug.Log("[IN[U]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.down) {
            //Debug.Log("[IN[D]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
    }

    public IEnumerator checkIfUnitsRemainCoroutine(GameObject unit, GameObject enemy) {
        while (unit.GetComponent<Unit>().combatQueue.Count != 0) {
            yield return new WaitForEndOfFrame();
        }
        while (enemy.GetComponent<Unit>().combatQueue.Count != 0) {
            yield return new WaitForEndOfFrame();
        }
        if (team1.transform.childCount == 0) {
            displayWinnerUI.enabled = true;
            displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Player 2 has won!");
        }
        else if (team2.transform.childCount == 0) {
            displayWinnerUI.enabled = true;
            displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Player 1 has won!");
        }
    }

    [ClientRpc]
    public void win() {
        displayWinnerUI.enabled = true;
        displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Winner!");
    }
    //------------------------------------------------------

    public struct Dependencies {
        public NewNetworkManager NetworkManager;
        public GameData Data;

        //public Material material;

        public bool IsValid() {
            return Data != null && NetworkManager != null;
        }
    }

    public void SetDependencies(Dependencies dependencies) {
        if (!dependencies.IsValid()) {
            Debug.LogError("Tried to setup with invalid dependencies");
            // TODO exit to menu?
            return;
        }

        room = dependencies.NetworkManager;
        Data = dependencies.Data;
        //material = dependencies.material;
            
        //Data.Init(this, config);
        //Grid.Init(this);
            
        //StartCoroutine(AwaitNetworkedPlayerRoutine());
    }

    public void SetGamePlayer (NewNetworkGamePlayer player) {
        gamePlayer = player;
    }

    public NewNetworkGamePlayer GetPlayerFromID(int id) {
        return room.GamePlayers.FirstOrDefault(p => p.ID == id);
    }

    public void StartGameClient() { 
        Debug.Log("Game started for client");
    }

    public void StartGameServer() {
        SetupPlayers();
        Data.NextTurn();
        Data.SetGameStarted();
            
        //Camera.ScrollTo(Grid.GetStartLocation(gamePlayer.ID));
            
        //Cells.RegisterCellLifecycleObserver(Data);
            
        Debug.Log("Game started for server");

        foreach (var player in room.GamePlayers) {
            player.RpcNotifyGameStart();
        }
    }

    private void SetupPlayers() {
        foreach (var player in room.GamePlayers) {
            //Grid.SetStartingLocation(player);
            //Data.OnSuppliesChanged(player, config.startingSupply);
        }
            //UI.UpdatePlayerInfo(room.GamePlayers);
    }

    public bool IsCurrentPlayer(int playerID) => Data.IsCurrentPlayer(playerID);
    public bool IsCurrentPlayer(NewNetworkGamePlayer player) => Data.IsCurrentPlayer(player.ID);
}