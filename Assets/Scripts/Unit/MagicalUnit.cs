using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MagicalUnit : GenericUnit {
    
    [Header("Magical Stats")]
    public int magic;

    [Command(requiresAuthority=false)]
    public override void CmdDealDamage(int battleMag, int battleRes) {
        int battleDamage = 0;
        if (battleMag - battleRes < 0) { 
            battleDamage = 0;
        } else {
            battleDamage = battleMag - battleRes;
        }
        currentHealth = currentHealth - battleDamage;
        RpcDealDamageClient(battleMag, battleRes);
        if (currentHealth <= 0)
        UnitDie();
        // send into checkifdead loop
        // check if units remain
    }

    [ClientRpc]
    public override void RpcDealDamageClient(int battleMagClient, int battleResClient) {
        if (!isServer) {
            int battleDamageClient = 0;
            if (battleMagClient - battleResClient < 0) {
                battleDamageClient = 0;
            } else {
                battleDamageClient = battleMagClient - battleResClient;
            }
            currentHealth = currentHealth - battleDamageClient; 
        }
        Debug.Log("damage dealt: " + battleMagClient);
        Debug.Log("hp of attacked unit: " + currentHealth);
        CmdUpdateHealthUI();
    }

    public override IEnumerator CombatEnd() {
        combatQueue.Enqueue(1);
        for (float f = 1f; f >= .05; f -= 0.01f) { yield return new WaitForEndOfFrame(); }
        combatQueue.Dequeue();
    }
}
