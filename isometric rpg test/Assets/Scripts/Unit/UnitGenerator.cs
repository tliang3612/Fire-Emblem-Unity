using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{

    public Transform UnitsContainer;
    public Transform OverlayTilesContainer;

    public List<Unit> SpawnUnits(TileGrid tileGrid)
    {
        List<Unit> list = new List<Unit>();
        for (int i = 0; i < UnitsContainer.childCount; i++)
        {
            var unit = UnitsContainer.GetChild(i).GetComponent<Unit>();
            if (unit != null)
            {
                unit.Initialize();
                list.Add(unit);
            }
            else
            {
                Debug.Log("Invalid Game Objects in Unit Container");
            }
        }
        return list;
    }

}

