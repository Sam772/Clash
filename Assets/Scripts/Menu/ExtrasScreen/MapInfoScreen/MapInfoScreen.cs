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

    void Start(){
        maponeinfo.SetActive(true);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(false);
    }

    public void ShowMapOneInfo(){
        maponeinfo.SetActive(true);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(false);
    }

    public void ShowMapTwoInfo(){
        maponeinfo.SetActive(false);
        maptwoinfo.SetActive(true);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(false);
    }

    public void ShowMapThreeInfo(){
        maponeinfo.SetActive(false);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(true);
        mapfourinfo.SetActive(false);
    }

    public void ShowMapFourInfo(){
        maponeinfo.SetActive(false);
        maptwoinfo.SetActive(false);
        mapthreeinfo.SetActive(false);
        mapfourinfo.SetActive(true);
    }
}
