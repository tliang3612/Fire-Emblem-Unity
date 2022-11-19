using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum BattleEvent
{
    RangedAction,
    MeleeAction,
    HealAction
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private GameObject rightPlatform;
    [SerializeField] private GameObject leftPlatform;

    //For Ranged battles where these components will be shifted
    [SerializeField] private GameObject[] PannedComponents = new GameObject[4];
    [SerializeField] private GameObject foreground;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;

    [SerializeField] private Image Background;

    [SerializeField] public float RangedPlatformOffset;
    [SerializeField] public float panDuration; 
    private Vector2 originalRightAnchoredPosition;
    private Vector2 originalLeftAnchoredPosition;

    //For battle between two players
    private CombatCalculator combatCalculator;
    private CombatStats playerStats;
    private CombatStats enemyStats;

    //For healing action
    private HealStats healStats;

    private int range;
    public event EventHandler BattleOver;

    public void Start()
    {
        gameObject.SetActive(false);
        originalRightAnchoredPosition = rightPlatform.GetComponent<RectTransform>().anchoredPosition;
        originalLeftAnchoredPosition = leftPlatform.GetComponent<RectTransform>().anchoredPosition;
    }

    public void StartBattle(Unit attacker, Unit defender, BattleEvent battleEvent)
    {
        range = battleEvent == BattleEvent.RangedAction ? 2 : 1;
        playerUnit.Unit = attacker;
        enemyUnit.Unit = defender;

        if (battleEvent == BattleEvent.RangedAction)
        {
            SetUpRangedPlatforms();
        }

        if (battleEvent == BattleEvent.HealAction)
        {
            healStats = new HealStats(attacker, defender);

            SetUpHeal(healStats);
            StartCoroutine(PerformHealerMove());
        }
        else
        {
            playerStats = new CombatStats(attacker, defender, range);
            enemyStats = new CombatStats(defender, attacker, range);
            SetUpBattle(playerStats, enemyStats);

            Queue<BattleAction> queue;
            combatCalculator = new CombatCalculator(playerStats, enemyStats, range);

            queue = combatCalculator.Calculate();
            StartCoroutine(RunBattleSequence(queue));
        }
    }

    public IEnumerator RunBattleSequence(Queue<BattleAction> battleActions)
    {       
        yield return new WaitForSeconds(0.5f);
        
        BattleUnit attacker;
        BattleUnit defender;

        while(battleActions.Count != 0)
        {
            yield return new WaitForSeconds(0.5f);

            var currentAction = battleActions.Dequeue();
            
            attacker = currentAction.IsPlayerAttacking ? playerUnit : enemyUnit;
            defender = currentAction.IsPlayerAttacking ? enemyUnit : playerUnit;

            if (currentAction.IsHit)
            {
                yield return attacker.PlayAttackAnimation(currentAction.IsCrit);
                if (range >= 2)
                    yield return ShiftPlatformsAndUnits(attacker.IsPlayer ? 1 : -1);

                defender.Unit.ReceiveDamage(attacker.Unit, currentAction.Damage);
                yield return defender.PlayHitAnimation(currentAction.IsCrit ? attacker.critEffect : attacker.hitEffect);
                ShakeBattlefield(currentAction.IsCrit ? 2 : 1);
                yield return defender.HUD.UpdateHP();
                yield return new WaitForSeconds(0.5f);
                yield return attacker.PlayBackupAnimation(currentAction.IsCrit);

                if (range >= 2)
                    yield return ShiftPlatformsAndUnits(attacker.IsPlayer ? -1f : 1f);
            }
            else
            {
                yield return attacker.PlayAttackAnimation(false);
                if (range >= 2)
                    yield return ShiftPlatformsAndUnits(attacker.IsPlayer ? 1 : -1);

                yield return defender.PlayDodgeAnimation();
                yield return attacker.PlayBackupAnimation(false);

                if (range >= 2)
                    yield return ShiftPlatformsAndUnits(attacker.IsPlayer ? -1f : 1f);

                Debug.Log("Enemy Dodged");
            }

            if (currentAction.IsDead)
            {
                defender.PlayDeathAnimation();
                yield return new WaitForSeconds(1f);
                EndBattle();
            }
        }

        yield return new WaitForSeconds(0.5f);

        //EndBattle at the end of the sequence
        EndBattle();       
    }

    public void SetUpBattle(CombatStats playerStats, CombatStats enemyStats)
    {
        playerUnit.SetupAttack(playerStats, enemyUnit);
        enemyUnit.SetupAttack(enemyStats, playerUnit);                    
    }

    public void SetUpHeal(HealStats healStats)
    {
        playerUnit.SetupHeal(healStats, enemyUnit);
        enemyUnit.SetUpEmpty(playerUnit);
    }

    private IEnumerator RunHealerSequence(BattleUnit healerUnit, BattleUnit allyUnit)
    {
        yield return new WaitForSeconds(1.5f);

        yield return healerUnit.PlayAttackAnimation(false);
        allyUnit.Unit.ReceiveHealing(healerUnit.Unit.UnitAttack);
        allyUnit.PlayHealingReceivedAnimation();
        yield return allyUnit.HUD.UpdateHP();

        yield return healerUnit.PlayBackupAnimation(false);
        yield return new WaitForSeconds(0.5f);

        EndBattle();
    }


    public void EndBattle()
    {
        if (BattleOver != null)
            BattleOver.Invoke(this, EventArgs.Empty);

        CleanpRangedPlatforms();
        playerUnit.EndBattleAnimation();
        enemyUnit.EndBattleAnimation();

        playerUnit.Unit.SetState(new UnitStateFinished(playerUnit.Unit));
        Debug.Log("battle ended");
    }

    private IEnumerator PerformHealerMove()
    {
        yield return RunHealerSequence(playerUnit, enemyUnit);
    }

    public void SetUpRangedPlatforms()
    {
        foreach(GameObject gameObject in PannedComponents)
        {
            var position = gameObject.GetComponent<RectTransform>().anchoredPosition;
            //if position.x > 0, then the component is on the right side, which is the player side
            var k = position.x > 0 ? 1 : -1;

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x + k*RangedPlatformOffset, position.y);           
        }
    }

    //k is positive if its player Unit
    public IEnumerator ShiftPlatformsAndUnits(float k)
    {        
        foreach(GameObject gameObject in PannedComponents)
        {
            var positionX = gameObject.GetComponent<RectTransform>().anchoredPosition.x;
            gameObject.GetComponent<RectTransform>().DOAnchorPosX(positionX + k * (RangedPlatformOffset), panDuration).SetEase(Ease.Linear);
        }

        yield return new WaitForSeconds(panDuration);

    }

    public void CleanpRangedPlatforms()
    {        
        rightPlatform.GetComponent<RectTransform>().anchoredPosition = originalRightAnchoredPosition;
        leftPlatform.GetComponent<RectTransform>().anchoredPosition = originalLeftAnchoredPosition;

        playerUnit.GetComponent<RectTransform>().anchoredPosition = originalRightAnchoredPosition;
        enemyUnit.GetComponent<RectTransform>().anchoredPosition = originalLeftAnchoredPosition;
    }

    public void ShakeBattlefield(float shakeMultiplier)
    {
        foreground.GetComponent<Transform>().DOShakePosition(shakeDuration * shakeMultiplier, shakeMagnitude * shakeMultiplier, fadeOut: true);
    }
    
}


