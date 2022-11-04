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
    private Vector2 originalAnchoredPosition;

    public event EventHandler<BattleOverEventArgs> BattleOver;

    private bool battleOver = false;

    public void Start()
    {
        gameObject.SetActive(false);
        originalAnchoredPosition = rightPlatform.GetComponent<RectTransform>().anchoredPosition;
    }

    public void StartBattle(Unit attacker, Unit defender, BattleEvent battleEvent)
    {
        battleOver = false;

        playerUnit.unit = attacker;
        enemyUnit.unit = defender;

        SetUpBattle(battleEvent);

        if(battleEvent == BattleEvent.HealAction)
        {
            StartCoroutine(PerformHealerMove());
        }
        else
        {
            var damageDetails = playerUnit.unit.AttackHandler(enemyUnit.unit, false);
            StartCoroutine(PerformAttackerMove(damageDetails));
        }

        if(battleEvent == BattleEvent.RangedAction)
        {
            Debug.Log(true);
            var rightPlatFormPosition = rightPlatform.GetComponent<RectTransform>().anchoredPosition;
            rightPlatform.GetComponent<RectTransform>().anchoredPosition = new Vector2(rightPlatFormPosition.x + RangedPlatformOffset, rightPlatFormPosition.y);

            var leftPlatFormPosition = leftPlatform.GetComponent<RectTransform>().anchoredPosition;
            leftPlatform.GetComponent<RectTransform>().anchoredPosition = new Vector2(leftPlatFormPosition.x - RangedPlatformOffset, leftPlatFormPosition.y);

            var playerPos = playerUnit.GetComponent<RectTransform>().anchoredPosition;
            playerUnit.GetComponent<RectTransform>().anchoredPosition = new Vector2(playerPos.x + RangedPlatformOffset, playerPos.y);

            var enemyPos = enemyUnit.GetComponent<RectTransform>().anchoredPosition;
            enemyUnit.GetComponent<RectTransform>().anchoredPosition = new Vector2(enemyPos.x - RangedPlatformOffset, enemyPos.y);
        }

        
    }

    public void SetUpBattle(BattleEvent battleEvent)
    {
        playerUnit.SetupAttack(enemyUnit, battleEvent);
        enemyUnit.SetupAttack(playerUnit, battleEvent);                    
    }

    private IEnumerator RunSequence(BattleUnit attackerUnit, BattleUnit defenderUnit, DamageDetails damageDetails)
    {       
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

            EndBattle(damageDetails, attackerUnit.unit, defenderUnit.unit);

            
        }
        else if (attackerUnit == enemyUnit)
        {
            EndBattle(damageDetails, attackerUnit.unit, defenderUnit.unit);
        }
    }

    public void EndBattle(DamageDetails damageDetails, Unit attacker, Unit defender)
    {
        if (BattleOver != null)
            BattleOver.Invoke(this, new BattleOverEventArgs(attacker, defender, damageDetails.IsDead));

        leftPlatform.GetComponent<RectTransform>().anchoredPosition = -originalAnchoredPosition;
        rightPlatform.GetComponent<RectTransform>().anchoredPosition = originalAnchoredPosition;



        playerUnit.unit.SetState(new UnitStateFinished(playerUnit.unit));
        Debug.Log("battle ended");
    }

    private IEnumerator PerformHealerMove()
    {
        yield return 0;
    }

    private IEnumerator PerformDefenderMove(DamageDetails damageDetails)
    {    
        yield return RunSequence(enemyUnit, playerUnit, damageDetails);
    }

    private IEnumerator PerformAttackerMove(DamageDetails damageDetails)
    {
        yield return RunSequence(playerUnit, enemyUnit, damageDetails);

        //Get Defender Damage details
        var defenderDamageDetails = enemyUnit.unit.AttackHandler(playerUnit.unit, true);

        if (defenderDamageDetails.InRange && !battleOver)
             StartCoroutine(PerformDefenderMove(defenderDamageDetails));
        else
            EndBattle(damageDetails, playerUnit.unit, enemyUnit.unit);
        
    }


    public class BattleOverEventArgs : EventArgs
    {
        public BattleOverEventArgs(Unit attacker, Unit defender, bool isDead)
        {
            Attacker = attacker;
            Defender = defender;
            IsDead = isDead;
        }

        public Unit Attacker;
        public Unit Defender;
        public bool IsDead;

    }
}
