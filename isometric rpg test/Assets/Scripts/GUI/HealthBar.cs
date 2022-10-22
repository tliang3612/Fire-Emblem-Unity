using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject hpBar;
    [SerializeField] Text hpText;

    public void SetupHP(float currentHp, float totalHp)
    {

        hpBar.GetComponent<Image>().fillAmount = currentHp/totalHp;
        hpText.text = currentHp.ToString();
    }

    public IEnumerator SetHP(float newHp, float maxHp)
    {
        //current fill of the hp bar
        float currentHp = hpBar.GetComponent<Image>().fillAmount;
        //amount
        float changeAmount = currentHp - newHp;

        while(currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            hpBar.GetComponent<Image>().fillAmount = currentHp;
            hpText.text = ((int)currentHp).ToString();
            yield return null;
        }

        hpBar.GetComponent<Image>().fillAmount = newHp;
    }
}
