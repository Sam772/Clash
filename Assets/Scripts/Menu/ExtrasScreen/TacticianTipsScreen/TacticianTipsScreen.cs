using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class TacticianTipsScreen : MenuScreen {
    [SerializeField] private GameObject unitsinfo;
    [SerializeField] private GameObject movementinfo;
    [SerializeField] private GameObject turnorderinfo;
    [SerializeField] private GameObject unitstatsinfo;
    [SerializeField] private GameObject attackinginfo;
    [SerializeField] private GameObject terraininfo;
    [SerializeField] private GameObject skillsinfo;
    [SerializeField] private GameObject tilesinfo;
    [SerializeField] private GameObject camerainfo;
    [SerializeField] private GameObject howtowininfo;
    
        
    // At startup, first screen menu will be active by default
    void Start(){
        unitsinfo.SetActive(true);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);

    }

    // Clicking unit info button displays unit info and hides other info
    public void ShowUnitsInfo(){
        unitsinfo.SetActive(true);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }
   
   // Clicking movement button displays movement info and hides other info
    public void ShowMovementInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(true);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }

    // Clicking Turn Order button displays Turn Order info and hides other info
    public void ShowTurnOrderInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(true);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }

    // Clicking unit Stats button displays unit stats info and hides other info
    public void ShowUnitStatsInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(true);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }

    // Clicking Attacking button displays Attacking info and hides other info
    public void ShowAttackingInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(true);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }

    // Clicking Terrain button displays Terrain info and hides other info
    public void ShowTerrainInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(true);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }

    // Clicking Skills button displays Skills info and hides other info
    public void ShowSkillsInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(true);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }

    // Clicking Tiles button displays Tiles info and hides other info
    public void ShowTilesInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(true);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(false);        
    }

    // Clicking Camera button displays Camera info and hides other info
    public void ShowCameraInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(true);
        howtowininfo.SetActive(false);        
    }

    // Clicking How to win button displays How to win info and hides other info
    public void ShowHowToWinInfo(){
        unitsinfo.SetActive(false);
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        unitstatsinfo.SetActive(false);
        attackinginfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        camerainfo.SetActive(false);
        howtowininfo.SetActive(true);        
    }
}  
