using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HealingDetails 
{
    public int AmountHealed { get; set; }
    public int NewHealth { get; set; }


    public HealingDetails(int amountHealed = 0, int newHealth = 0)
    {
        AmountHealed = amountHealed;
        NewHealth = newHealth;
    }
}
