using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TileMap : NetworkBehaviour {

    [Header("Manager Scripts")]
    // for handling battles
    // for handling game things
    public GameManager GMS;
    
    [Header("Tiles")]
    // types of tiles
    public TileType[] tileTypes;
    // for creating an array of tiles
    public int[,] tiles;

    [Header("Map Units")]
    // the units on the map
    public GameObject unitsOnBoard;
    // the tiles on the map
    public GameObject[,] tilesOnMap;
    public GameObject[,] quadOnMap;
    public GameObject[,] quadOnMapForUnitMovementDisplay;
    public GameObject[,] quadOnMapCursor;
    public GameObject mapUI;
    public GameObject mapCursorUI;
    public GameObject mapUnitMovementUI;

    // for pathfinding 
    public List<Node> currentPath = null;
    public Node[,] graph;

    [Header("Containers")]
    public GameObject tileContainer;
    public GameObject UIQuadPotentialMovesContainer;
    public GameObject UIQuadCursorContainer;
    public GameObject UIUnitMovementPathContainer;

    [Header("Map Size")]
    public int mapSizeX;
    public int mapSizeY;

    [Header("Selected Unit Info")]
    public GameObject selectedUnit;
    public HashSet<Node> selectedUnitTotalRange;
    public HashSet<Node> selectedUnitMoveRange;
    public bool unitSelected = false;
    public int unitSelectedPreviousX;
    public int unitSelectedPreviousY;

    //[SyncVar]
    public GameObject previousOccupiedTile;

    [Header("Highlight Materials")]
    public Material redUIMat;
    public Material blueUIMat;

//------------------------- IGNORE SECTION -------------------------
//------------------------- Syncing Owner -------------------------

    public int PlayerId;

    public NewNetworkGamePlayer Owner;

    public int ID;
    

    protected GameData GameData;

    public void Init(GameData data, NewNetworkGamePlayer owner) {
        GameData = data;
        Owner = owner;
        PlayerId = Owner.ID;
        OnInit();
    }

    protected virtual void OnInit() {}

//------------------------- Map Setup -------------------------
// calling start to setup the map
    private void Start() {
        // creating an array of tiles and loading the map
        GenerateMapInfo();
        GenerateMapVisuals();

        // pathfinding for movement
        GeneratePathFindingGraph();
        SetIfTileIsOccupied();
    }

    private void Update() {
        // moves from here but crashes for having no tileclick
        //MouseClickToSelectUnit();
        if (Input.GetMouseButtonDown(0)) {
            if (selectedUnit == null) {
                GeneratePathFindingGraph();
                SetIfTileIsOccupied();
                MouseClickToSelectUnitV2();
                //moves from here but crashes for having no tileclick
                // GameObject tempSelectedUnit = GMS.tileBeingDisplayed.GetComponent<TileClick>().unitOnTile;
                // DisableHighlightUnitRange();
                // selectedUnit = tempSelectedUnit;
                // selectedUnit.GetComponent<Unit>().map = this;
                // selectedUnit.GetComponent<Unit>().SetMovementState(1);
                // unitSelected = true;
                // HighlightUnitRange();
                MouseClickToSelectUnit();
            }
            else if (selectedUnit.GetComponent<Unit>().unitMoveState == selectedUnit.GetComponent<Unit>().GetMovementStateEnum(1)
                    && selectedUnit.GetComponent<Unit>().movementQueue.Count == 0) {

                if (SelectTileToMoveTo()) {
                    Debug.Log("move path exists");
                    unitSelectedPreviousX = selectedUnit.GetComponent<Unit>().x;
                    unitSelectedPreviousY = selectedUnit.GetComponent<Unit>().y;
                    previousOccupiedTile = selectedUnit.GetComponent<Unit>().tileBeingOccupied;
                    //selectedUnit.GetComponent<Unit>().SetWalkingAnimation();
                    // from this check this is where client cannot sync
                    MoveUnit();
                    StartCoroutine(MoveUnitAndFinalise());
                }
            }
            else if(selectedUnit.GetComponent<Unit>().unitMoveState == selectedUnit.GetComponent<Unit>().GetMovementStateEnum(2)) {
                FinaliseOption();
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            if (selectedUnit != null) {
                if (selectedUnit.GetComponent<Unit>().movementQueue.Count == 0 && selectedUnit.GetComponent<Unit>().combatQueue.Count==0) {
                    if (selectedUnit.GetComponent<Unit>().unitMoveState != selectedUnit.GetComponent<Unit>().GetMovementStateEnum(3)) {
                        DeselectUnit();
                        }
                    }
                else if (selectedUnit.GetComponent<Unit>().movementQueue.Count == 1) {
                    selectedUnit.GetComponent<Unit>().visualMovementSpeed = 0.5f;
                }
            }
        }  
    }

//------------------------- Map Generation -------------------------
// for generating the visuals of the map
    public void GenerateMapInfo() {
        tiles = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                // setting all tiles to grass
                tiles[x, y] = 0;
            }
        }
        // setting tiles to forest
        tiles[1, 4] = 1;
        tiles[2, 6] = 1;
        tiles[3, 2] = 1;
        tiles[4, 5] = 1;
        tiles[9, 4] = 1;
        tiles[6, 3] = 1;
        tiles[7, 7] = 1;
    }

    public void GenerateMapVisuals() {
        tilesOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMapForUnitMovementDisplay = new GameObject[mapSizeX, mapSizeY];
        quadOnMapCursor = new GameObject[mapSizeX, mapSizeY];

        int index;
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                index = tiles[x, y];
                // generating actual visuals of the tile
                GameObject newTile = Instantiate(tileTypes[index].tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity);
                newTile.GetComponent<TileClick>().tileX = x;
                newTile.GetComponent<TileClick>().tileY = y;
                newTile.GetComponent<TileClick>().map = this;
                newTile.transform.SetParent(tileContainer.transform);
                tilesOnMap[x, y] = newTile;
                GameObject gridUI = Instantiate(mapUI, new Vector3(x, 0.501f, y),Quaternion.Euler(90f,0,0));
                gridUI.transform.SetParent(UIQuadPotentialMovesContainer.transform);
                quadOnMap[x, y] = gridUI;
                GameObject gridUIForPathfindingDisplay = Instantiate(mapUnitMovementUI, new Vector3(x, 0.502f, y), Quaternion.Euler(90f, 0, 0));
                gridUIForPathfindingDisplay.transform.SetParent(UIUnitMovementPathContainer.transform);
                quadOnMapForUnitMovementDisplay[x, y] = gridUIForPathfindingDisplay;
                GameObject gridUICursor = Instantiate(mapCursorUI, new Vector3(x, 0.503f, y), Quaternion.Euler(90f, 0, 0));
                gridUICursor.transform.SetParent(UIQuadCursorContainer.transform);              
                quadOnMapCursor[x, y] = gridUICursor;
            }
        }
    }

