using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public Unit unit { get; set; }
    public bool isPlayerUnit;


    public void Setup()
    {
        if (isPlayerUnit)
            GetComponent<Image>().sprite = unit.UnitPortrait;
    }
}
