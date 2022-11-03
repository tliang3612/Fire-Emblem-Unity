using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerUnit : Unit
{
    public int HealHandler(Unit unitToHeal)
    {
        unitToHeal.ReceiveHealing(AttackFactor);
        return AttackFactor;
    }
}
