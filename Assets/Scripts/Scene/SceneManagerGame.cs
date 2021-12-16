using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerGame : MonoBehaviour {
    public Canvas mainCanvas;
    public Canvas helpCanvas;
    public Canvas helpCanvas2;

    public void LoadTitleScreen() {
        SceneManager.LoadScene(0);
    }

    public void LoadHelpCanvas() {
        mainCanvas.enabled = false;
        helpCanvas.enabled = true;
    }

    public void LoadHelpCanvasBack() {
        helpCanvas2.enabled = false;
        helpCanvas.enabled = true;
    }

    public void LoadHelpCanvas2() {
        helpCanvas.enabled = false;
        helpCanvas2.enabled = true;
    }

    public void LoadMainCanvas() {
        mainCanvas.enabled = true;
        helpCanvas.enabled = false;
        helpCanvas2.enabled = false;
    }
}
