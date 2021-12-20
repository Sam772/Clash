using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerGame : MonoBehaviour {
    
    public void LoadTitleScreen() {
        SceneManager.LoadScene(0);
    }
}