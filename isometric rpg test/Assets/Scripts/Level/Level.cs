using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Level", order = 2)]
[System.Serializable]
public class Level : ScriptableObject
{
#if UNITY_EDITOR
    [HideInInspector] public bool showBoard;
#endif

    public int columns;
    public int rows;
    public TileType[,] board;

}

public enum TileType { 
    Plains, Forest, Cliff 
};

