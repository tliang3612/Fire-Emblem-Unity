using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordUnit : Unit 
{
    public override int GetEffectiveness(Unit unitToAttack)
    {
        if (unitToAttack is AxeUnit)
        {
            return 1;
        }
        else if (unitToAttack is LanceUnit)
        {
            return -1;
        }
        return 0;
    }
}
