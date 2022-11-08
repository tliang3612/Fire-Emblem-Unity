using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject hpBar;
    [SerializeField] Text hpText;

    public void SetupHP(float currentHp, float totalHp)
    {

        hpBar.GetComponent<Image>().fillAmount = currentHp / totalHp;
        hpText.text = currentHp.ToString();
    }

    public IEnumerator SetHP(float newHp, float maxHp)
    {
        //the current hp text
        float currentText = float.Parse(hpText.text);
        float newText = newHp;
        float floatChangeAmount = currentText - newText;

        //new fill amount
        float newFill = newHp / maxHp;

        
        //current fill of the hp bar
        float currentFill = hpBar.GetComponent<Image>().fillAmount;       
        float changeAmount = currentFill - newFill;

        //if the new health is greater than the old health
        var a = newFill > currentFill ? -1 : 1;

        while (a * (currentFill - newFill) > Mathf.Epsilon)
        {
            currentFill -= changeAmount * Time.deltaTime;
            currentText -= floatChangeAmount * Time.deltaTime;

            hpText.text = ((int)currentText).ToString();
            hpBar.GetComponent<Image>().fillAmount = currentFill;        
            yield return null;
        }

        hpText.text = ((int)newHp).ToString();
        hpBar.GetComponent<Image>().fillAmount = newFill;
    }
}
