using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BattleManager : NetworkBehaviour {
    public GameManager GMS;
    private bool battleStatus;
    public void battle(GameObject initiator, GameObject recipient) {
        battleStatus = true;
        var initiatorUnit = initiator.GetComponent<Unit>();
        var recipientUnit = recipient.GetComponent<Unit>();
        int initiatorAtt = initiatorUnit.attackDamage;
        int recipientAtt = recipientUnit.attackDamage;
        if (initiatorUnit.attackRange == recipientUnit.attackRange) {
            recipientUnit.DealDamage(initiatorAtt);
            if (checkIfDead(recipient)) {
                recipient.transform.parent = null;
                recipientUnit.UnitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);
                return;
            }
            initiatorUnit.DealDamage(recipientAtt);
            if (checkIfDead(initiator)) {
                initiator.transform.parent = null;
                initiatorUnit.UnitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);
                return;
            }
        }
        else {
            recipientUnit.DealDamage(initiatorAtt);
            if (checkIfDead(recipient)) {
                recipient.transform.parent = null;
                recipientUnit.UnitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);
                return;
            }
        }
        battleStatus = false;
    }

    public bool checkIfDead(GameObject unitToCheck) {
        if (unitToCheck.GetComponent<Unit>().currentHealthPoints <= 0) {
            return true;
        }
        return false;
    }

    [ClientRpc]
    public void destroyObject(GameObject unitToDestroy) {
        //Destroy(unitToDestroy);
        //NetworkServer.Destroy(unitToDestroy);
        NetworkServer.UnSpawn(unitToDestroy);
    }

    public IEnumerator attack(GameObject unit, GameObject enemy) {
        battleStatus = true;
        float elapsedTime = 0;
        Vector3 startingPos = unit.transform.position;
        Vector3 endingPos = enemy.transform.position;
        unit.GetComponent<Unit>().SetWalkingAnimation();
        while (elapsedTime < .25f) {
            unit.transform.position = Vector3.Lerp(startingPos, startingPos+((((endingPos - startingPos) / (endingPos - startingPos).magnitude)).normalized*.5f), (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (battleStatus) {
            if(unit.GetComponent<Unit>().attackRange == enemy.GetComponent<Unit>().attackRange && enemy.GetComponent<Unit>().currentHealthPoints - unit.GetComponent<Unit>().attackDamage > 0) {
                StartCoroutine(unit.GetComponent<Unit>().DisplayDamageEnum(enemy.GetComponent<Unit>().attackDamage));
                StartCoroutine(enemy.GetComponent<Unit>().DisplayDamageEnum(unit.GetComponent<Unit>().attackDamage));
            }
            else {
                StartCoroutine(enemy.GetComponent<Unit>().DisplayDamageEnum(unit.GetComponent<Unit>().attackDamage));
            }
            battle(unit, enemy);
            yield return new WaitForEndOfFrame();
        }
        if (unit != null) {
           StartCoroutine(returnAfterAttack(unit, startingPos));
        }
    }

    public IEnumerator returnAfterAttack(GameObject unit, Vector3 endPoint) {
        float elapsedTime = 0;
        while (elapsedTime < .30f) {
            unit.transform.position = Vector3.Lerp(unit.transform.position, endPoint, (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        unit.GetComponent<Unit>().SetWaitIdleAnimation();
        unit.GetComponent<Unit>().Wait();
    }

    public Vector3 getDirection(GameObject unit, GameObject enemy) {
        Vector3 startingPos = unit.transform.position;
        Vector3 endingPos = enemy.transform.position;
        return (((endingPos - startingPos) / (endingPos - startingPos).magnitude)).normalized;
    }
}