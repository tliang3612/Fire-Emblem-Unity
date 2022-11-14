using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitInfo", order = 3)]
public class UnitInfo : ScriptableObject
{
    public string Name;

    public int TotalMovementPoints;
    public int TotalActionPoints;

    public int TotalHitPoints;
    public int BaseAttack;
    public int BaseSkill;
    public int BaseSpeed;
    public int BaseLuck;
    public int BaseDefence;
    public int BaseConst;
       
    public Sprite Portrait;
    public Sprite BattleSprite;
    public RuntimeAnimatorController BattleAnimController;
    public List<Weapon> StartingWeapons;

}
