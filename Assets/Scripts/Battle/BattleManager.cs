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
                GMS.UnitsRemainClient(initiator, recipient);
                return;
            }
            initiatorUnit.DealDamage(recipientAtt);
            if (CheckIfDead(initiator)) {
                initiator.transform.parent = null;
                initiatorUnit.UnitDie();
                battleStatus = false;
                GMS.UnitsRemainClient(initiator, recipient);
                return;
            }
        }
        else {
            recipientUnit.DealDamage(initiatorAtt);
            if (CheckIfDead(recipient)) {
                recipient.transform.parent = null;
                recipientUnit.UnitDie();
                battleStatus = false;
                GMS.UnitsRemainClient(initiator, recipient);
                return;
            }
        }
        battleStatus = false;
    }

    public bool CheckIfDead(GameObject unitToCheck) {
        if (unitToCheck.GetComponent<Unit>().currentHealthPoints <= 0) {
            Debug.Log("enemy dead");
            return true;
        }
        // current health of enemy not being updated from client here
        Debug.Log("current health of enemy: " + unitToCheck.GetComponent<Unit>().currentHealthPoints);
        Debug.Log("enemy still alive");
        return false;
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
}