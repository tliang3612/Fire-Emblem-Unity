using System.Collections;
using UnityEngine;
using Units;

public class AttackAnimation : MonoBehaviour
{
    public float speed = 3f;
    private Vector2 originalPos;

    public void StartAttackAnimation(Unit unit, Unit otherUnit)
    {
        Debug.Log("attack started");
        otherUnit.MarkAsUnderAttack();
        originalPos = unit.transform.position;
        StartCoroutine(AttackAnimationCoroutine(unit, otherUnit));
    }

    private IEnumerator AttackAnimationCoroutine(Unit unit, Unit otherUnit)
    {
        
        //MoveUnit until half the distance of other unit
        while(Vector2.Distance(unit.transform.position, otherUnit.transform.position) > 0.01f)
        {
            unit.transform.position = Vector2.MoveTowards(unit.transform.position, otherUnit.transform.position, speed * Time.deltaTime);

            yield return null;
        }

        while (Vector2.Distance(unit.transform.position, originalPos) > 0.01f)
        {
            unit.transform.position = Vector2.MoveTowards(unit.transform.position, originalPos, speed * Time.deltaTime);

            yield return null;
        }

        unit.transform.position = originalPos;
        otherUnit.UnMark();
    }
}
