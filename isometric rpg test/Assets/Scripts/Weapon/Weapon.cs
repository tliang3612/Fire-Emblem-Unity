using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum WeaponType
{
	Sword,
	Lance,
	Axe,
	Bow,
	Staff,
	Tome		
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Weapon", order = 2)]
public class Weapon : Item
{
	public int Attack, Hit, Crit, Weight;
	public int Range;

	public string AnimationKey;
	public WeaponType Type;

	[HideInInspector] public UnitType UnitEffectiveAgainst;

	public GameObject HitEffect;
	public GameObject CritEffect;

	public bool HasProjectile;
	public GameObject Projectile;

	public bool HasUnitEffectiveness;

	public int GetEffectiveness(UnitType otherUnit, WeaponType otherWeapon)
	{
		int effectiveness = 0;

		//Unit Effectiveness
		if(HasUnitEffectiveness && otherUnit == UnitEffectiveAgainst)
        {
			effectiveness++;
        }

		//Weapon Triangle
		if(Type == WeaponType.Sword)
        {
			if (otherWeapon == WeaponType.Lance) effectiveness--;
			else if (otherWeapon == WeaponType.Axe) effectiveness++;
        }
		else if(Type == WeaponType.Axe)
        {
			if (otherWeapon == WeaponType.Sword) effectiveness--;
			else if (otherWeapon == WeaponType.Lance) effectiveness++;
		}
		else if(Type == WeaponType.Lance)
        {
			if (otherWeapon == WeaponType.Axe) effectiveness--;
			else if (otherWeapon == WeaponType.Sword) effectiveness++;
		}

		return Mathf.Clamp(effectiveness, -1, 2);
	}

	[CustomEditor(typeof(Weapon))]
	public class WeaponEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

			Weapon weapon = (Weapon)target;
			if (weapon.HasUnitEffectiveness)
            {
				weapon.UnitEffectiveAgainst = (UnitType)EditorGUILayout.EnumPopup("Unit Effective Against", weapon.UnitEffectiveAgainst);
			}
				
        }
    }
}
