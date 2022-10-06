using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MapData", order = 2)]
public class MapData : ScriptableObject
{
    //17 x 24
    public int height;
    public int width;
    public Dictionary<string, ScriptableObject> TileTypeMap;


}


