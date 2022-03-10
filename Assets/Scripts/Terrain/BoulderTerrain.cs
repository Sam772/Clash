using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoulderTerrain : GenericUnit {

    public override void CmdDealDamage(int battleStr, int battleDef) {
        int battleDamage = 0;
        if (battleStr - battleDef < 0) { 
            battleDamage = 0;
        } else {
            battleDamage = battleStr - battleDef;
        }
        currentHealth = currentHealth - battleDamage;
        UpdateHealthUI();
    }

    public override IEnumerator CombatEnd() {
        combatQueue.Enqueue(1);
        for (float f = 1f; f >= .05; f -= 0.01f) { yield return new WaitForEndOfFrame(); }
        combatQueue.Dequeue();
        for (float f = 1f; f >= .05; f -= 0.01f) { yield return new WaitForEndOfFrame(); }
    }
}