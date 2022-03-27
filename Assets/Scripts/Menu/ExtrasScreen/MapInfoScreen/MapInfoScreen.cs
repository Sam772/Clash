using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class MapInfoScreen : MenuScreen
{
    [SerializeField] private GameObject maponeinfo;
    [SerializeField] private GameObject maptwoinfo;
    [SerializeField] private GameObject mapthreeinfo;
    [SerializeField] private GameObject mapfourinfo;

    //At startup, grasslands info will display by default
    void Start(){
        maponeinfo.SetActive(true);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(false);
    }

    // Clicking grasslands button displays grasslands info and hides other info
    public void ShowMapOneInfo(){
        maponeinfo.SetActive(true);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(false);
    }

    // Clicking highlands button displays highlands info and hides other info
    public void ShowMapTwoInfo(){
        maponeinfo.SetActive(false);
        maptwoinfo.SetActive(true);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(false);
    }

    // Clicking sandlands button displays sandlands info and hides other info
    public void ShowMapThreeInfo(){
        maponeinfo.SetActive(false);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(true);
        mapfourinfo.SetActive(false);
    }

    // Clicking frostlands button displays frostlands info and hides other info
    public void ShowMapFourInfo(){
        maponeinfo.SetActive(false);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(true);
    }
}
