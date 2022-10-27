using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{

    [SerializeField] private Animator anim;

    public Unit unit { get; set; }

    public BattleSystemHUD HUD;

    public BattleUnit unitToAttack { get; set; }
    public GameObject HitEffect;
    

    //set to false with unity animation events
    public bool isAnimationPlaying;

    private Vector2 originalAnchoredPosition;

    public void Setup(BattleUnit battleUnitToAttack)
    {       
        GetComponent<Image>().sprite = unit.UnitBattleSprite;
        unitToAttack = battleUnitToAttack;
        originalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        HUD.SetData(unit, unitToAttack.unit);
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

    public void PlayHitAnimation()
    {
        //Instantiate Hit Effect at the center of unit
        var hitEffect = Instantiate(HitEffect, transform.position, transform.rotation, transform);
        hitEffect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        Destroy(hitEffect, 2f);
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
        GetComponent<RectTransform>().DOAnchorPos(toDestinationInLocalSpace, .2f).SetRelative(true);
    }

    public void MoveBackToPosition()
    {
        var toDestinationInLocalSpace = originalAnchoredPosition - GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().DOAnchorPos(toDestinationInLocalSpace, .2f).SetRelative(true);

    }
}
