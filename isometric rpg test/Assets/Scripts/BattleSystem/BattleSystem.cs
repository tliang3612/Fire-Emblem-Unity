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
    [SerializeField] private GameObject[] PannedComponents = new GameObject[4];
    [SerializeField] private GameObject foreground;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;

    [SerializeField] private Image Background;

    private BattleEvent currentBattleEvent;

    [SerializeField] public float RangedPlatformOffset;
    [SerializeField] public float panDuration; 
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

        currentBattleEvent = battleEvent;

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
        Action<float> shake = ShakeBattlefield;

        yield return new WaitForSeconds(0.5f);

        if (damageDetails.IsHit)
        {
            yield return attackerUnit.PlayAttackAnimation(damageDetails.IsCrit);
            if(currentBattleEvent == BattleEvent.RangedAction)
                yield return ShiftPlatformsAndUnits(attackerUnit.IsPlayer ? 1 : -1);

            yield return defenderUnit.PlayHitAnimation(damageDetails.IsCrit ? attackerUnit.critEffect : attackerUnit.hitEffect);
            ShakeBattlefield(damageDetails.IsCrit ? 2 : 1);
            yield return defenderUnit.HUD.UpdateHP();
            yield return attackerUnit.PlayBackupAnimation(damageDetails.IsCrit);

            if (currentBattleEvent == BattleEvent.RangedAction)
                yield return ShiftPlatformsAndUnits(attackerUnit.IsPlayer ? -1f : 1f);
        }
        else
        {
            yield return attackerUnit.PlayAttackAnimation(false);
            if (currentBattleEvent == BattleEvent.RangedAction)
                yield return ShiftPlatformsAndUnits(attackerUnit.IsPlayer ? 1 : -1);

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
        foreach(GameObject gameObject in PannedComponents)
        {
            var position = gameObject.GetComponent<RectTransform>().anchoredPosition;
            //if position.x > 0, then the component is on the right side, which is the player side
            var k = position.x > 0 ? 1 : -1;

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x + k*RangedPlatformOffset, position.y);           
        }
    }

    //k is positive if its player unit
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