//------------------------- Map Pathfinding -------------------------
// for the unit pathfinding algorithm

    // initialise the graph for the nodes
    public void GeneratePathFindingGraph() {
        graph = new Node[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        // calculate the neighbour tiles for pathfinding
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {              
                if (x > 0) {                   
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                }
                if (x < mapSizeX-1) {                   
                    graph[x, y].neighbours.Add(graph[x + 1, y]);
                }
                if (y > 0) {
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                }
                if (y < mapSizeY - 1) {
                    graph[x, y].neighbours.Add(graph[x, y + 1]);
                } 
            }
        }
    }

    //[ClientRpc]
    public void SetIfTileIsOccupied() {
        foreach (Transform team in unitsOnBoard.transform) {
            foreach (Transform unitOnTeam in team) { 
                int unitX = unitOnTeam.GetComponent<Unit>().x;
                int unitY = unitOnTeam.GetComponent<Unit>().y;
                unitOnTeam.GetComponent<Unit>().tileBeingOccupied = tilesOnMap[unitX, unitY];
                tilesOnMap[unitX, unitY].GetComponent<TileClick>().unitOnTile = unitOnTeam.gameObject;
            }
        }
    }

    //[Command(requiresAuthority=false)]
    public void MoveUnit() {
        if (selectedUnit != null) {
            Debug.Log(selectedUnit);
            selectedUnit.GetComponent<Unit>().MoveNextTile();
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y) {
        return new Vector3(x, 0.75f, y);
    }

    public void GeneratePathTo(int x, int y) {
        if (selectedUnit.GetComponent<Unit>().x == x && selectedUnit.GetComponent<Unit>().y == y) {
            Debug.Log("clicked the same tile that the unit is standing on");
            currentPath = new List<Node>();
            selectedUnit.GetComponent<Unit>().path = currentPath;
            return;
        }
        if (unitCanEnterTile(x, y) == false){
            return;
        }

        selectedUnit.GetComponent<Unit>().path = null;
        currentPath = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        Node source = graph[selectedUnit.GetComponent<Unit>().x, selectedUnit.GetComponent<Unit>().y];
        Node target = graph[x, y];
        dist[source] = 0;
        prev[source] = null;
        List<Node> unvisited = new List<Node>();
        foreach (Node n in graph) {
            if (n != source) {
                dist[n] = Mathf.Infinity;
                prev[n] = null;
            }
            unvisited.Add(n);
        }
        while (unvisited.Count > 0) {
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
                float alt = dist[u] + costToEnterTile(n.x, n.y);
                if (alt < dist[n]) {
                    dist[n] = alt;
                    prev[n] = u;
                }
            }
        }
        if (prev[target] == null) {
            return;
        }
        currentPath = new List<Node>();
        Node curr = target;
        while (curr != null) {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        currentPath.Reverse();
        selectedUnit.GetComponent<Unit>().path = currentPath;
    }
    public float costToEnterTile(int x, int y) {
        if (unitCanEnterTile(x, y) == false) {
            return Mathf.Infinity;
        }
        TileType t = tileTypes[tiles[x, y]];
        float dist = t.movementCost;
        return dist;
    }
    public bool unitCanEnterTile(int x, int y) {
        if (tilesOnMap[x, y].GetComponent<TileClick>().unitOnTile != null) {
            if (tilesOnMap[x, y].GetComponent<TileClick>().unitOnTile.GetComponent<Unit>().teamNum != selectedUnit.GetComponent<Unit>().teamNum) {
                return false;
            }
        }
        return tileTypes[tiles[x, y]].isWalkable;
    }
    public void MouseClickToSelectUnit() {
        GameObject tempSelectedUnit;
        RaycastHit hit;       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            if (unitSelected == false) {  
                if (hit.transform.gameObject.CompareTag("Tile")) {
                    if (hit.transform.GetComponent<TileClick>().unitOnTile != null) {
                        tempSelectedUnit = hit.transform.GetComponent<TileClick>().unitOnTile;
                        if (tempSelectedUnit.GetComponent<Unit>().unitMoveState == tempSelectedUnit.GetComponent<Unit>().GetMovementStateEnum(0)
                            && tempSelectedUnit.GetComponent<Unit>().teamNum == GMS.currentTeam) {
                            DisableHighlightUnitRange();
                            selectedUnit = tempSelectedUnit;
                            selectedUnit.GetComponent<Unit>().map = this;
                            selectedUnit.GetComponent<Unit>().SetMovementState(1);
                            unitSelected = true;
                            HighlightUnitRange();
                        }
                    }
                }
                else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit")) {   
                    tempSelectedUnit = hit.transform.parent.gameObject;
                    if (tempSelectedUnit.GetComponent<Unit>().unitMoveState == tempSelectedUnit.GetComponent<Unit>().GetMovementStateEnum(0)
                          && tempSelectedUnit.GetComponent<Unit>().teamNum == GMS.currentTeam) {
                        DisableHighlightUnitRange();
                        selectedUnit = tempSelectedUnit;
                        selectedUnit.GetComponent<Unit>().SetMovementState(1);
                        selectedUnit.GetComponent<Unit>().map = this;
                        unitSelected = true;
                        HighlightUnitRange();
                    }
                }
            }
         }
    }
    //[Command(requiresAuthority=false)]
    public void FinaliseMovementPosition() {
        tilesOnMap[selectedUnit.GetComponent<Unit>().x, selectedUnit.GetComponent<Unit>().y].GetComponent<TileClick>().unitOnTile = selectedUnit;
        selectedUnit.GetComponent<Unit>().SetMovementState(2);
        HighlightUnitAttackOptionsFromPosition();
        HighlightTileUnitIsOccupying();
    }
    
    public void MouseClickToSelectUnitV2() {
        if (unitSelected == false && GMS.tileBeingDisplayed!=null) {
            if (GMS.tileBeingDisplayed.GetComponent<TileClick>().unitOnTile != null) {
                GameObject tempSelectedUnit = GMS.tileBeingDisplayed.GetComponent<TileClick>().unitOnTile;
                if (tempSelectedUnit.GetComponent<Unit>().unitMoveState == tempSelectedUnit.GetComponent<Unit>().GetMovementStateEnum(0)
                    && tempSelectedUnit.GetComponent<Unit>().teamNum == GMS.currentTeam) {
                    DisableHighlightUnitRange();
                    selectedUnit = tempSelectedUnit;
                    selectedUnit.GetComponent<Unit>().map = this;
                    selectedUnit.GetComponent<Unit>().SetMovementState(1);
                    unitSelected = true;
                    HighlightUnitRange();
                }
            }
        }
    }

    //[Command(requiresAuthority=false)]
    public void FinaliseOption() {
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    HashSet<Node> attackableTiles = GetUnitAttackOptionsFromPosition();
    if (Physics.Raycast(ray, out hit)) {
        if (hit.transform.gameObject.CompareTag("Tile")) {
            if (hit.transform.GetComponent<TileClick>().unitOnTile != null) {
                GameObject unitOnTile = hit.transform.GetComponent<TileClick>().unitOnTile;
                int unitX = unitOnTile.GetComponent<Unit>().x;
                int unitY = unitOnTile.GetComponent<Unit>().y;
                if (unitOnTile == selectedUnit) {
                    DisableHighlightUnitRange();
                    Debug.Log("ITS THE SAME UNIT JUST WAIT");
                    selectedUnit.GetComponent<Unit>().Wait();
                    //selectedUnit.GetComponent<Unit>().SetWaitIdleAnimation();
                    selectedUnit.GetComponent<Unit>().SetMovementState(3);
                    DeselectUnit();
                    }
                    else if (unitOnTile.GetComponent<Unit>().teamNum != selectedUnit.GetComponent<Unit>().teamNum && attackableTiles.Contains(graph[unitX,unitY])) {
                        if (unitOnTile.GetComponent<Unit>().currentHealthPoints > 0) {
                            Debug.Log("We clicked an enemy that should be attacked");
                            Debug.Log(selectedUnit.GetComponent<Unit>().currentHealthPoints);
                            // StartCoroutine(BMS.attack(selectedUnit, unitOnTile));
                            StartCoroutine(DeselectAfterMovements(selectedUnit, unitOnTile));
                        }
                    }                                     
            }
        }
        else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit")) {
            GameObject unitClicked = hit.transform.parent.gameObject;
            int unitX = unitClicked.GetComponent<Unit>().x;
            int unitY = unitClicked.GetComponent<Unit>().y;
            if (unitClicked == selectedUnit) {
                DisableHighlightUnitRange();
                Debug.Log("ITS THE SAME UNIT JUST WAIT");
                selectedUnit.GetComponent<Unit>().Wait();
                selectedUnit.GetComponent<Unit>().SetWaitIdleAnimation();
                selectedUnit.GetComponent<Unit>().SetMovementState(3);
                DeselectUnit();
            }
            else if (unitClicked.GetComponent<Unit>().teamNum != selectedUnit.GetComponent<Unit>().teamNum && attackableTiles.Contains(graph[unitX, unitY])) {
                    if (unitClicked.GetComponent<Unit>().currentHealthPoints > 0) {
                        Debug.Log("We clicked an enemy that should be attacked");
                        Debug.Log("Add Code to Attack enemy");
                        // StartCoroutine(BMS.attack(selectedUnit, unitClicked));
                        StartCoroutine(DeselectAfterMovements(selectedUnit, unitClicked));
                    }
            }
        }
    } 
}
    public void DeselectUnit() {  
        if (selectedUnit != null) {
            if (selectedUnit.GetComponent<Unit>().unitMoveState == selectedUnit.GetComponent<Unit>().GetMovementStateEnum(1)) {
            DisableHighlightUnitRange();
            DisableUnitUIRoute();
            selectedUnit.GetComponent<Unit>().SetMovementState(0);
            selectedUnit = null;
            unitSelected = false;
            }
            else if (selectedUnit.GetComponent<Unit>().unitMoveState == selectedUnit.GetComponent<Unit>().GetMovementStateEnum(3) ) {
                DisableHighlightUnitRange();
                DisableUnitUIRoute();
                unitSelected = false;
                selectedUnit = null;
            } else {
                DisableHighlightUnitRange();
                DisableUnitUIRoute();
                tilesOnMap[selectedUnit.GetComponent<Unit>().x, selectedUnit.GetComponent<Unit>().y].GetComponent<TileClick>().unitOnTile = null;
                tilesOnMap[unitSelectedPreviousX, unitSelectedPreviousY].GetComponent<TileClick>().unitOnTile = selectedUnit;
                selectedUnit.GetComponent<Unit>().x = unitSelectedPreviousX;
                selectedUnit.GetComponent<Unit>().y = unitSelectedPreviousY;
                selectedUnit.GetComponent<Unit>().tileBeingOccupied = previousOccupiedTile;
                selectedUnit.transform.position = TileCoordToWorldCoord(unitSelectedPreviousX, unitSelectedPreviousY);
                selectedUnit.GetComponent<Unit>().SetMovementState(0);
                selectedUnit = null;
                unitSelected = false;
            }
        }
    }
    public void HighlightUnitRange() {
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        HashSet<Node> finalEnemyUnitsInMovementRange = new HashSet<Node>();
        int attRange = selectedUnit.GetComponent<Unit>().attackRange;
        int moveSpeed = selectedUnit.GetComponent<Unit>().moveSpeed;
        Node unitInitialNode = graph[selectedUnit.GetComponent<Unit>().x, selectedUnit.GetComponent<Unit>().y];
        finalMovementHighlight = GetUnitMovementOptions();
        totalAttackableTiles = GetUnitTotalAttackableTiles(finalMovementHighlight, attRange, unitInitialNode);
        foreach (Node n in totalAttackableTiles) {
            if (tilesOnMap[n.x, n.y].GetComponent<TileClick>().unitOnTile != null) {
                GameObject unitOnCurrentlySelectedTile = tilesOnMap[n.x, n.y].GetComponent<TileClick>().unitOnTile;
                if (unitOnCurrentlySelectedTile.GetComponent<Unit>().teamNum != selectedUnit.GetComponent<Unit>().teamNum) {
                    finalEnemyUnitsInMovementRange.Add(n);
                }
            }
        }
        HighlightEnemiesInRange(totalAttackableTiles);
        HighlightMovementRange(finalMovementHighlight);
        selectedUnitMoveRange = finalMovementHighlight;
        selectedUnitTotalRange = GetUnitTotalRange(finalMovementHighlight, totalAttackableTiles);
    }
    public void DisableUnitUIRoute() {
        foreach(GameObject quad in quadOnMapForUnitMovementDisplay) {
            if (quad.GetComponent<Renderer>().enabled == true) {    
                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }
    public HashSet<Node> GetUnitMovementOptions() {
        float[,] cost = new float[mapSizeX, mapSizeY];
        HashSet<Node> UIHighlight = new HashSet<Node>();
        HashSet<Node> tempUIHighlight = new HashSet<Node>();
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();      
        int moveSpeed = selectedUnit.GetComponent<Unit>().moveSpeed;
        Node unitInitialNode = graph[selectedUnit.GetComponent<Unit>().x, selectedUnit.GetComponent<Unit>().y];
        finalMovementHighlight.Add(unitInitialNode);
        foreach (Node n in unitInitialNode.neighbours) {
            cost[n.x, n.y] = costToEnterTile(n.x, n.y);
            if (moveSpeed - cost[n.x, n.y] >= 0) {
                UIHighlight.Add(n);
            }
        }
        finalMovementHighlight.UnionWith(UIHighlight);
        while (UIHighlight.Count != 0) {
            foreach (Node n in UIHighlight) {
                foreach (Node neighbour in n.neighbours) {
                    if (!finalMovementHighlight.Contains(neighbour)) {
                        cost[neighbour.x, neighbour.y] = costToEnterTile(neighbour.x, neighbour.y) + cost[n.x, n.y];
                        if (moveSpeed - cost[neighbour.x, neighbour.y] >= 0) {
                            tempUIHighlight.Add(neighbour);
                        }
                    }
                }
            }
            UIHighlight = tempUIHighlight;
            finalMovementHighlight.UnionWith(UIHighlight);
            tempUIHighlight = new HashSet<Node>();
        }
        Debug.Log("The total amount of movable spaces for this unit is: " + finalMovementHighlight.Count);
        Debug.Log("We have used the function to calculate it this time");
        return finalMovementHighlight;
    }
    public HashSet<Node> GetUnitTotalRange(HashSet<Node> finalMovementHighlight, HashSet<Node> totalAttackableTiles) {
        HashSet<Node> unionTiles = new HashSet<Node>();
        unionTiles.UnionWith(finalMovementHighlight);
        unionTiles.UnionWith(totalAttackableTiles);
        return unionTiles;
    }
    public HashSet<Node> GetUnitTotalAttackableTiles(HashSet<Node> finalMovementHighlight, int attRange, Node unitInitialNode) {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        foreach (Node n in finalMovementHighlight) {
            neighbourHash = new HashSet<Node>();
            neighbourHash.Add(n);
            for (int i = 0; i < attRange; i++) {
                foreach (Node t in neighbourHash) {
                    foreach (Node tn in t.neighbours) {
                        tempNeighbourHash.Add(tn);
                    }
                }
                neighbourHash = tempNeighbourHash;
                tempNeighbourHash = new HashSet<Node>();
                if (i < attRange - 1) {
                    seenNodes.UnionWith(neighbourHash);
                }
            }
            neighbourHash.ExceptWith(seenNodes);
            seenNodes = new HashSet<Node>();
            totalAttackableTiles.UnionWith(neighbourHash);
        }
        totalAttackableTiles.Remove(unitInitialNode);
        return (totalAttackableTiles);
    }
    public HashSet<Node> GetUnitAttackOptionsFromPosition() {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        Node initialNode = graph[selectedUnit.GetComponent<Unit>().x, selectedUnit.GetComponent<Unit>().y];
        int attRange = selectedUnit.GetComponent<Unit>().attackRange;
        neighbourHash = new HashSet<Node>();
        neighbourHash.Add(initialNode);
        for (int i = 0; i < attRange; i++) {
            foreach (Node t in neighbourHash) {
                foreach (Node tn in t.neighbours) {
                    tempNeighbourHash.Add(tn);
                }
            }
            neighbourHash = tempNeighbourHash;
            tempNeighbourHash = new HashSet<Node>();
            if (i < attRange - 1) {
                seenNodes.UnionWith(neighbourHash);
            }
        }
        neighbourHash.ExceptWith(seenNodes);
        neighbourHash.Remove(initialNode);
        return neighbourHash;
    }
    public HashSet<Node> GetTileUnitIsOccupying() {
       
        int x = selectedUnit.GetComponent<Unit>().x;
        int y = selectedUnit.GetComponent<Unit>().y;
        HashSet<Node> singleTile = new HashSet<Node>();
        singleTile.Add(graph[x, y]);
        return singleTile;
    }
    public void HighlightTileUnitIsOccupying() {
        if (selectedUnit != null) {
            HighlightMovementRange(GetTileUnitIsOccupying());
        }
    }
    public void HighlightUnitAttackOptionsFromPosition() {
        if (selectedUnit != null) {
            HighlightEnemiesInRange(GetUnitAttackOptionsFromPosition());
        }
    }
    public void HighlightMovementRange(HashSet<Node> movementToHighlight) {
        foreach (Node n in movementToHighlight) {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = blueUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;
        }
    }
    public void HighlightEnemiesInRange(HashSet<Node> enemiesToHighlight) {
        foreach (Node n in enemiesToHighlight) {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = redUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;
        }
    }
    public void DisableHighlightUnitRange() {
        foreach(GameObject quad in quadOnMap) {
            if(quad.GetComponent<Renderer>().enabled == true) {
                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }
    public IEnumerator MoveUnitAndFinalise() {
        DisableHighlightUnitRange();
        DisableUnitUIRoute();
        while (selectedUnit.GetComponent<Unit>().movementQueue.Count != 0) {
            yield return new WaitForEndOfFrame();
        }
        FinaliseMovementPosition();
        //selectedUnit.GetComponent<Unit>().SetSelectedAnimation();
    }
    public IEnumerator DeselectAfterMovements(GameObject unit, GameObject enemy) {
        selectedUnit.GetComponent<Unit>().SetMovementState(3);
        DisableHighlightUnitRange();
        DisableUnitUIRoute();
        yield return new WaitForSeconds(.25f);
        while (unit.GetComponent<Unit>().combatQueue.Count > 0) {
            yield return new WaitForEndOfFrame();
        }
        while (enemy.GetComponent<Unit>().combatQueue.Count > 0) {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("All animations done playing");
        DeselectUnit();
    }
    public bool SelectTileToMoveTo() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) { 
            if (hit.transform.gameObject.CompareTag("Tile")) {
                int clickedTileX = hit.transform.GetComponent<TileClick>().tileX;
                int clickedTileY = hit.transform.GetComponent<TileClick>().tileY;
                Node nodeToCheck = graph[clickedTileX, clickedTileY];

                if (selectedUnitMoveRange.Contains(nodeToCheck)) {
                    if ((hit.transform.gameObject.GetComponent<TileClick>().unitOnTile == null || hit.transform.gameObject.GetComponent<TileClick>().unitOnTile == selectedUnit) && (selectedUnitMoveRange.Contains(nodeToCheck))) {
                        Debug.Log("We have finally selected the tile to move to");
                        GeneratePathTo(clickedTileX, clickedTileY);
                        return true;
                    }
                }
            }
            else if (hit.transform.gameObject.CompareTag("Unit")) {
                if (hit.transform.parent.GetComponent<Unit>().teamNum != selectedUnit.GetComponent<Unit>().teamNum) {
                    Debug.Log("Clicked an Enemy");
                }
                else if(hit.transform.parent.gameObject == selectedUnit) {    
                    GeneratePathTo(selectedUnit.GetComponent<Unit>().x, selectedUnit.GetComponent<Unit>().y);
                    return true;
                }
            }
        }
        return false;
    }
}