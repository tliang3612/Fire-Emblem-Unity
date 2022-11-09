using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] protected Animator anim;
    [SerializeField] private bool IsPlayer;
    public Unit unit { get; set; }
    

    public BattleSystemHUD HUD;

    public BattleUnit unitToAttack { get; set; }

    [HideInInspector] public GameObject hitEffect;
    [HideInInspector] public GameObject critEffect;
    

    //set to false with unity animation events
    public bool isAnimationPlaying;

    private Vector2 originalAnchoredPosition;

    public void SetupAttack(BattleUnit battleUnitToAttack, BattleEvent battleEvent)
    {       
        GetComponent<Image>().sprite = unit.UnitBattleSprite;
        unitToAttack = battleUnitToAttack;
        originalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        anim.runtimeAnimatorController = unit.BattleAnimController;

        hitEffect = unit.HitEffect;
        critEffect = unit.CritEffect;

        HUD.SetData(unit, unitToAttack.unit, battleEvent);      
    }

    public IEnumerator PlayAttackAnimation(bool isCrit)
    {
        if (isCrit)
            anim.SetTrigger("Crit");
        else
            anim.SetTrigger("Attack");

        isAnimationPlaying = true;

        while (isAnimationPlaying)
        {
            yield return null;
        }
                  
    }

    public IEnumerator PlayBackupAnimation(bool isCrit)
    {
        if (isCrit)
            anim.SetTrigger("CritBackup");
        else
            anim.SetTrigger("Backup");

        isAnimationPlaying = true;

        while (isAnimationPlaying)
        {
            yield return null;
        }
    }

    public IEnumerator PlayDodgeAnimation()
    {
        anim.SetTrigger("Dodge");

        isAnimationPlaying = true;

        while (isAnimationPlaying)
        {
            yield return null;
        }

    }

    public IEnumerator PlayHitAnimation(GameObject effect)
    {

        HitEffect hitEffect;
        
        if (effect.GetComponent<HitEffect>().IsUnitBased)
        {
            //effect is instantiated as a child of the unit
            hitEffect = Instantiate(effect, transform).GetComponent<HitEffect>();

            //centered on the unit
            hitEffect.SetProperties(Vector2.zero, Vector3.one);            
        }
        else
        {
            //effect is instantiated in the battle frame to take up the entire screen space, and centered based on the screen                       
            hitEffect = Instantiate(effect, GameObject.Find("BattleFrame").transform).GetComponent<HitEffect>();

            //reflect the image if the unit hit is the player unit
            if (IsPlayer)
                hitEffect.SetProperties(Vector2.zero, new Vector3(-1, 1, 1));
            else
                hitEffect.SetProperties(Vector2.zero, Vector3.one);
        }

        //PlayHitAnimation won't end until the hit effect animation is finished casting. Used for hit effects like magic
        yield return hitEffect.StartCasting();
    }

    public IEnumerator PlayHealAnimation()
    {
        anim.SetTrigger("Heal");
        isAnimationPlaying = true;

        while (isAnimationPlaying)
        {
            yield return null;
        }
    }

    public IEnumerator PlayHealBackupAnimation()
    {
        anim.SetTrigger("HealBackup");

        isAnimationPlaying = true;
        while (isAnimationPlaying)
        {
            yield return null;
        }

    }

    public void PlayHealingReceivedAnimation()
    {

    }

    public void PlayDeathAnimation()
    {
        Debug.Log("Dead");
    }

    public void EndPlayAnimation()
    {
        isAnimationPlaying = false;
    }

    
    public void MoveTowardsEnemy()
    {
        var toDestinationInLocalSpace = unitToAttack.GetComponent<RectTransform>().anchoredPosition - GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().DOAnchorPos(toDestinationInLocalSpace, .3f).SetRelative(true);
    }

    public void MoveBackToPosition()
    {
        var toDestinationInLocalSpace = originalAnchoredPosition - GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().DOAnchorPos(toDestinationInLocalSpace, .2f).SetRelative(true);
    }

    public void ShootArrow()
    {

    }
}
