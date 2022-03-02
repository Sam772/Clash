using System.Collections;
using UnityEngine;
using Mirror;

public class BattleManager : NetworkBehaviour {
    public GameManager gameManager;
    private bool battleStatus;
    public GenericUnit unit;
    public void Battle(GameObject attacker, GameObject receiver) {
        battleStatus = true;
        
        // UNIT BATTLING UNIT
        if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<PhysicalUnit>()) {
            // physical initiator
            var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
            // physical initiator strength
            int attackerStr = attackerUnitPhysical.strength;
            // physical initiator defence
            int attackerPhysicalDef = attackerUnitPhysical.defence;
            // physical receiver
            var receiverUnitPhysical = receiver.GetComponent<PhysicalUnit>();
            // physical receiver strength
            int receiverStr = receiverUnitPhysical.strength;
            // physical receiver defence
            int receiverPhysicalDef = receiverUnitPhysical.defence;

            // if the attacker and receiver are both physical units
            if (attackerUnitPhysical.range == receiverUnitPhysical.range) {
                receiverUnitPhysical.CmdDealDamage(attackerStr, receiverPhysicalDef);
                if (CheckIfDead(receiver)) {
                    receiverUnitPhysical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
                // counter if survive
                attackerUnitPhysical.CmdDealDamage(receiverStr, attackerPhysicalDef);
                if (CheckIfDead(attacker)) {
                    attackerUnitPhysical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            } else {
            receiverUnitPhysical.CmdDealDamage(attackerStr, receiverPhysicalDef);
                if (CheckIfDead(receiver)) {
                    receiverUnitPhysical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            }
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<PhysicalUnit>()) {
            // magical initiator
            var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
            // magical initiator magic
            int attackerMag = attackerUnitMagical.magic;
            // magical initiator defence
            int attackerMagicalDef = attackerUnitMagical.defence;
            // physical receiver
            var receiverUnitPhysical = receiver.GetComponent<PhysicalUnit>();
            // physical receiver strength
            int receiverStr = receiverUnitPhysical.strength;
            // physical receiver resistance
            int receiverPhysicalRes = receiverUnitPhysical.resistance;

            // if the attacker is magical and the receiver is physical
            if (attackerUnitMagical.range == receiverUnitPhysical.range) {
                receiverUnitPhysical.CmdDealDamage(attackerMag, receiverPhysicalRes);
                if (CheckIfDead(receiver)) {
                    receiverUnitPhysical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
                // counter if survive
                attackerUnitMagical.CmdDealDamage(receiverStr, attackerMagicalDef);
                if (CheckIfDead(attacker)) {
                    attackerUnitMagical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            } else {
                receiverUnitPhysical.CmdDealDamage(attackerMag, receiverPhysicalRes);
                if (CheckIfDead(receiver)) {
                    receiverUnitPhysical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            }
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<MagicalUnit>()) {
            // physical initiator
            var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
            // physical initiator strength
            int attackerStr = attackerUnitPhysical.strength;
            // physical initiator resistance
            int attackerPhysicalRes = attackerUnitPhysical.resistance;
            // magical receiver
            var receiverUnitMagical = receiver.GetComponent<MagicalUnit>();
            // magical receiver magic
            int receiverMag = receiverUnitMagical.magic;
            // magical receiver def
            int receiverMagicalDef = receiverUnitMagical.defence;

            // if the attacker is physical and the receiver is magical
            if (attackerUnitPhysical.range == receiverUnitMagical.range) {
                receiverUnitMagical.CmdDealDamage(attackerStr, receiverMagicalDef);
                if (CheckIfDead(receiver)) {
                    receiverUnitMagical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
                // counter if survive
                attackerUnitPhysical.CmdDealDamage(receiverMag, attackerPhysicalRes);
                if (CheckIfDead(attacker)) {
                    attackerUnitPhysical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            } else {
                receiverUnitMagical.CmdDealDamage(attackerStr, receiverMagicalDef);
                if (CheckIfDead(receiver)) {
                    receiverUnitMagical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            }
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<MagicalUnit>()) {
            // magical initiator
            var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
            // magical initiator magic
            int attackerMag = attackerUnitMagical.magic;
            // magical initiator resistance
            int attackerMagicalRes = attackerUnitMagical.resistance;
            // magical receiver
            var receiverUnitMagical = receiver.GetComponent<MagicalUnit>();
            // magical receiver magic
            int receiverMag = receiverUnitMagical.magic;
            // magical receiver resistance
            int receiverMagicalRes = receiverUnitMagical.resistance;

            // if the attacker and receiver are both physical units
            if (attackerUnitMagical.range == receiverUnitMagical.range) {
                receiverUnitMagical.CmdDealDamage(attackerMag, receiverMagicalRes);
                if (CheckIfDead(receiver)) {
                    receiverUnitMagical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
                // counter if survive
                attackerUnitMagical.CmdDealDamage(receiverMag, attackerMagicalRes);
                if (CheckIfDead(attacker)) {
                    attackerUnitMagical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            } else {
                receiverUnitMagical.CmdDealDamage(attackerMag, receiverMagicalRes);
                if (CheckIfDead(receiver)) {
                    receiverUnitMagical.UnitDie();
                    battleStatus = false;
                    gameManager.CmdUnitsRemainClient(attacker, receiver);
                    return;
                }
            }
            // UNIT BATTLING TERRAIN
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<LogTerrain>()) {
            // physical initiator
            var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
            // physical initiator strength
            int attackerStr = attackerUnitPhysical.strength;
            // log terrain receiver
            var receiverTerrainLog = receiver.GetComponent<LogTerrain>();
            // log terrain receiver defence
            int receiverDef = receiverTerrainLog.defence;

            // physical unit attacking log terrain
            receiverTerrainLog.CmdDealDamage(attackerStr, receiverDef);
            if (CheckIfDead(receiver)) {
                receiverTerrainLog.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<LogTerrain>()) {
            // magical initiator
            var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
            // magical initiator magic
            int attackerMag = attackerUnitMagical.magic;
            // log terrain receiver
            var receiverTerrainLog = receiver.GetComponent<LogTerrain>();
            // log terrain receiver resistance
            int receiverRes = receiverTerrainLog.resistance;

            // magical unit attacking log terrain
            receiverTerrainLog.CmdDealDamage(attackerMag, receiverRes);
            if (CheckIfDead(receiver)) {
                receiverTerrainLog.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<BoulderTerrain>()) {
            // physical initiator
            var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
            // physical initiator strength
            int attackerStr = attackerUnitPhysical.strength;
            // boulder terrain receiver
            var receiverTerrainBoulder = receiver.GetComponent<BoulderTerrain>();
            // boulder terrain receiver defence
            int receiverDef = receiverTerrainBoulder.defence;

            // physical unit attacking boulder terrain
            receiverTerrainBoulder.CmdDealDamage(attackerStr, receiverDef);
            if (CheckIfDead(receiver)) {
                receiverTerrainBoulder.UnitDie();
                battleStatus = false;

                // first we check which side of the boulder it was destroyed from
                Debug.Log("Boulder was destroyed from tile [" + attackerUnitPhysical.x + ", " + attackerUnitPhysical.y + "]");

                // if there is a unit present on the opposite side of the boulder subtract hp from that unit
                if (unit.x + unit.y == (attackerUnitPhysical.x) + (attackerUnitPhysical.y + 2)) {
                    unit.currentHealth -= 2;
                    // gamemanager.unitbeingdisplayed
                }

                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<BoulderTerrain>()) {
            // magical initiator
            var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
            // magical initiator magic
            int attackerMag = attackerUnitMagical.magic;
            // boulder terrain receiver
            var receiverTerrainBoulder = receiver.GetComponent<BoulderTerrain>();
            // boulder terrain receiver resistance
            int receiverRes = receiverTerrainBoulder.resistance;

            // magical unit attacking boulder terrain
            receiverTerrainBoulder.CmdDealDamage(attackerMag, receiverRes);
            if (CheckIfDead(receiver)) {
                receiverTerrainBoulder.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<HealingPotTerrain>()) {
            // physical initiator
            var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
            // physical initiator strength
            int attackerStr = attackerUnitPhysical.strength;
            // healing pot terrain receiver
            var receiverTerrainHealingPot = receiver.GetComponent<HealingPotTerrain>();
            // healing pot terrain receiver defence
            int receiverDef = receiverTerrainHealingPot.defence;

            // physical unit attacking healing pot terrain
            receiverTerrainHealingPot.CmdDealDamage(attackerStr, receiverDef);
            if (CheckIfDead(receiver)) {
                receiverTerrainHealingPot.UnitDie();
                battleStatus = false;

                // when the healing pot is destroyed heal the attacker unit by 2
                attackerUnitPhysical.currentHealth += 2;

                // make sure the health isn't increased beyond the max health
                if (attackerUnitPhysical.currentHealth >= attackerUnitPhysical.maxHealth) {
                    attackerUnitPhysical.currentHealth = attackerUnitPhysical.maxHealth;
                }

                // update the health
                attackerUnitPhysical.CmdUpdateHealthUI();

                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<HealingPotTerrain>()) {
            // magical initiator
            var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
            // magical initiator magic
            int attackerMag = attackerUnitMagical.magic;
            // healing pot terrain receiver
            var receiverTerrainHealingPot = receiver.GetComponent<HealingPotTerrain>();
            // healing pot terrain receiver resistance
            int receiverRes = receiverTerrainHealingPot.resistance;

            // magical unit attacking healing pot terrain
            receiverTerrainHealingPot.CmdDealDamage(attackerMag, receiverRes);
            if (CheckIfDead(receiver)) {
                receiverTerrainHealingPot.UnitDie();
                battleStatus = false;

                // when the healing pot is destroyed heal the attacker unit by 2
                attackerUnitMagical.currentHealth += 2;

                // make sure the health isn't increased beyond the max health
                if (attackerUnitMagical.currentHealth >= attackerUnitMagical.maxHealth) {
                    attackerUnitMagical.currentHealth = attackerUnitMagical.maxHealth;
                }
                // update the health
                attackerUnitMagical.CmdUpdateHealthUI();

                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        } else if (attacker.GetComponent<PhysicalUnit>() && receiver.GetComponent<StoneCrackedTerrain>()) {
            // physical initiator
            var attackerUnitPhysical = attacker.GetComponent<PhysicalUnit>();
            // physical initiator strength
            int attackerStr = attackerUnitPhysical.strength;
            // stone cracked terrain receiver
            var receiverTerrainStoneCracked = receiver.GetComponent<StoneCrackedTerrain>();
            // stone cracked terrain receiver defence
            int receiverDef = receiverTerrainStoneCracked.defence;

            // physical unit attacking stone cracked terrain
            receiverTerrainStoneCracked.CmdDealDamage(attackerStr, receiverDef);
            if (CheckIfDead(receiver)) {
                receiverTerrainStoneCracked.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        } else if (attacker.GetComponent<MagicalUnit>() && receiver.GetComponent<StoneCrackedTerrain>()) {
            // magical initiator
            var attackerUnitMagical = attacker.GetComponent<MagicalUnit>();
            // magical initiator magic
            int attackerMag = attackerUnitMagical.magic;
            // stone cracked terrain receiver
            var receiverTerrainStoneCracked = receiver.GetComponent<StoneCrackedTerrain>();
            // stone cracked terrain receiver resistance
            int receiverRes = receiverTerrainStoneCracked.resistance;

            // magical unit attacking log terrain
            receiverTerrainStoneCracked.CmdDealDamage(attackerMag, receiverRes);
            if (CheckIfDead(receiver)) {
                receiverTerrainStoneCracked.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;
            }
        }
        battleStatus = false;
    }

    public bool CheckIfDead(GameObject unitToCheck) {
        if (unitToCheck.GetComponent<GenericUnit>().currentHealth <= 0) {
            Debug.Log("enemy dead");
            return true;}
        // current health of enemy not being updated from client here
        Debug.Log("current health of enemy: " + unitToCheck.GetComponent<GenericUnit>().currentHealth);
        Debug.Log("enemy still alive");
        return false;
    }

    // [Command(requiresAuthority=false)]
    // public void CmdBattle(GameObject attacker, GameObject receiver) {
    //     RpcBattle(attacker, receiver);
    // }

    // [Client]
    // public void RpcBattle(GameObject attacker, GameObject receiver) {
    //     Battle(attacker, receiver);
    // }

    public IEnumerator Attack(GameObject unit, GameObject enemy) {
        battleStatus = true;
        float elapsedTime = 0;
        Vector3 startPos = unit.transform.position;
        Vector3 finishPos = enemy.transform.position;
        while (elapsedTime < .25f) {
            unit.transform.position = Vector3.Lerp(startPos, startPos + ((((finishPos - startPos) / (finishPos - startPos).magnitude)).normalized*.5f), (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();}
        while (battleStatus) {
            //CmdBattle(unit, enemy);
            Battle(unit, enemy);
            yield return new WaitForEndOfFrame();}
        if (unit != null) { StartCoroutine(ReturnAfterAttack(unit, startPos)); }
    }

    public IEnumerator ReturnAfterAttack(GameObject unit, Vector3 endPoint) {
        float elapsedTime = 0;
        while (elapsedTime < .30f) {
            unit.transform.position = Vector3.Lerp(unit.transform.position, endPoint, (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();}
        unit.GetComponent<GenericUnit>().Wait();
    }
}