using System.Collections;
using UnityEngine;
using Mirror;
public class BattleManager : NetworkBehaviour {
    public GameManager gameManager;
    private bool battleStatus;
    public void Battle(GameObject attacker, GameObject receiver) {
        battleStatus = true;
        var attackerUnit = attacker.GetComponent<Unit>();
        var receiverUnit = receiver.GetComponent<Unit>();
        int attackerAtk = attackerUnit.damage;
        int receiverAtk = receiverUnit.damage;
        if (attackerUnit.range == receiverUnit.range) {
            receiverUnit.CmdDealDamage(attackerAtk);
            if (CheckIfDead(receiver)) {
                receiverUnit.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;}
            attackerUnit.CmdDealDamage(receiverAtk);
            if (CheckIfDead(attacker)) {
                attackerUnit.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;}
        } else {
            receiverUnit.CmdDealDamage(attackerAtk);
            if (CheckIfDead(receiver)) {
                receiverUnit.UnitDie();
                battleStatus = false;
                gameManager.CmdUnitsRemainClient(attacker, receiver);
                return;}}
        battleStatus = false;
    }

    public bool CheckIfDead(GameObject unitToCheck) {
        if (unitToCheck.GetComponent<Unit>().currentHealth <= 0) {
            Debug.Log("enemy dead");
            return true;}
        // current health of enemy not being updated from client here
        Debug.Log("current health of enemy: " + unitToCheck.GetComponent<Unit>().currentHealth);
        Debug.Log("enemy still alive");
        return false;
    }

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
        unit.GetComponent<Unit>().Wait();
    }
}