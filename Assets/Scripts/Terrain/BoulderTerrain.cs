using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoulderTerrain : GenericUnit {

    // for this example we treat this terrain as an object that can do damage

    [Command(requiresAuthority=false)]
    public override void CmdDealDamage(int battleStr, int battleDef) {
        int battleDamage = 0;
        if (battleStr - battleDef < 0) { 
            battleDamage = 0;
        } else {
            battleDamage = battleStr - battleDef;
        }
        currentHealth = currentHealth - battleDamage;
        RpcDealDamageClient(battleStr, battleDef);
        if (currentHealth <= 0)
        UnitDie();
        // send into checkifdead loop
        // check if units remain
    }

    [ClientRpc]
    public override void RpcDealDamageClient(int battleStrClient, int battleDefClient) {
        if (!isServer) {
            int battleDamageClient = 0;
            if (battleStrClient - battleDefClient < 0) {
                battleDamageClient = 0;
            } else {
                battleDamageClient = battleStrClient - battleDefClient;
            }
            currentHealth = currentHealth - battleDamageClient; 
        }
        Debug.Log("damage dealt: " + battleStrClient);
        Debug.Log("hp of attacked unit: " + currentHealth);
        CmdUpdateHealthUI();
    }

    public override IEnumerator CombatEnd() {
        combatQueue.Enqueue(1);
        for (float f = 1f; f >= .05; f -= 0.01f) { yield return new WaitForEndOfFrame(); }
        combatQueue.Dequeue();
        // here we will do something that does damage back when the object is destroyed
        // if unitontile currenthealth -= 1?
    }
}