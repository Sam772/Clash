using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class GenericTileMap : NetworkBehaviour {
    public BattleManager battleManager;
    public GameManager gameManager;
    public TileType[] tileTypes;
    public int[,] tiles;
    public GameObject[,] tilesOnMap;
    public GameObject[,] quadOnMap;
    public GameObject[,] quadOnMapForUnitMovementDisplay;
    public GameObject[,] quadOnMapCursor;
    public GameObject mapUI;
    public GameObject mapCursorUI;
    public GameObject mapUnitMovementUI;
    public List<Node> currentPath = null;
    public Node[,] graph;
    public GameObject tileContainer;
    public GameObject UIQuadPotentialMovesContainer;
    public GameObject UIQuadCursorContainer;
    public GameObject UIUnitMovementPathContainer;
    public int mapSizeX;
    public int mapSizeY;
    public GameObject selectedUnit;
    public HashSet<Node> selectedUnitTotalRange;
    public HashSet<Node> selectedUnitMoveRange;
    private bool unitSelected = false;
    private int unitSelectedPreviousX;
    private int unitSelectedPreviousY;
    private GameObject previousOccupiedTile;
    public Material enemyRangeUIMat;
    public Material movementTileUIMat;
    [SyncVar]
    public int actionChoice;
    public bool skillUsed = false;
    public SkillManager skillManager;


    private void Start() {
        GenerateMapInfo();
        GenerateMapVisuals();
        GeneratePathFindingGraph();
        SetIfTileIsOccupied();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (selectedUnit == null) {
                GeneratePathFindingGraph();
                SetIfTileIsOccupied();
                MouseClickToSelectUnitV2();
                MouseClickToSelectUnit();
            } else if (selectedUnit.GetComponent<GenericUnit>().unitMoveState == selectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(1) && selectedUnit.GetComponent<GenericUnit>().movementQueue.Count == 0) {
                if (SelectTileToMoveTo()) {
                    unitSelectedPreviousX = selectedUnit.GetComponent<GenericUnit>().x;
                    unitSelectedPreviousY = selectedUnit.GetComponent<GenericUnit>().y;
                    previousOccupiedTile = selectedUnit.GetComponent<GenericUnit>().tileBeingOccupied;
                    MoveUnit();
                    StartCoroutine(MoveUnitAndFinalise());}
            } else if(selectedUnit.GetComponent<GenericUnit>().unitMoveState == selectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(2)) { FinaliseOption(); }}
        if (Input.GetMouseButtonDown(1)) {
            if (selectedUnit != null) {
                if (selectedUnit.GetComponent<GenericUnit>().movementQueue.Count == 0 && selectedUnit.GetComponent<GenericUnit>().combatQueue.Count == 0) {
                    if (selectedUnit.GetComponent<GenericUnit>().unitMoveState != selectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(3)) { DeselectUnit(); }}
                else if (selectedUnit.GetComponent<GenericUnit>().movementQueue.Count == 1) { selectedUnit.GetComponent<GenericUnit>().visualMovementSpeed = 0.5f; }
            }
        }  
    }

    public abstract void GenerateMapInfo();

    public void GenerateMapVisuals() {
        tilesOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMapForUnitMovementDisplay = new GameObject[mapSizeX, mapSizeY];
        quadOnMapCursor = new GameObject[mapSizeX, mapSizeY];
        int index;
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                index = tiles[x, y];
                GameObject newTile = Instantiate(tileTypes[index].tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity);
                newTile.GetComponent<TileClick>().tileX = x;
                newTile.GetComponent<TileClick>().tileY = y;
                newTile.GetComponent<TileClick>().map = this;
                newTile.transform.SetParent(tileContainer.transform);
                tilesOnMap[x, y] = newTile;
                GameObject gridUI = Instantiate(mapUI, new Vector3(x, 0.501f, y),Quaternion.Euler(90f ,0 ,0));
                gridUI.transform.SetParent(UIQuadPotentialMovesContainer.transform);
                quadOnMap[x, y] = gridUI;
                GameObject gridUIForPathfindingDisplay = Instantiate(mapUnitMovementUI, new Vector3(x, 0.502f, y), Quaternion.Euler(90f, 0, 0));
                gridUIForPathfindingDisplay.transform.SetParent(UIUnitMovementPathContainer.transform);
                quadOnMapForUnitMovementDisplay[x, y] = gridUIForPathfindingDisplay;
                GameObject gridUICursor = Instantiate(mapCursorUI, new Vector3(x, 0.503f, y), Quaternion.Euler(90f, 0, 0));
                gridUICursor.transform.SetParent(UIQuadCursorContainer.transform);              
                quadOnMapCursor[x, y] = gridUICursor;}}
    }

    public void GeneratePathFindingGraph() {
        graph = new Node[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;}}
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {              
                if (x > 0) { graph[x, y].neighbours.Add(graph[x - 1, y]); }
                if (x < mapSizeX-1) { graph[x, y].neighbours.Add(graph[x + 1, y]); }
                if (y > 0) { graph[x, y].neighbours.Add(graph[x, y - 1]); }
                if (y < mapSizeY - 1) { graph[x, y].neighbours.Add(graph[x, y + 1]); }}}
    }

    public void SetIfTileIsOccupied() {
        GenericUnit[] unitsList = FindObjectsOfType<GenericUnit>();
        foreach (GenericUnit unitOnTeam in unitsList) { 
            int unitX = unitOnTeam.GetComponent<GenericUnit>().x;
            int unitY = unitOnTeam.GetComponent<GenericUnit>().y;
            unitOnTeam.GetComponent<GenericUnit>().tileBeingOccupied = tilesOnMap[unitX, unitY];
            tilesOnMap[unitX, unitY].GetComponent<TileClick>().unitOnTile = unitOnTeam.gameObject;}
    }

    public void MoveUnit() {
        if (selectedUnit != null) { selectedUnit.GetComponent<GenericUnit>().MoveNextTile(); }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y) {
        return new Vector3(x, 0.88f, y);
    }

    public void GeneratePathTo(int x, int y) {
        if (selectedUnit.GetComponent<GenericUnit>().x == x && selectedUnit.GetComponent<GenericUnit>().y == y) {
            currentPath = new List<Node>();
            selectedUnit.GetComponent<GenericUnit>().path = currentPath;
            return;}
        if (UnitCanEnterTile(x, y) == false){ return; }
        selectedUnit.GetComponent<GenericUnit>().path = null;
        currentPath = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        Node source = graph[selectedUnit.GetComponent<GenericUnit>().x, selectedUnit.GetComponent<GenericUnit>().y];
        Node target = graph[x, y];
        dist[source] = 0;
        prev[source] = null;
        List<Node> unvisited = new List<Node>();
        foreach (Node n in graph) {
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
                float alt = dist[u] + CostToEnterTile(n.x, n.y);
                if (alt < dist[n]) {
                    dist[n] = alt;
                    prev[n] = u;}}}
        if (prev[target] == null) { return; }
        currentPath = new List<Node>();
        Node curr = target;
        while (curr != null) {
            currentPath.Add(curr);
            curr = prev[curr];}
        currentPath.Reverse();
        selectedUnit.GetComponent<GenericUnit>().path = currentPath;
    }

    public float CostToEnterTile(int x, int y) {
        if (UnitCanEnterTile(x, y) == false) { return Mathf.Infinity; }
        TileType t = tileTypes[tiles[x, y]];
        float dist = t.movementCost;
        return dist;
    }

    public bool UnitCanEnterTile(int x, int y) {
        if (tilesOnMap[x, y].GetComponent<TileClick>().unitOnTile != null) {
            if (tilesOnMap[x, y].GetComponent<TileClick>().unitOnTile.GetComponent<GenericUnit>().team != selectedUnit.GetComponent<GenericUnit>().team) { return false; }}
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
                        if (tempSelectedUnit.GetComponent<GenericUnit>().unitMoveState == tempSelectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(0) && tempSelectedUnit.GetComponent<GenericUnit>().team == gameManager.currentTeam) {
                            DisableHighlightUnitRange();
                            showActionMenu();
                            selectedUnit = tempSelectedUnit;
                            selectedUnit.GetComponent<GenericUnit>().map = this;
                            selectedUnit.GetComponent<GenericUnit>().SetMovementState(1);
                            unitSelected = true;
                            HighlightUnitRange();
                        }
                    }
                }
                else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit")) {   
                    tempSelectedUnit = hit.transform.parent.gameObject;
                    if (tempSelectedUnit.GetComponent<GenericUnit>().unitMoveState == tempSelectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(0) && tempSelectedUnit.GetComponent<GenericUnit>().team == gameManager.currentTeam) {
                        DisableHighlightUnitRange();
                        showActionMenu();

                        selectedUnit = tempSelectedUnit;
                        selectedUnit.GetComponent<GenericUnit>().SetMovementState(1);
                        selectedUnit.GetComponent<GenericUnit>().map = this;
                        unitSelected = true;
                        HighlightUnitRange();}}}}
    }

    public void FinaliseMovementPosition() {
        tilesOnMap[selectedUnit.GetComponent<GenericUnit>().x, selectedUnit.GetComponent<GenericUnit>().y].GetComponent<TileClick>().unitOnTile = selectedUnit;
        selectedUnit.GetComponent<GenericUnit>().SetMovementState(2);
        HighlightUnitAttackOptionsFromPosition();
        HighlightTileUnitIsOccupying();
    }
    public void showActionMenu()
    {
        actionChoice = 0;
        gameManager.ActionCanvas.enabled = true;
    }
    public void hideActionMenu()
    {
        gameManager.ActionCanvas.enabled = false;
    }
    public void MouseClickToSelectUnitV2() {
        if (unitSelected == false && gameManager.tileBeingDisplayed!=null) {
            if (gameManager.tileBeingDisplayed.GetComponent<TileClick>().unitOnTile != null) {
                GameObject tempSelectedUnit = gameManager.tileBeingDisplayed.GetComponent<TileClick>().unitOnTile;
                if (tempSelectedUnit.GetComponent<GenericUnit>().unitMoveState == tempSelectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(0)
                    && tempSelectedUnit.GetComponent<GenericUnit>().team == gameManager.currentTeam) {
                    DisableHighlightUnitRange();
                    showActionMenu();

                    selectedUnit = tempSelectedUnit;
                    selectedUnit.GetComponent<GenericUnit>().map = this;
                    selectedUnit.GetComponent<GenericUnit>().SetMovementState(1);
                    unitSelected = true;
                    HighlightUnitRange();}}}
    }

    public void FinaliseOption() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HashSet<Node> attackableTiles = GetUnitAttackOptionsFromPosition();
        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.gameObject.CompareTag("Tile")) {
                if (hit.transform.GetComponent<TileClick>().unitOnTile != null) {
                    GameObject unitOnTile = hit.transform.GetComponent<TileClick>().unitOnTile;
                    int unitX = unitOnTile.GetComponent<GenericUnit>().x;
                    int unitY = unitOnTile.GetComponent<GenericUnit>().y;
                    if (unitOnTile == selectedUnit) {
                        DisableHighlightUnitRange();
                        selectedUnit.GetComponent<GenericUnit>().Wait();
                        selectedUnit.GetComponent<GenericUnit>().SetMovementState(3);
                        DeselectUnit();
                    } else if (unitOnTile.GetComponent<GenericUnit>().team != selectedUnit.GetComponent<GenericUnit>().team && attackableTiles.Contains(graph[unitX, unitY])) {
                        if (unitOnTile.GetComponent<GenericUnit>().currentHealth > 0) {
                            StartCoroutine(battleManager.Attack(selectedUnit, unitOnTile));
                            StartCoroutine(DeselectAfterMovements(selectedUnit, unitOnTile));}}}}
            else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit")) {
                GameObject unitClicked = hit.transform.parent.gameObject;
                int unitX = unitClicked.GetComponent<GenericUnit>().x;
                int unitY = unitClicked.GetComponent<GenericUnit>().y;
                if (unitClicked == selectedUnit) {
                    DisableHighlightUnitRange();
                    selectedUnit.GetComponent<GenericUnit>().Wait();
                    selectedUnit.GetComponent<GenericUnit>().SetMovementState(3);
                    DeselectUnit();
                } else if (unitClicked.GetComponent<GenericUnit>().team != selectedUnit.GetComponent<GenericUnit>().team && attackableTiles.Contains(graph[unitX, unitY])) {
                    if (unitClicked.GetComponent<GenericUnit>().currentHealth > 0) {
                        StartCoroutine(battleManager.Attack(selectedUnit, unitClicked));
                        StartCoroutine(DeselectAfterMovements(selectedUnit, unitClicked));}}}}
    }

    public void DeselectUnit() {  
        if (selectedUnit != null) {
            skillManager.SkillDeactivation();

            if (selectedUnit.GetComponent<GenericUnit>().unitMoveState == selectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(1)) {
            DisableHighlightUnitRange();
            DisableUnitUIRoute();
            selectedUnit.GetComponent<GenericUnit>().SetMovementState(0);
            selectedUnit = null;
            unitSelected = false;
            } else if (selectedUnit.GetComponent<GenericUnit>().unitMoveState == selectedUnit.GetComponent<GenericUnit>().GetMovementStateEnum(3) ) {
                DisableHighlightUnitRange();
                DisableUnitUIRoute();
                unitSelected = false;
                selectedUnit = null;
            } else {
                DisableHighlightUnitRange();
                DisableUnitUIRoute();
                tilesOnMap[selectedUnit.GetComponent<GenericUnit>().x, selectedUnit.GetComponent<GenericUnit>().y].GetComponent<TileClick>().unitOnTile = null;
                tilesOnMap[unitSelectedPreviousX, unitSelectedPreviousY].GetComponent<TileClick>().unitOnTile = selectedUnit;
                selectedUnit.GetComponent<GenericUnit>().x = unitSelectedPreviousX;
                selectedUnit.GetComponent<GenericUnit>().y = unitSelectedPreviousY;
                selectedUnit.GetComponent<GenericUnit>().tileBeingOccupied = previousOccupiedTile;
                selectedUnit.transform.position = TileCoordToWorldCoord(unitSelectedPreviousX, unitSelectedPreviousY);
                selectedUnit.GetComponent<GenericUnit>().SetMovementState(0);
                selectedUnit = null;
                unitSelected = false;}}
        hideActionMenu();

    }

    public void HighlightUnitRange() {
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        HashSet<Node> finalEnemyUnitsInMovementRange = new HashSet<Node>();
        int attRange = selectedUnit.GetComponent<GenericUnit>().range;
        int moveSpeed = selectedUnit.GetComponent<GenericUnit>().move;
        Node unitInitialNode = graph[selectedUnit.GetComponent<GenericUnit>().x, selectedUnit.GetComponent<GenericUnit>().y];
        finalMovementHighlight = GetUnitMovementOptions();
        totalAttackableTiles = GetUnitTotalAttackableTiles(finalMovementHighlight, attRange, unitInitialNode);
        foreach (Node n in totalAttackableTiles) {
            if (tilesOnMap[n.x, n.y].GetComponent<TileClick>().unitOnTile != null) {
                GameObject unitOnCurrentlySelectedTile = tilesOnMap[n.x, n.y].GetComponent<TileClick>().unitOnTile;
                if (unitOnCurrentlySelectedTile.GetComponent<GenericUnit>().team != selectedUnit.GetComponent<GenericUnit>().team) { finalEnemyUnitsInMovementRange.Add(n); }}}
        HighlightEnemiesInRange(totalAttackableTiles);
        HighlightMovementRange(finalMovementHighlight);
        selectedUnitMoveRange = finalMovementHighlight;
        selectedUnitTotalRange = GetUnitTotalRange(finalMovementHighlight, totalAttackableTiles);
    }

    public void DisableUnitUIRoute() {
        foreach(GameObject quad in quadOnMapForUnitMovementDisplay) { if (quad.GetComponent<Renderer>().enabled == true) { quad.GetComponent<Renderer>().enabled = false; }}
    }

    public HashSet<Node> GetUnitMovementOptions() {
        float[,] cost = new float[mapSizeX, mapSizeY];
        HashSet<Node> UIHighlight = new HashSet<Node>();
        HashSet<Node> tempUIHighlight = new HashSet<Node>();
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();      
        int moveSpeed = selectedUnit.GetComponent<GenericUnit>().move;
        Node unitInitialNode = graph[selectedUnit.GetComponent<GenericUnit>().x, selectedUnit.GetComponent<GenericUnit>().y];
        finalMovementHighlight.Add(unitInitialNode);
        foreach (Node n in unitInitialNode.neighbours) {
            cost[n.x, n.y] = CostToEnterTile(n.x, n.y);
            if (moveSpeed - cost[n.x, n.y] >= 0) { UIHighlight.Add(n); }}
        finalMovementHighlight.UnionWith(UIHighlight);
        while (UIHighlight.Count != 0) {
            foreach (Node n in UIHighlight) {
                foreach (Node neighbour in n.neighbours) {
                    if (!finalMovementHighlight.Contains(neighbour)) { cost[neighbour.x, neighbour.y] = CostToEnterTile(neighbour.x, neighbour.y) + cost[n.x, n.y];
                        if (moveSpeed - cost[neighbour.x, neighbour.y] >= 0) { tempUIHighlight.Add(neighbour); }}}}
            UIHighlight = tempUIHighlight;
            finalMovementHighlight.UnionWith(UIHighlight);
            tempUIHighlight = new HashSet<Node>();}
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
                    foreach (Node tn in t.neighbours) { tempNeighbourHash.Add(tn); }}
                neighbourHash = tempNeighbourHash;
                tempNeighbourHash = new HashSet<Node>();
                if (i < attRange - 1) { seenNodes.UnionWith(neighbourHash); }}
            neighbourHash.ExceptWith(seenNodes);
            seenNodes = new HashSet<Node>();
            totalAttackableTiles.UnionWith(neighbourHash);}
        totalAttackableTiles.Remove(unitInitialNode);
        return (totalAttackableTiles);
    }

    public HashSet<Node> GetUnitAttackOptionsFromPosition() {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        Node initialNode = graph[selectedUnit.GetComponent<GenericUnit>().x, selectedUnit.GetComponent<GenericUnit>().y];
        int attRange = selectedUnit.GetComponent<GenericUnit>().range;
        neighbourHash = new HashSet<Node>();
        neighbourHash.Add(initialNode);
        for (int i = 0; i < attRange; i++) {
            foreach (Node t in neighbourHash) {
                foreach (Node tn in t.neighbours) { tempNeighbourHash.Add(tn); }}
            neighbourHash = tempNeighbourHash;
            tempNeighbourHash = new HashSet<Node>();
            if (i < attRange - 1) { seenNodes.UnionWith(neighbourHash); }}
        neighbourHash.ExceptWith(seenNodes);
        neighbourHash.Remove(initialNode);
        return neighbourHash;
    }

    public HashSet<Node> GetTileUnitIsOccupying() {
        int x = selectedUnit.GetComponent<GenericUnit>().x;
        int y = selectedUnit.GetComponent<GenericUnit>().y;
        HashSet<Node> singleTile = new HashSet<Node>();
        singleTile.Add(graph[x, y]);
        return singleTile;
    }

    public void HighlightTileUnitIsOccupying() {
        if (selectedUnit != null) { HighlightMovementRange(GetTileUnitIsOccupying()); }
    }

    public void HighlightUnitAttackOptionsFromPosition() {
        if (selectedUnit != null) { HighlightEnemiesInRange(GetUnitAttackOptionsFromPosition()); }
    }

    public void HighlightMovementRange(HashSet<Node> movementToHighlight) {
        foreach (Node n in movementToHighlight) {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = movementTileUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;}
    }

    public void HighlightEnemiesInRange(HashSet<Node> enemiesToHighlight) {
        foreach (Node n in enemiesToHighlight) {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = enemyRangeUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;}
    }

    public void DisableHighlightUnitRange() {
        foreach(GameObject quad in quadOnMap) { if (quad.GetComponent<Renderer>().enabled == true) { quad.GetComponent<Renderer>().enabled = false; }}
    }

    public IEnumerator MoveUnitAndFinalise() {
        DisableHighlightUnitRange();
        DisableUnitUIRoute();
        while (selectedUnit.GetComponent<GenericUnit>().movementQueue.Count != 0) { yield return new WaitForEndOfFrame(); }
        FinaliseMovementPosition();
    }

    public IEnumerator DeselectAfterMovements(GameObject unit, GameObject enemy) {
        selectedUnit.GetComponent<GenericUnit>().SetMovementState(3);
        DisableHighlightUnitRange();
        DisableUnitUIRoute();
        yield return new WaitForSeconds(.1f);
        if (unit && enemy != null) {
            while (unit.GetComponent<GenericUnit>().combatQueue.Count > 0) { yield return new WaitForEndOfFrame(); }
            while (enemy.GetComponent<GenericUnit>().combatQueue.Count > 0) { yield return new WaitForEndOfFrame(); }
        }
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
                        GeneratePathTo(clickedTileX, clickedTileY);
                        return true;}}
            } else if (hit.transform.gameObject.CompareTag("Unit")) {
                if (hit.transform.parent.gameObject == selectedUnit) {    
                    GeneratePathTo(selectedUnit.GetComponent<GenericUnit>().x, selectedUnit.GetComponent<GenericUnit>().y);
                    return true;}}}
        return false;}
}