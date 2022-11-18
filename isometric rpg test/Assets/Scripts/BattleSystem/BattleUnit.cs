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
    private GameObject _projectilePrefab;

    public Unit Unit { get; set; }    
    public BattleSystemHUD HUD;
    public BattleUnit _unitToAttack { get; set; }

    [HideInInspector] public GameObject hitEffect;
    [HideInInspector] public GameObject critEffect;

    private Image background;
    private string _animationStateKey;

    //set to false with unity animation events
    public bool isAnimationPlaying;

    private Vector2 originalAnchoredPosition;

    public void SetupAttack(CombatStats stats, BattleUnit battleUnitToAttack)
    {

        _animationStateKey = Unit.EquippedWeapon.Name;

        SetUp(battleUnitToAttack);

        hitEffect = Unit.EquippedWeapon.HitEffect;
        critEffect = Unit.EquippedWeapon.CritEffect;

        HUD.SetAttackData(Unit, stats);

        //if the unit's weapon shoots a projectile
        if (Unit.EquippedWeapon.HasProjectile)
        {
            _projectilePrefab = Unit.EquippedWeapon.Projectile;
        }
    }

    public void SetupHeal(HealStats stats, BattleUnit battleUnitToAttack)
    {
        _animationStateKey = Unit.EquippedStaff.Name;

        SetUp(battleUnitToAttack);

        hitEffect = Unit.EquippedStaff.HealEffect;
        critEffect = Unit.EquippedStaff.HealEffect;        

        HUD.SetHealData(Unit, stats);
    }

    public void SetUpEmpty(BattleUnit battleUnitToAttack)
    { 
        _animationStateKey = Unit.EquippedWeapon.Name;

        SetUp(battleUnitToAttack);
        HUD.SetEmptyData(Unit);

        
    }

    public void SetUp(BattleUnit battleUnitToAttack)
    {
        _unitToAttack = battleUnitToAttack;

        GetComponent<Image>().sprite = Unit.UnitBattleSprite;
        originalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        background = GameObject.Find("DimBackground").GetComponent<Image>();

        anim.runtimeAnimatorController = Unit.BattleAnimController;        

        anim.SetBool(_animationStateKey, true);
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
        
        if(_projectilePrefab)
        {
            yield return ShootProjectile(_projectilePrefab);
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

        //reflect the image if the Unit hit is the player Unit
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
        var toDestinationInLocalSpace = _unitToAttack.GetComponent<RectTransform>().anchoredPosition - GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().DOAnchorPos(toDestinationInLocalSpace, .3f).SetRelative(true);
    }

    public void MoveBackToPosition()
    {
        var toDestinationInLocalSpace = originalAnchoredPosition - GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().DOAnchorPos(toDestinationInLocalSpace, .2f).SetRelative(true);
    }

    public IEnumerator ShootProjectile(GameObject projectileObject)
    {
        //effect is instantiated in the battle frame to take up the entire screen space, and centered based on the screen                       
        Projectile projectile = Instantiate(projectileObject, PrefabHolder.transform).GetComponent<Projectile>();

        //reflect the image if the Unit hit is the enemy Unit
        projectile.SetProperties(GetComponent<RectTransform>().anchoredPosition.x, _unitToAttack.GetComponent<RectTransform>().anchoredPosition.x, IsPlayer ? 1 : -1);

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

    public void EndBattleAnimation()
    {
        anim.SetBool(_animationStateKey, false);
    }

}
