using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="Data", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileData : ScriptableObject , ISerializationCallbackReceiver //Unity can't serialize dictionaries, so this is used as workaround
{
    public TileBase[] Tiles;

    public List<UnitType> UnitTypes = new List<UnitType>(4) { UnitType.Infantry, UnitType.Armoured, UnitType.Horseback, UnitType.Flying };

    public List<int> Costs;

    public Dictionary<UnitType, int> CostBasedOnUnit;

    public string TileName;
    public int DefenseBoost;
    public int AvoidBoost;

    //After unity serializes this object
    public void OnAfterDeserialize()
    {
        CostBasedOnUnit = new Dictionary<UnitType, int>();

        for (int i = 0; i < UnitTypes.Count; i++)
            CostBasedOnUnit.Add(UnitTypes[i], Costs[i]);
    }

    //Before unity serializes this object
    public void OnBeforeSerialize()
    {
        CostBasedOnUnit = new Dictionary<UnitType, int>();

        for (int i = 0; i < UnitTypes.Count; i++)
            CostBasedOnUnit.Add(UnitTypes[i], Costs[i]);
    }
  
}
