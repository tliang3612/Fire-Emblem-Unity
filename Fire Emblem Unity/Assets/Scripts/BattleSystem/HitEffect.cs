using System.Collections;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private bool IsCasting;
    public bool CanDimBackground;

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
