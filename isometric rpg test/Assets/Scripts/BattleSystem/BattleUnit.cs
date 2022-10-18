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

    public Vector3 originalPosition;
    public Vector3 originalAnchoredPosition;

    public void Setup(BattleUnit battleUnitToAttack)
    {       
        GetComponent<Image>().sprite = unit.UnitBattleSprite;
        unitToAttack = battleUnitToAttack;

        originalPosition = GetComponent<RectTransform>().position;
        originalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    public IEnumerator PlayAttackAnimation(bool isCrit)
    {
        anim.SetTrigger("Attack");
        while(anim.GetCurrentAnimatorStateInfo(0).normalizedTime < anim.GetCurrentAnimatorStateInfo(0).length)
        {
            yield return null;
        }
        
             
    }

    public IEnumerator PlayBackupAnimation(bool isCrit)
    {
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
        var toDestinationInWorldSpace = unitToAttack.GetComponent<RectTransform>().position - GetComponent<RectTransform>().position;
        var toDestinationInLocalSpace = GetComponent<RectTransform>().InverseTransformVector(toDestinationInWorldSpace);
        GetComponent<RectTransform>().DOAnchorPos(toDestinationInLocalSpace, .2f).SetRelative(true);
    }

    public void MoveBackToPosition()
    {
        /*var toDestinationInLocalSpace = GetComponent<RectTransform>().InverseTransformVector(originalPosition - GetComponent<RectTransform>().position);
        GetComponent<RectTransform>().DOAnchorPosX(toDestinationInLocalSpace.x, .4f).SetRelative(true);*/
        Debug.Log(GetComponent<RectTransform>().anchoredPosition);
        GetComponent<RectTransform>().anchoredPosition = originalAnchoredPosition;
        Debug.Log(GetComponent<RectTransform>().anchoredPosition);
    }
}
