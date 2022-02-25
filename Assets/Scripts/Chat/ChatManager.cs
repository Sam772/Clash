using System.Collections;
using System.Collections.Generic;
using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour {
    
    public string DisplayName;
    [SerializeField] private GameObject chatCanvas = null;
    [SerializeField] private TMP_Text chatHistory = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private Scrollbar scrollbar;

    private static event Action<string> OnMessage;

    public override void OnStartAuthority() {
        OnMessage += HandleNewMessage;
        CmdSetDisplayName(MenuUtil.GetNameFromPlayerPrefs());
        chatCanvas.SetActive(true);
    }

    [Command]
    private void CmdSetDisplayName(string displayName) {
        DisplayName = displayName;
    }

    [ClientCallback]
    private void OnDestroy() {
        if (!hasAuthority) { return; }
        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message) {
        chatHistory.text += message;
    }

    [Client]
    public void Send(string message) {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }
        if (string.IsNullOrWhiteSpace(message)) { return; }
        CmdSendMessage(message);
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message) {
        RpcHandleMessage($"{DisplayName}: {message}");
        AppendMessage(message);
    }

    [ClientRpc]
    private void RpcHandleMessage(string message) {
        OnMessage?.Invoke($"\n{message}");
    }

    internal void AppendMessage(string message)
    {
        StartCoroutine(AppendAndScroll(message));
    }

    IEnumerator AppendAndScroll(string message)
    {

        // it takes 2 frames for the UI to update ?!?!
        yield return null;
        yield return null;

        // slam the scrollbar down
        scrollbar.value = 0;
    }

}