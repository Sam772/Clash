using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {
    #pragma warning disable 649
        [Header("HUD")]
        [SerializeField] private PlayerInfo player1;
        [SerializeField] private PlayerInfo player2;
        //[SerializeField] private CurrencyUI currency;
        //[SerializeField] private ControlPanel controlPanel;
        //[SerializeField] private TurnIndicator turnIndicator;
        //[SerializeField] private Tooltip tooltip;
        //[SerializeField] private GameOverUI gameOverUI;
        
        [Header("Cursors")]
        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D attackCursor;
        [SerializeField] private Texture2D moveCursor;
        [SerializeField] private Texture2D invalidCursor;

        [Header("World")]
        //[SerializeField] private HealthBars healthBars;
        //[SerializeField] private FloatingUI floatingUI;
#pragma warning restore 649

        private NewNetworkGamePlayer player;
        private bool isCursorInUI;
        //private HexCursor gameCursor;
        //private HexCursor uiCursor;
}
