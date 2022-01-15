using UnityEngine;
using UnityEngine.EventSystems;

public class MapInfo : MapSelectionScreen, IPointerEnterHandler, IPointerExitHandler {
    public string mapInfoToShow;

    public void OnPointerEnter(PointerEventData eventData) {
        ShowMapInfo();
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        MapInfoManager.OnMouseExit();
    }

    private void ShowMapInfo() {
        MapInfoManager.OnMouseHover(mapInfoToShow, Input.mousePosition);
    }
}
