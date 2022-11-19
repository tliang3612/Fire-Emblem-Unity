using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBar : MonoBehaviour
{
    private int barWidth = 16;
    private int barHeight = 55;

    [SerializeField] private GameObject _hpBar;
    [SerializeField] private GameObject _emptyHpBar;
    [SerializeField] Text hpText;

    private void Start()
    {
        barWidth = (int)_hpBar.GetComponent<RectTransform>().sizeDelta.x;
        barHeight = (int)_hpBar.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void SetupHP(int currentHp, float totalHp)
    {
        _emptyHpBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth * totalHp, barHeight);
        _hpBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth * currentHp, barHeight);
        hpText.text = currentHp.ToString();
    }

    public IEnumerator SetHP(float newHp)
    {
        //the current hp text
        float currentText = float.Parse(hpText.text);
        float newText = newHp;
        float floatChangeAmount = currentText - newText;

        //new fill amount
        float newWidth = newHp * barWidth;


        //current fill of the hp bar
        float currentWidth = _hpBar.GetComponent<RectTransform>().sizeDelta.x; 
        float changeAmount = currentWidth - newWidth;

        //if the new health is greater than the old health
        var a = newWidth > currentWidth ? -1 : 1;

        while (a * (currentWidth - newWidth) > Mathf.Epsilon)
        {
            currentWidth -= changeAmount * Time.deltaTime;
            currentText -= floatChangeAmount * Time.deltaTime;

            hpText.text = ((int)currentText).ToString();
            _hpBar.GetComponent<RectTransform>().sizeDelta = new Vector2(currentWidth, barHeight);       
            yield return null;
        }

        hpText.text = ((int)newHp).ToString();
        _hpBar.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, barHeight);
    }
}
