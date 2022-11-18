using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Staff", order = 4)]
public class Staff : Item
{
    public int HealAmount;
    public int Range;

    public GameObject HealEffect;
}
