using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.UI;

public class GameCreditsScreen : MenuScreen
{
    [SerializeField] private GameObject background;
    void Start(){
        background.SetActive(false);
    }
    public void returnBackground(){
        background.SetActive(true);
    }
}
