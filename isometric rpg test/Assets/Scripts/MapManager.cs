using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{

    private static MapManager _instance;

    //instance is immutable
    public static MapManager Instance 
    { 
        get { return _instance;} 
    }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    //map of the tile's location
    public Dictionary<Vector2Int, OverlayTile> map;

    private void Awake()
    {
        //Initializes _instance if it doesn't exist, else destroy it
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        var tileMap = gameObject.GetComponentsInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();
        SetUpOverlayTiles(tileMap[0]);
        SetUpOverlayTiles(tileMap[1]);
        SetUpOverlayTiles(tileMap[2]);
        SetUpOverlayTiles(tileMap[3]);

    }

    public void SetUpOverlayTiles(Tilemap tileMap)
    {
        //limits of the current tilemap
        BoundsInt bounds = tileMap.cellBounds;
        //loops through all the tiles of the tilemap, from top to bottom
        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);
                    var tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder + 1;

                        overlayTile.gridLocation = tileLocation;
                        overlayTile.tileMap = tileMap;

                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }

    public List<OverlayTile> GetNeighborTiles(OverlayTile tile, List<OverlayTile> searchableTiles)
    {
        var map = MapManager.Instance.map;

        Dictionary<Vector2Int, OverlayTile> tilesToSearch = new Dictionary<Vector2Int, OverlayTile>();
        if(searchableTiles.Count > 0)
        {
            foreach(var item in searchableTiles)
            {
                if (!tilesToSearch.ContainsKey(item.gridLocation2D))
                    tilesToSearch.Add(item.gridLocation2D, item);
            }
        }
        else
        {
            tilesToSearch = map;
        }
        
        List<OverlayTile> neighbors = new List<OverlayTile>();

        
        Vector2Int locationToCheck = new Vector2Int();

        //checks left and right neighbors
        for(int i=1; i >=-1; i-=2)
        {
            locationToCheck = new Vector2Int(tile.gridLocation.x + i, tile.gridLocation.y);
            if (tilesToSearch.ContainsKey(locationToCheck))
            {
                var val = tilesToSearch.FirstOrDefault(x => x.Key == locationToCheck).Value;
                if (val.tileMap.GetCellCenterWorld(val.gridLocation).z >= 0)
                {
                    neighbors.Add(tilesToSearch[locationToCheck]);
                }
            }
        }

        //checks top and down neighbors
        for (int i = 1; i >= -1; i -= 2)
        {
            locationToCheck = new Vector2Int(tile.gridLocation.x, tile.gridLocation.y + i);
            if (tilesToSearch.ContainsKey(locationToCheck))
            {
                var val = tilesToSearch.FirstOrDefault(x => x.Key == locationToCheck).Value;
                if (val.tileMap.GetCellCenterWorld(val.gridLocation).z >= 0)
                {
                    neighbors.Add(tilesToSearch[locationToCheck]);
                }
            }
        }

        return neighbors;
    }

    void Update()
    {
        
    }
}

