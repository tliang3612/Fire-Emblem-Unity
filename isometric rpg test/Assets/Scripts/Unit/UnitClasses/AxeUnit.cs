using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeUnit : Unit
{
    public override int GetEffectiveness(Unit unitToAttack)
    {
        if (unitToAttack is LanceUnit)
        {
            return 1;
        }
        else if (unitToAttack is SwordUnit)
        {
            return -1;
        }
        return 0;
    }
}
