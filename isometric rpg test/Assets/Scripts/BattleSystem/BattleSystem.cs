using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleSystemHUD playerHUD;

    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleSystemHUD enemyHUD;

    public event EventHandler OnBattleOver;

    public void StartBattle(Unit attacker, Unit defender)
    {
        playerUnit.unit = attacker;
        playerUnit.isPlayerUnit = true;

        enemyUnit.unit = defender;

        SetUpBattle();
    }

    public void SetUpBattle()
    {
        playerUnit.Setup(enemyUnit);
        playerHUD.SetData(playerUnit.unit);

        enemyUnit.Setup(playerUnit);
        enemyHUD.SetData(enemyUnit.unit);

        StartCoroutine(PerformPlayerMove());
    }

    private IEnumerator PerformPlayerMove()
    {
        var damageDetails = playerUnit.unit.AttackHandler(enemyUnit.unit, false);

        yield return playerUnit.PlayAttackAnimation(damageDetails.IsCrit);
        yield return new WaitForSeconds(0.5f);

        enemyUnit.PlayHitAnimation();
        
        yield return enemyHUD.UpdateHP();

        yield return playerUnit.PlayBackupAnimation(damageDetails.IsCrit);
        yield return new WaitForSeconds(0.5f);

        if (damageDetails.IsDead)
        {
            enemyUnit.PlayDeathAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(damageDetails, EventArgs.Empty);
        }
        else
        {
            StartCoroutine(EnemyMove());          
        }
    }

    private IEnumerator EnemyMove()
    {
        var damageDetails = enemyUnit.unit.AttackHandler(playerUnit.unit, true);

        yield return enemyUnit.PlayAttackAnimation(damageDetails.IsCrit);
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();
        

        yield return playerHUD.UpdateHP();

        yield return enemyUnit.PlayBackupAnimation(damageDetails.IsCrit);
        yield return new WaitForSeconds(1f);

        if (damageDetails.IsDead)
        {
            //end battle scene
            playerUnit.PlayDeathAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(damageDetails, EventArgs.Empty);
        }
        else
        {
            OnBattleOver(damageDetails, EventArgs.Empty);
        }
    }

    

}
