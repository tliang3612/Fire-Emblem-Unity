using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    public Unit unit { get; set; }
    public BattleUnit unitToAttack { get; set; }
    public bool isPlayerUnit;
    public Animator anim;

    public Vector2 originalAnchoredPosition;

    public void Setup(BattleUnit battleUnitToAttack)
    {       
        GetComponent<Image>().sprite = unit.UnitBattleSprite;
        unitToAttack = battleUnitToAttack;

        originalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public IEnumerator PlayAttackAnimation(bool isCrit)
    {
        if (isCrit)
            Debug.Log("PlayCritAnimation");
        else
            anim.SetTrigger("Attack");

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < anim.GetCurrentAnimatorStateInfo(0).length)
        {
            yield return null;
        }
        
             
    }

    public IEnumerator PlayBackupAnimation(bool isCrit)
    {
        if (isCrit)
            Debug.Log("PlayCritAnimation");
        else
            anim.SetTrigger("Backup");

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < anim.GetCurrentAnimatorStateInfo(0).length)
        {
            yield return null;
        }
    }

    public void PlayHitAnimation()
    {
        Debug.Log("Hit");
    }

    public void PlayDeathAnimation()
    {
        Debug.Log("Dead");
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
