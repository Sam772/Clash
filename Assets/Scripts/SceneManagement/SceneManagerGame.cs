using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerGame : MonoBehaviour {
    
    private MainMenu mainMenu;

    public void LoadTitleScreen() {
        SceneManager.LoadScene(0);
        //mainMenu = FindObjectOfType<MainMenu>();
        //mainMenu.ReturnToMainScreenClicked();
        // destroy gameplayer objects here before returning to the menu
    }
}