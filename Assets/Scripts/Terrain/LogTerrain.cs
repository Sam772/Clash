using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LogTerrain : GenericUnit {
    
    // for this example we treat this terrain as a barrier
    // public override IEnumerator TerrainDestroy() {
    //     yield return new WaitForEndOfFrame();
    // }

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
        // uncomment this block
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
        UpdateHealthUI();
    }
}
