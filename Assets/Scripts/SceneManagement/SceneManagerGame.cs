using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerGame : MonoBehaviour {
    
    public void LoadTitleScreen() {
        SceneManager.LoadScene(0);
        // destroy gameplayer objects here before returning to the menu
    }
}