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

            //a workaround around a bug that sets the unit's z pos to 1
            unit.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y, -1);
        }
        return list;
    }

}

