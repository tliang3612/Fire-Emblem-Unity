using System.Collections;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public bool IsUnitBased;
    private bool IsCasting;

    public void SetProperties(Vector2 anchoredPos, Vector3 localScale)
    {         
        GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        GetComponent<RectTransform>().localScale = localScale;
    }
    public IEnumerator StartCasting()
    {
        IsCasting = true;

        while (IsCasting)
        {
            yield return null;
        }
    }

    public void FinishCasting()
    {
        IsCasting = false;
        Destroy(gameObject, 2f);
    }
}
