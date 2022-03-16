using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AccountScreen : MenuScreen {
    EventSystem system;
    public Selectable firstInput;
    public Button submitButton;
    [SerializeField] private SettingsScreen settingsScreen;

    public void Start() {
        system = EventSystem.current;
        firstInput.Select();

        if (!PlayerPrefs.HasKey("volume")) {
            PlayerPrefs.SetFloat("volume", 0.5f);
            settingsScreen.LoadVolume();
        } else {
            settingsScreen.LoadVolume();
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) {
            Selectable previous = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (previous != null) {
                previous.Select();
            }
        } else if (Input.GetKeyDown(KeyCode.Tab)) {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null) {
                next.Select();
            }
        } else if (Input.GetKeyDown(KeyCode.Return)) {
            submitButton.onClick.Invoke();
        }
    }
}