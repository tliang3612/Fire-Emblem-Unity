using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanceUnit : Unit
{
    public override int GetEffectiveness(Unit unitToAttack)
    {
        if (unitToAttack is SwordUnit)
        {
            return 1;
        }
        else if (unitToAttack is AxeUnit)
        {
            return -1;
        }
        return 0;

    }
}
