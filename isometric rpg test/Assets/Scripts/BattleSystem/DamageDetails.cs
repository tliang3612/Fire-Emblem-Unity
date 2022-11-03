using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetails
{
    public bool IsDead { get; set; }
    public bool IsHit { get; set; }
    public int TotalDamage { get; set; }
    public bool IsCrit { get; set; }
    public bool InRange { get; set; }

    
    public DamageDetails(bool inRange, bool isDead = false, bool isHit = false, bool isCrit = false,  int totalDamage = 0)
    {
        IsDead = isDead;
        IsHit = isHit;
        IsCrit = isCrit;
        InRange = inRange;
        TotalDamage = totalDamage;
    }

}
