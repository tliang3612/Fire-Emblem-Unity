using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Units;

public class HPUpdate : MonoBehaviour
{
    public Text HPText;

    void Start()
    {
        GetComponent<Unit>().UnitAttacked += OnUnitAttacked;
    }

    private void OnUnitAttacked(object sender, AttackEventArgs e)
    {
        HPText.text = ((int)Mathf.Ceil(GetComponent<Unit>().HitPoints * 10f / GetComponent<Unit>().TotalHitPoints)).ToString();
    }
}