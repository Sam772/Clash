using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MagicalUnit : GenericUnit {
    
    [Header("Magical Stats")]
    public int magic;

    public override void CmdDealDamage(int battleMag, int battleRes) {
        int battleDamage = 0;
        if (battleMag - battleRes < 0) { 
            battleDamage = 0;
        } else {
            battleDamage = battleMag - battleRes;
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
