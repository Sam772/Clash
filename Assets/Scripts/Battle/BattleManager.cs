using System.Collections;
using UnityEngine;
using Mirror;
public class BattleManager : NetworkBehaviour {
    public GameManager GMS;
    private bool battleStatus;
    public void Battle(GameObject initiator, GameObject recipient) {
        battleStatus = true;
        var initiatorUnit = initiator.GetComponent<Unit>();
        var recipientUnit = recipient.GetComponent<Unit>();
        int initiatorAtt = initiatorUnit.attackDamage;
        int recipientAtt = recipientUnit.attackDamage;
        if (initiatorUnit.attackRange == recipientUnit.attackRange) {
            recipientUnit.DealDamage(initiatorAtt);
            if (CheckIfDead(recipient)) {
                recipient.transform.parent = null;
                recipientUnit.UnitDie();
                battleStatus = false;
                GMS.CheckIfUnitsRemain(initiator, recipient);
                return;
            }
            initiatorUnit.DealDamage(recipientAtt);
            if (CheckIfDead(initiator)) {
                initiator.transform.parent = null;
                initiatorUnit.UnitDie();
                battleStatus = false;
                GMS.CheckIfUnitsRemain(initiator, recipient);
                return;
            }
        }
        else {
            recipientUnit.DealDamage(initiatorAtt);
            if (CheckIfDead(recipient)) {
                recipient.transform.parent = null;
                recipientUnit.UnitDie();
                battleStatus = false;
                GMS.CheckIfUnitsRemain(initiator, recipient);
                return;
            }
        }
        battleStatus = false;
    }

    public bool CheckIfDead(GameObject unitToCheck) {
        if (unitToCheck.GetComponent<Unit>().currentHealthPoints <= 0) {
            return true;
        }
        return false;
    }

    public void DestroyObject(GameObject unitToDestroy) {
        //Destroy(unitToDestroy);
        NetworkServer.Destroy(unitToDestroy);
    }

    public IEnumerator Attack(GameObject unit, GameObject enemy) {
        battleStatus = true;
        float elapsedTime = 0;
        Vector3 startingPos = unit.transform.position;
        Vector3 endingPos = enemy.transform.position;
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
            Battle(unit, enemy);
            yield return new WaitForEndOfFrame();
        }
        if (unit != null) {
           StartCoroutine(ReturnAfterAttack(unit, startingPos));
        }
    }

    public IEnumerator ReturnAfterAttack(GameObject unit, Vector3 endPoint) {
        float elapsedTime = 0;
        while (elapsedTime < .30f) {
            unit.transform.position = Vector3.Lerp(unit.transform.position, endPoint, (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        unit.GetComponent<Unit>().Wait();
    }

    public Vector3 GetDirection(GameObject unit, GameObject enemy) {
        Vector3 startingPos = unit.transform.position;
        Vector3 endingPos = enemy.transform.position;
        return (((endingPos - startingPos) / (endingPos - startingPos).magnitude)).normalized;
    }
}