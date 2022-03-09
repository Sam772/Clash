using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PhysicalUnit : GenericUnit {
    
    [Header("Physical Stats")]
    public int strength;

    public override void CmdDealDamage(int battleStr, int battleDef) {
        int battleDamage = 0;
        if (battleStr - battleDef < 0) { 
            battleDamage = 0;
        } else {
            battleDamage = battleStr - battleDef;
        }
        currentHealth = currentHealth - battleDamage;
        UpdateHealthUI();
        Debug.Log("Hi");
        // RpcDealDamageClient(battleStr, battleDef);
        // if (currentHealth <= 0)
        // UnitDie();
    }

    
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
    }
}
