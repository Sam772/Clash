using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public abstract class GenericTerrain : NetworkBehaviour {

    public int terrainTeam;
    public int terrainX;
    public int terrainZ;
    public GameObject terrainTileBeingOccupied;
    public string terrainName;
    public int terrainMaxHealth;
    [SyncVar]
    public int terrainCurrentHealth;
    public Sprite terrainSprite;

    [Header("Terrain Healthbar")]
    public Canvas terrainHealthBarCanvas;
    public TMP_Text terrainHitPointsText;
    public Image terrainHealthBar;
    public GenericTileMap terrainMap;
    public GameObject terrainHolder2D;

    private void Awake() {
        terrainX = (int) transform.position.x;
        terrainZ = (int) transform.position.z;
        terrainCurrentHealth = terrainMaxHealth;
        terrainHitPointsText.SetText(terrainCurrentHealth.ToString());
        terrainHealthBar.color = Color.yellow;
    }

    public void LateUpdate() {
        terrainHealthBarCanvas.transform.forward = Camera.main.transform.forward;
        terrainHolder2D.transform.forward = Camera.main.transform.forward;
    }

    public void TerrainUpdateHealthUI() {
        terrainHealthBar.fillAmount = (float) terrainCurrentHealth / terrainMaxHealth;
        terrainHitPointsText.SetText(terrainCurrentHealth.ToString());
    }

    public void TerrainDie() {
        if (terrainHolder2D.activeSelf) {
            StartCoroutine(TerrainDestroy());
        }
    }

    // define something that will happen when terrain is destroyed
    public abstract IEnumerator TerrainDestroy();

    // maybe abstract GenericUnit script further and have this class extend it
}
