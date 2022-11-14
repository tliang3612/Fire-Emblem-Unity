using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] protected Animator anim;
    [SerializeField] public bool IsPlayer;
    [SerializeField] public GameObject PrefabHolder;
    [SerializeField] public GameObject ProjectilePrefab;

    public Unit unit { get; set; }    
    public BattleSystemHUD HUD;
    public BattleUnit unitToAttack { get; set; }

    [HideInInspector] public GameObject hitEffect;
    [HideInInspector] public GameObject critEffect;

    private Image background;   
    //set to false with unity animation events
    public bool isAnimationPlaying;

    private Vector2 originalAnchoredPosition;

    public void SetupAttack(CombatStats stats, BattleUnit battleUnitToAttack)
    {       
        GetComponent<Image>().sprite = unit.UnitBattleSprite;
        unitToAttack = battleUnitToAttack;
        originalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        anim.runtimeAnimatorController = unit.BattleAnimController;

        hitEffect = unit.EquippedWeapon.HitEffect;
        critEffect = unit.EquippedWeapon.CritEffect;

        background = GameObject.Find("DimBackground").GetComponent<Image>();
        HUD.SetData(unit, stats);      
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
        
        if(unit.UnitName == "Lyn")
        {
            yield return ShootArrow();
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
        //effect is instantiated in the battle frame to take up the entire screen space, and centered based on the screen                       
        hitEffect = Instantiate(effect, PrefabHolder.transform).GetComponent<HitEffect>();

        //reflect the image if the unit hit is the player unit
        if (IsPlayer)
            hitEffect.SetProperties(Vector2.zero, new Vector3(-1, 1, 1));
        else
            hitEffect.SetProperties(Vector2.zero, Vector3.one);

        if (hitEffect.CanDimBackground)
            DimBackground();

        //PlayHitAnimation won't end until the hit effect animation is finished casting. Used for hit effects like magic
        yield return hitEffect.StartCasting();

        if (hitEffect.CanDimBackground)
            UndimBackground();
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

    public IEnumerator ShootArrow()
    {
        Projectile projectile;
        //effect is instantiated in the battle frame to take up the entire screen space, and centered based on the screen                       
        projectile = Instantiate(ProjectilePrefab, PrefabHolder.transform).GetComponent<Projectile>();

        //reflect the image if the unit hit is the enemy unit
        projectile.SetProperties(GetComponent<RectTransform>().anchoredPosition.x, unitToAttack.GetComponent<RectTransform>().anchoredPosition.x, IsPlayer ? 1 : -1);

        yield return projectile.MoveProjectile();

    }

    public void DimBackground()
    {
        background.DOFade(0.5f, 0.2f);
    }

    public void UndimBackground()
    {
        background.DOFade(0, 0.2f);
    }

}
