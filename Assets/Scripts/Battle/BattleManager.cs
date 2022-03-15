using System.Collections;
using UnityEngine;
using Mirror;

public class BattleManager : NetworkBehaviour {
    public GameManager gameManager;
    private bool battleStatus;

    [Command(requiresAuthority=false)]
    public void CmdBattle(GameObject attacker, GameObject receiver) {
        battleStatus = true;
        
        // UNIT BATTLING UNIT
        if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<PhysicalUnit>()) {
            // Physical vs Physical
            RpcBattlePhyVsPhy(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<PhysicalUnit>()) {
            // Magical vs Physical
            RpcBattleMagVsPhy(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<MagicalUnit>()) {
            // Physical vs Magical
            RpcBattlePhyVsMag(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<MagicalUnit>()) {
            // Magical vs Magical
            RpcBattleMagVsMag(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        }

        // UNIT BATTLING TERRAIN
        else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<LogTerrain>()) {
            // Physical vs Log
            RpcBattlePhyVsLog(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<LogTerrain>()) {
            // Magical vs Log
            RpcBattleMagVsLog(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<BoulderTerrain>()) {
            // Physical vs Boulder
            RpcBattlePhyVsBoulder(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<BoulderTerrain>()) {
            // Magical vs Boulder
            RpcBattleMagVsBoulder(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<HealingPotTerrain>()) {
            // Physical vs Healing Pot
            RpcBattlePhyVsPot(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<HealingPotTerrain>()) {
            // Magical vs Healing Pot
            RpcBattleMagVsPot(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<StoneCrackedTerrain>()) {
            // Physical vs Stone Cracked Wall
            RpcBattlePhyVsStone(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<StoneCrackedTerrain>()) {
            // Magical vs Stone Cracked Wall
            RpcBattleMagVsStone(attacker, receiver);
            gameManager.CmdUnitsRemainClient(attacker, receiver);
        }
        battleStatus = false;
    }

    public bool CheckIfDead(GameObject unitToCheck) {
        if (unitToCheck.GetComponent<GenericUnit>().currentHealth <= 0) {
            return true;
        }
        return false;
    }

    [ClientRpc]
    public void RpcBattlePhyVsPhy(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
        int attackerStr = attackerUnitPhysical.strength;
        int attackerPhysicalDef = attackerUnitPhysical.defence;

        // get relevant object and stats for receiver
        var receiverUnitPhysical = receiver.GetComponent<PhysicalUnit>();
        int receiverStr = receiverUnitPhysical.strength;
        int receiverPhysicalDef = receiverUnitPhysical.defence;

        // check range then calculate damage
        if (attackerUnitPhysical.range == receiverUnitPhysical.range) {
            receiverUnitPhysical.CmdDealDamage(attackerStr, receiverPhysicalDef);
            if (CheckIfDead(receiver)) {
                receiverUnitPhysical.UnitDie();
                battleStatus = false;
                return;
            }
            // counter if receiver survived
            attackerUnitPhysical.CmdDealDamage(receiverStr, attackerPhysicalDef);
            if (CheckIfDead(attacker)) {
                attackerUnitPhysical.UnitDie();
                battleStatus = false;
                return;
            }
        } else {
            receiverUnitPhysical.CmdDealDamage(attackerStr, receiverPhysicalDef);
            if (CheckIfDead(receiver)) {
                receiverUnitPhysical.UnitDie();
                battleStatus = false;
                return;
            }
        }
    }

    [ClientRpc]
    public void RpcBattleMagVsPhy(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
        int attackerMag = attackerUnitMagical.magic;
        int attackerMagicalDef = attackerUnitMagical.defence;

        // get relevant object and stats for receiver
        var receiverUnitPhysical = receiver.GetComponent<PhysicalUnit>();
        int receiverStr = receiverUnitPhysical.strength;
        int receiverPhysicalRes = receiverUnitPhysical.resistance;

        // check range then calculate damage
        if (attackerUnitMagical.range == receiverUnitPhysical.range) {
            receiverUnitPhysical.CmdDealDamage(attackerMag, receiverPhysicalRes);
            if (CheckIfDead(receiver)) {
                receiverUnitPhysical.UnitDie();
                battleStatus = false;
                return;
            }
            // counter if survive
            attackerUnitMagical.CmdDealDamage(receiverStr, attackerMagicalDef);
            if (CheckIfDead(attacker)) {
                attackerUnitMagical.UnitDie();
                battleStatus = false;
                return;
            }
        } else {
            receiverUnitPhysical.CmdDealDamage(attackerMag, receiverPhysicalRes);
            if (CheckIfDead(receiver)) {
                receiverUnitPhysical.UnitDie();
                battleStatus = false;
                return;
            }
        }
    }

    [ClientRpc]
    public void RpcBattlePhyVsMag(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
        int attackerStr = attackerUnitPhysical.strength;
        int attackerPhysicalRes = attackerUnitPhysical.resistance;

        // get relevant object and stats for receiver
        var receiverUnitMagical = receiver.GetComponent<MagicalUnit>();
        int receiverMag = receiverUnitMagical.magic;
        int receiverMagicalDef = receiverUnitMagical.defence;

        // check range then calculate damage
        if (attackerUnitPhysical.range == receiverUnitMagical.range) {
            receiverUnitMagical.CmdDealDamage(attackerStr, receiverMagicalDef);
            if (CheckIfDead(receiver)) {
                receiverUnitMagical.UnitDie();
                battleStatus = false;
                return;
            }
            // counter if survive
            attackerUnitPhysical.CmdDealDamage(receiverMag, attackerPhysicalRes);
            if (CheckIfDead(attacker)) {
                attackerUnitPhysical.UnitDie();
                battleStatus = false;
                return;
            }
        } else {
            receiverUnitMagical.CmdDealDamage(attackerStr, receiverMagicalDef);
            if (CheckIfDead(receiver)) {
                receiverUnitMagical.UnitDie();
                battleStatus = false;
                return;
            }
        }
    }

    [ClientRpc]
    public void RpcBattleMagVsMag(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
        int attackerMag = attackerUnitMagical.magic;
        int attackerMagicalRes = attackerUnitMagical.resistance;

        // get relevant object and stats for receiver
        var receiverUnitMagical = receiver.GetComponent<MagicalUnit>();
        int receiverMag = receiverUnitMagical.magic;
        int receiverMagicalRes = receiverUnitMagical.resistance;

        // check range then calculate damage
        if (attackerUnitMagical.range == receiverUnitMagical.range) {
            receiverUnitMagical.CmdDealDamage(attackerMag, receiverMagicalRes);
            if (CheckIfDead(receiver)) {
                receiverUnitMagical.UnitDie();
                battleStatus = false;
                return;
            }
            // counter if survive
            attackerUnitMagical.CmdDealDamage(receiverMag, attackerMagicalRes);
            if (CheckIfDead(attacker)) {
                attackerUnitMagical.UnitDie();
                battleStatus = false;
                return;
            }
        } else {
            receiverUnitMagical.CmdDealDamage(attackerMag, receiverMagicalRes);
            if (CheckIfDead(receiver)) {
                receiverUnitMagical.UnitDie();
                battleStatus = false;
                return;
            }
        }
    }

    [ClientRpc]
    public void RpcBattlePhyVsLog(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
        int attackerStr = attackerUnitPhysical.strength;
        
        // get relevant object and stats for receiver
        var receiverTerrainLog = receiver.GetComponent<LogTerrain>();
        int receiverDef = receiverTerrainLog.defence;

        // calculate damage
        receiverTerrainLog.CmdDealDamage(attackerStr, receiverDef);
        if (CheckIfDead(receiver)) {
            receiverTerrainLog.UnitDie();
            battleStatus = false;
            return;
        }
    }

    [ClientRpc]
    public void RpcBattleMagVsLog(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
        int attackerMag = attackerUnitMagical.magic;

        // get relevant object and stats for receiver
        var receiverTerrainLog = receiver.GetComponent<LogTerrain>();
        int receiverRes = receiverTerrainLog.resistance;

        // calculate damage
        receiverTerrainLog.CmdDealDamage(attackerMag, receiverRes);
        if (CheckIfDead(receiver)) {
            receiverTerrainLog.UnitDie();
            battleStatus = false;
            return;
        }
    }

    [ClientRpc]
    public void RpcBattlePhyVsBoulder(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
        int attackerStr = attackerUnitPhysical.strength;

        // get relevant object and stats for receiver
        var receiverTerrainBoulder = receiver.GetComponent<BoulderTerrain>();
        int receiverDef = receiverTerrainBoulder.defence;

        // calculate damage
        receiverTerrainBoulder.CmdDealDamage(attackerStr, receiverDef);
        if (CheckIfDead(receiver)) {
            receiverTerrainBoulder.UnitDie();
            battleStatus = false;

            GenericUnit[] unitsList = FindObjectsOfType<GenericUnit>();

            foreach (GenericUnit unitPresent in unitsList) {
                // if there is a unit present in a radius around the boulder deal damage to it as long as it is not the unit that destroyed the boulder
                if ((unitPresent.x == receiverTerrainBoulder.x) && (unitPresent.y == (receiverTerrainBoulder.y + 1)) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == receiverTerrainBoulder.x) && (unitPresent.y == receiverTerrainBoulder.y - 1) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x + 1)) && (unitPresent.y == receiverTerrainBoulder.y) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x - 1)) && (unitPresent.y == receiverTerrainBoulder.y) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x + 1)) && (unitPresent.y == (receiverTerrainBoulder.y - 1)) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x + 1)) && (unitPresent.y == (receiverTerrainBoulder.y + 1)) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x - 1)) && ( unitPresent.y == (receiverTerrainBoulder.y - 1)) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x - 1)) && (unitPresent.y == (receiverTerrainBoulder.y + 1)) && unitPresent != attackerUnitPhysical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                }
            }
            return;
        }
    }

    [ClientRpc]
    public void RpcBattleMagVsBoulder(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
        int attackerMag = attackerUnitMagical.magic;

        // get relevant object and stats for receiver
        var receiverTerrainBoulder = receiver.GetComponent<BoulderTerrain>();
        int receiverRes = receiverTerrainBoulder.resistance;

        // calculate damage
        receiverTerrainBoulder.CmdDealDamage(attackerMag, receiverRes);
        if (CheckIfDead(receiver)) {
            receiverTerrainBoulder.UnitDie();
            battleStatus = false;

            GenericUnit[] unitsList = FindObjectsOfType<GenericUnit>();

            foreach (GenericUnit unitPresent in unitsList) {
                // if there is a unit present in a radius around the boulder deal damage to it as long as it is not the unit that destroyed the boulder
                if ((unitPresent.x == receiverTerrainBoulder.x) && (unitPresent.y == (receiverTerrainBoulder.y + 1)) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == receiverTerrainBoulder.x) && (unitPresent.y == receiverTerrainBoulder.y - 1) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x + 1)) && (unitPresent.y == receiverTerrainBoulder.y) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x - 1)) && (unitPresent.y == receiverTerrainBoulder.y) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x + 1)) && (unitPresent.y == (receiverTerrainBoulder.y - 1)) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x + 1)) && (unitPresent.y == (receiverTerrainBoulder.y + 1)) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x - 1)) && ( unitPresent.y == (receiverTerrainBoulder.y - 1)) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                } else if ((unitPresent.x == (receiverTerrainBoulder.x - 1)) && (unitPresent.y == (receiverTerrainBoulder.y + 1)) && unitPresent != attackerUnitMagical) {
                    unitPresent.currentHealth -= 2;
                    unitPresent.UpdateHealthUI();
                    if (CheckIfDead(unitPresent.gameObject)) {
                        unitPresent.UnitDie();
                    }
                }
            }
            return;
        }
    }

    [ClientRpc]
    public void RpcBattlePhyVsPot(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
        int attackerStr = attackerUnitPhysical.strength;

        // get relevant object and stats for receiver
        var receiverTerrainHealingPot = receiver.GetComponent<HealingPotTerrain>();
        int receiverDef = receiverTerrainHealingPot.defence;

        // calculate damage
        receiverTerrainHealingPot.CmdDealDamage(attackerStr, receiverDef);
        if (CheckIfDead(receiver)) {
            receiverTerrainHealingPot.UnitDie();
            battleStatus = false;

            // when the healing pot is destroyed heal the attacker unit by 2
            attackerUnitPhysical.currentHealth += 2;

            // make sure the health isn't increased beyond the max health of the attacker
            if (attackerUnitPhysical.currentHealth >= attackerUnitPhysical.maxHealth) {
                attackerUnitPhysical.currentHealth = attackerUnitPhysical.maxHealth;
            }

            // update the health
            attackerUnitPhysical.UpdateHealthUI();
            return;
        }
    }

    [ClientRpc]
    public void RpcBattleMagVsPot(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
        int attackerMag = attackerUnitMagical.magic;

        // get relevant object and stats for receiver
        var receiverTerrainHealingPot = receiver.GetComponent<HealingPotTerrain>();
        int receiverRes = receiverTerrainHealingPot.resistance;

        // calculate damage
        receiverTerrainHealingPot.CmdDealDamage(attackerMag, receiverRes);
        if (CheckIfDead(receiver)) {
            receiverTerrainHealingPot.UnitDie();
            battleStatus = false;

            // when the healing pot is destroyed heal the attacker unit by 2
            attackerUnitMagical.currentHealth += 2;

            // make sure the health isn't increased beyond the max health of the attacker
            if (attackerUnitMagical.currentHealth >= attackerUnitMagical.maxHealth) {
                attackerUnitMagical.currentHealth = attackerUnitMagical.maxHealth;
            }

            // update the health
            attackerUnitMagical.UpdateHealthUI();
            return;
        }
    }

    [ClientRpc]
    public void RpcBattlePhyVsStone(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
        int attackerStr = attackerUnitPhysical.strength;

        // get relevant object and stats for receiver
        var receiverTerrainStoneCracked = receiver.GetComponent<StoneCrackedTerrain>();
        int receiverDef = receiverTerrainStoneCracked.defence;

        // calculate damage
        receiverTerrainStoneCracked.CmdDealDamage(attackerStr, receiverDef);
        if (CheckIfDead(receiver)) {
            receiverTerrainStoneCracked.UnitDie();
            battleStatus = false;
            return;
        }
    }

    [ClientRpc]
    public void RpcBattleMagVsStone(GameObject attacker, GameObject receiver) {
        // get relevant object and stats for attacker
        var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
        int attackerMag = attackerUnitMagical.magic;

        // get relevant object and stats for receiver
        var receiverTerrainStoneCracked = receiver.GetComponent<StoneCrackedTerrain>();
        int receiverRes = receiverTerrainStoneCracked.resistance;

        // calculate damage
        receiverTerrainStoneCracked.CmdDealDamage(attackerMag, receiverRes);
        if (CheckIfDead(receiver)) {
            receiverTerrainStoneCracked.UnitDie();
            battleStatus = false;
            return;
        }
    }

    public IEnumerator Attack(GameObject unit, GameObject enemy) {
        battleStatus = true;
        float elapsedTime = 0;
        Vector3 startPos = unit.transform.position;
        Vector3 finishPos = enemy.transform.position;
        while (elapsedTime < .25f) {
            unit.transform.position = Vector3.Lerp(startPos, startPos + ((((finishPos - startPos) / (finishPos - startPos).magnitude)).normalized*.5f), (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (battleStatus) {
            CmdBattle(unit, enemy);
            yield return new WaitForSeconds(0.05f);
            battleStatus = false;
        }
        if (unit != null) { 
            StartCoroutine(ReturnAfterAttack(unit, startPos)); 
        }
    }

    public IEnumerator ReturnAfterAttack(GameObject unit, Vector3 endPoint) {
        float elapsedTime = 0;
        while (elapsedTime < .30f) {
            unit.transform.position = Vector3.Lerp(unit.transform.position, endPoint, (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        unit.GetComponent<GenericUnit>().Wait();
    }
}