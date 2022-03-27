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
    // Removes the background image when the credits are selected 
    void Start(){
        background.SetActive(false);
    }
    // When leaving credits screen the background image is back to normal
    public void returnBackground(){
        background.SetActive(true);
    }
}
