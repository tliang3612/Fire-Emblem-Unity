using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleEvent
{
    RangedAction,
    MeleeAction,
    HealAction
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;

    public GameObject rightPlatform;
    public GameObject leftPlatform;

    [SerializeField] public float RangedPlatformOffset;
    private Vector2 originalRightAnchoredPosition;
    private Vector2 originalLeftAnchoredPosition;

    public event EventHandler BattleOver;

    private bool battleOver = false;

    public void Start()
    {
        gameObject.SetActive(false);
        originalRightAnchoredPosition = rightPlatform.GetComponent<RectTransform>().anchoredPosition;
        originalLeftAnchoredPosition = leftPlatform.GetComponent<RectTransform>().anchoredPosition;
    }

    public void StartBattle(Unit attacker, Unit defender, BattleEvent battleEvent)
    {
        battleOver = false;

        playerUnit.unit = attacker;
        enemyUnit.unit = defender;

        SetUpBattle(battleEvent);

        if(battleEvent == BattleEvent.HealAction)
        {
            var healingDetails = (attacker as HealerUnit).HealHandler(defender);
            StartCoroutine(PerformHealerMove(healingDetails));
        }
        else
        {
            var damageDetails = attacker.AttackHandler(defender, false);
            StartCoroutine(PerformAttackerMove(damageDetails));
        }
        if(battleEvent == BattleEvent.RangedAction)
        {
            SetUpRangedPlatforms();
        }
        
    }

    public void SetUpBattle(BattleEvent battleEvent)
    {
        playerUnit.SetupAttack(enemyUnit, battleEvent);
        enemyUnit.SetupAttack(playerUnit, battleEvent);                    
    }

    private IEnumerator RunAttackSequence(BattleUnit attackerUnit, BattleUnit defenderUnit, DamageDetails damageDetails)
    {       
        yield return new WaitForSeconds(0.5f);

        if (damageDetails.IsHit)
        {
            yield return attackerUnit.PlayAttackAnimation(damageDetails.IsCrit);
            yield return defenderUnit.PlayHitAnimation(damageDetails.IsCrit ? attackerUnit.critEffect : attackerUnit.hitEffect);
            yield return defenderUnit.HUD.UpdateHP();

            yield return attackerUnit.PlayBackupAnimation(damageDetails.IsCrit);
        }
        else
        {
            yield return attackerUnit.PlayAttackAnimation(false);
            yield return defenderUnit.PlayDodgeAnimation();
            yield return attackerUnit.PlayBackupAnimation(false);
        }
        yield return new WaitForSeconds(0.5f);

        if (damageDetails.IsDead)
        {
            battleOver = true;
            defenderUnit.PlayDeathAnimation();
            yield return new WaitForSeconds(1f);

            EndBattle(attackerUnit.unit, defenderUnit.unit);

            
        }
        else if (attackerUnit == enemyUnit)
        {
            EndBattle(attackerUnit.unit, defenderUnit.unit);
        }
    }

    private IEnumerator RunHealerSequence(BattleUnit healerUnit, BattleUnit allyUnit, HealingDetails healingDetails)
    {
        yield return new WaitForSeconds(0.5f);

        yield return healerUnit.PlayHealAnimation();
        allyUnit.PlayHealingReceivedAnimation();
        yield return allyUnit.HUD.UpdateHP();

        yield return healerUnit.PlayHealBackupAnimation();
        yield return new WaitForSeconds(0.5f);

        EndBattle(healerUnit.unit, allyUnit.unit);
    }


    public void EndBattle(Unit attacker, Unit defender)
    {
        if (BattleOver != null)
            BattleOver.Invoke(this, EventArgs.Empty);

        CleanpRangedPlatforms();

        playerUnit.unit.SetState(new UnitStateFinished(playerUnit.unit));
        Debug.Log("battle ended");
    }

    private IEnumerator PerformHealerMove(HealingDetails healingDetails)
    {
        yield return RunHealerSequence(playerUnit, enemyUnit, healingDetails);
    }
    
    private IEnumerator PerformDefenderMove(DamageDetails damageDetails)
    {    
        yield return RunAttackSequence(enemyUnit, playerUnit, damageDetails);
    }

    private IEnumerator PerformAttackerMove(DamageDetails damageDetails)
    {
        yield return RunAttackSequence(playerUnit, enemyUnit, damageDetails);

        //Get Defender Damage details
        var defenderDamageDetails = enemyUnit.unit.AttackHandler(playerUnit.unit, true);

        if (defenderDamageDetails.InRange && !battleOver)
             StartCoroutine(PerformDefenderMove(defenderDamageDetails));
        else
            EndBattle(playerUnit.unit, enemyUnit.unit);
        
    }

    public void SetUpRangedPlatforms()
    {
        var rightPlatFormPosition = rightPlatform.GetComponent<RectTransform>().anchoredPosition;
        rightPlatform.GetComponent<RectTransform>().anchoredPosition = new Vector2(rightPlatFormPosition.x + RangedPlatformOffset, rightPlatFormPosition.y);

        var leftPlatFormPosition = leftPlatform.GetComponent<RectTransform>().anchoredPosition;
        leftPlatform.GetComponent<RectTransform>().anchoredPosition = new Vector2(leftPlatFormPosition.x - RangedPlatformOffset, leftPlatFormPosition.y);

        var playerPos = playerUnit.GetComponent<RectTransform>().anchoredPosition;
        playerUnit.GetComponent<RectTransform>().anchoredPosition = new Vector2(playerPos.x + RangedPlatformOffset, playerPos.y);

        var enemyPos = enemyUnit.GetComponent<RectTransform>().anchoredPosition;
        enemyUnit.GetComponent<RectTransform>().anchoredPosition = new Vector2(enemyPos.x - RangedPlatformOffset, enemyPos.y);
    }

    public void CleanpRangedPlatforms()
    {        
        rightPlatform.GetComponent<RectTransform>().anchoredPosition = originalRightAnchoredPosition;
        leftPlatform.GetComponent<RectTransform>().anchoredPosition = originalLeftAnchoredPosition;

        playerUnit.GetComponent<RectTransform>().anchoredPosition = originalRightAnchoredPosition;
        enemyUnit.GetComponent<RectTransform>().anchoredPosition = originalLeftAnchoredPosition;
    }
}
