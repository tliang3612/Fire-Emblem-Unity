using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Flying,
    Armoured,
    Horseback,
    Infantry,
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitInfo", order = 3)]
public class UnitInfo : ScriptableObject
{
    public string Name;
    public UnitType UnitType;

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
    public Sprite Mugshot;
    public Sprite MapSprite;

    public RuntimeAnimatorController BattleAnimController;
    public List<Item> StartingItems;

}
