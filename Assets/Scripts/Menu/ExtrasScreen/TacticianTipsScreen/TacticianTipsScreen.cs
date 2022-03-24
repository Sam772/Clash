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
