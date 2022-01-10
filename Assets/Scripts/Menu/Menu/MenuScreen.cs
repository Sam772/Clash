using UnityEngine;

public class MenuScreen : MonoBehaviour {
    protected MainMenu Menu { get; private set; }
        
    public void Setup(MainMenu menu) {
        Menu = menu;
    }
        
    public void Show() {
        gameObject.SetActive(true);
        OnShow();
    }

    public void Hide() {
        gameObject.SetActive(false);
        OnHide();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}