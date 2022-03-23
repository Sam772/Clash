using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class TacticianTipsScreen : MenuScreen {
    [SerializeField] private GameObject movementinfo;
    [SerializeField] private GameObject turnorderinfo;
    [SerializeField] private GameObject howtowininfo;
    [SerializeField] private GameObject unitsinfo;
    [SerializeField] private GameObject tilesinfo;
    [SerializeField] private GameObject terraininfo;
    [SerializeField] private GameObject skillsinfo;

    void Start(){
        movementinfo.SetActive(true);
        turnorderinfo.SetActive(false);
        howtowininfo.SetActive(false);
        unitsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false);
    }

    public void ShowMovementInfo(){
        movementinfo.SetActive(true);
        turnorderinfo.SetActive(false);
        howtowininfo.SetActive(false);
        unitsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false); 
    }

    public void ShowTurnOrderInfo(){
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(true);
        howtowininfo.SetActive(false);
        unitsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false); 
    }

    public void ShowHowToWinInfo(){
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        howtowininfo.SetActive(true);
        unitsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false); 
    }

    public void ShowUnitsInfo(){
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        howtowininfo.SetActive(false);
        unitsinfo.SetActive(true);
        tilesinfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false); 
    }

    public void ShowTilesInfo(){
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        howtowininfo.SetActive(false);
        unitsinfo.SetActive(false);
        tilesinfo.SetActive(true);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(false); 
    }

    public void ShowTerrainInfo(){
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        howtowininfo.SetActive(false);
        unitsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        terraininfo.SetActive(true);
        skillsinfo.SetActive(false); 
    }

    public void ShowSkillsInfo(){
        movementinfo.SetActive(false);
        turnorderinfo.SetActive(false);
        howtowininfo.SetActive(false);
        unitsinfo.SetActive(false);
        tilesinfo.SetActive(false);
        terraininfo.SetActive(false);
        skillsinfo.SetActive(true); 
    }

}  
