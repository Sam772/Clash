using System;
using UnityEngine;
using TMPro;

public class MapInfoManager : MonoBehaviour {

    public TextMeshProUGUI mapInfoText;
    public RectTransform mapInfoBox;
    public static Action<string, Vector2> OnMouseHover;
    public static Action OnMouseExit;

    void Start() {
        HideMapInfo();
    }

    private void OnEnable() {
        OnMouseHover += ShowMapInfo;
        OnMouseExit += HideMapInfo;
    }

    private void OnDisable() {
        OnMouseHover -= ShowMapInfo;
        OnMouseExit -= HideMapInfo;
    }

    private void ShowMapInfo(string info, Vector2 mousePosition) {
        mapInfoText.text = info;
        mapInfoBox.gameObject.SetActive(true);
    }

    private void HideMapInfo() {
        mapInfoText.text = default;
    }
}
