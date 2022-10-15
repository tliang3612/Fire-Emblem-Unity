using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject hpBar;

    private void Start()
    {
        hpBar.transform.localScale = new Vector3(0.5f, 1f);
    }

    public IEnumerator SetHP(float newHp)
    {
        float currentHp = hpBar.transform.localScale.x;
        float changeAmount = currentHp - newHp;

        while(currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            hpBar.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }
        hpBar.transform.localScale = new Vector3(newHp, 1f);
    }
}
