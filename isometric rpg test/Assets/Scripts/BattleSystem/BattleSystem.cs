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
        enemyUnit.unit = defender;

        SetUpBattle();
    }

    public void SetUpBattle()
    {
        playerUnit.Setup(enemyUnit);
        //Pass in enemy unit as well to precalculate battle accuracy
        playerHUD.SetData(playerUnit.unit, enemyUnit.unit);

        enemyUnit.Setup(playerUnit);
        enemyHUD.SetData(enemyUnit.unit, playerUnit.unit);

        StartCoroutine(PerformPlayerMove());
    }

    private IEnumerator PerformPlayerMove()
    {
        var damageDetails = playerUnit.unit.AttackHandler(enemyUnit.unit, false);
        yield return new WaitForSeconds(1f);

        if (damageDetails.IsHit)
        {
            yield return playerUnit.PlayAttackAnimation(damageDetails.IsCrit);

            enemyUnit.PlayHitAnimation();
            yield return enemyHUD.UpdateHP();
            
            
            yield return playerUnit.PlayBackupAnimation(damageDetails.IsCrit);
        }
        else
        {
            yield return playerUnit.PlayAttackAnimation(damageDetails.IsCrit);

            yield return enemyUnit.PlayDodgeAnimation();

            yield return playerUnit.PlayBackupAnimation(damageDetails.IsCrit);
        }
        
        
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

        if (damageDetails.IsHit)
        {
            yield return enemyUnit.PlayAttackAnimation(damageDetails.IsCrit);

            playerUnit.PlayHitAnimation();
            yield return playerHUD.UpdateHP();

            yield return enemyUnit.PlayBackupAnimation(damageDetails.IsCrit);
        }
        else
        {
            yield return enemyUnit.PlayAttackAnimation(damageDetails.IsCrit);

            yield return playerUnit.PlayDodgeAnimation();

            yield return enemyUnit.PlayBackupAnimation(damageDetails.IsCrit);
        }

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
