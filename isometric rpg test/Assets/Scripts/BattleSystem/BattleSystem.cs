using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleAction {Attack, Defend }
public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;

    [SerializeField] BattleUnit enemyUnit;

    public event EventHandler OnBattleOver;

    private bool battleOver = false;

    public void StartBattle(Unit attacker, Unit defender)
    {
        battleOver = false;

        playerUnit.unit = attacker;
        enemyUnit.unit = defender;

        SetUpBattle();
    }

    public void SetUpBattle()
    {
        playerUnit.Setup(enemyUnit);
        enemyUnit.Setup(playerUnit);

        StartCoroutine(PerformAttackerMove());
             
    }

    private IEnumerator RunSequence(BattleUnit attackerUnit, BattleUnit defenderUnit)
    {
        var damageDetails = attackerUnit.unit.AttackHandler(defenderUnit.unit, attackerUnit.Equals(enemyUnit));
        yield return new WaitForSeconds(0.5f);

        if (damageDetails.IsHit)
        {
            yield return attackerUnit.PlayAttackAnimation(damageDetails.IsCrit);
            defenderUnit.PlayHitAnimation();
            yield return defenderUnit.HUD.UpdateHP();

            yield return attackerUnit.PlayBackupAnimation(damageDetails.IsCrit);
        }
        else
        {
            yield return attackerUnit.PlayAttackAnimation(false);
            yield return defenderUnit.PlayDodgeAnimation();
            yield return attackerUnit.PlayBackupAnimation(false);
        }

        if (damageDetails.IsDead)
        {
            battleOver = true;
            defenderUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(1f);
            if (OnBattleOver != null)
                OnBattleOver.Invoke(damageDetails, EventArgs.Empty);
        }
        else if (attackerUnit == enemyUnit)
        {
            if (OnBattleOver != null)
                OnBattleOver.Invoke(damageDetails, EventArgs.Empty);
        }
    }

    private IEnumerator PerformDefenderMove()
    {
        yield return RunSequence(enemyUnit, playerUnit);
    }

    private IEnumerator PerformAttackerMove()
    {
        yield return RunSequence(playerUnit, enemyUnit);
        if (!battleOver)
        {
            StartCoroutine(PerformDefenderMove());
        }
    }

   
}
