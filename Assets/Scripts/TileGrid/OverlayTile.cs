using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Linq;

public class OverlayTile : MonoBehaviour, IClickable
{
    //directions the tile can scan for neighbors
    private readonly Vector2Int[] neighborDirections = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

    // TileClicked event is invoked when user clicks on the tile. 
    public event EventHandler TileClicked;
    // TileHighlighed event is invoked when cursor enters the tile's collider. 
    public event EventHandler TileHighlighted;
    // TileDehighlighted event is invoked when cursor exits the tile's collider. 
    public event EventHandler TileDehighlighted;

    public bool IsOccupied { get; set; }

    public Vector3Int gridLocation { get; private set; }
    public Vector2Int gridLocation2D { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }
    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private GameObject CursorSprite;
    [SerializeField] private Sprite _blueTile;
    [SerializeField] private Sprite _redTile;
    [SerializeField] private Sprite _greenTile;

    public string TileName { get; private set; }
    public int DefenseBoost { get; private set; }
    public int AvoidBoost { get; private set; }
    public Unit CurrentUnit { get; set; }

    private Dictionary<UnitType, int> _costBasedOnUnit;
    private List<OverlayTile> _neighbors;

    public List<Sprite> arrowImages;

    private void Awake()
    {
        _costBasedOnUnit = new Dictionary<UnitType, int>();
        _neighbors = null;
    }

    public void OnMouseEnter()
    {
        if (TileHighlighted != null)
            TileHighlighted.Invoke(this, EventArgs.Empty);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            
        }
    }

    public void OnMouseExit()
    {
        if (TileDehighlighted != null)
            TileDehighlighted.Invoke(this, EventArgs.Empty);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            
        }
    }

    public void OnPointerDown()
    {
        if (TileClicked != null)
            TileClicked.Invoke(this, EventArgs.Empty);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            HideCursor();
        }
    }

    public void InitializeTile(Tilemap tilemap, Vector3Int tileLocation, Dictionary<TileBase, TileData> tileDataMap, TileGrid tileGrid)
    {
        //Make sure the overlay tile appears ontop of the tilemap
        GetComponent<SpriteRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;

        gridLocation = tileLocation;

        var cellWorldPosition = tilemap.GetCellCenterWorld(tileLocation);
        transform.position = new Vector2(cellWorldPosition.x, cellWorldPosition.y);

        //If the tilemap contains a TileBase at the given tileLocation
        if(tileDataMap.TryGetValue(tilemap.GetTile(tileLocation), out TileData val)){
            TileName = val.TileName;

            _costBasedOnUnit = val.CostBasedOnUnit;
            DefenseBoost = val.DefenseBoost;
            AvoidBoost = val.AvoidBoost;
        }
        else if(tilemap.GetTile(tileLocation) != null)
        {
            var tileData = tileGrid.TileDataList.Where(x => x.TileName == "Plains").First();

            _costBasedOnUnit = tileData.CostBasedOnUnit;

            TileName = tileData.TileName;
            DefenseBoost = tileData.DefenseBoost;
            AvoidBoost = tileData.AvoidBoost;
        }
        else
        {
            Debug.Log("No TileBase at postion " + tileLocation);
        }
    }

    public int GetMovementCost(UnitType type)
    {
        return _costBasedOnUnit[type];
    }

    //A unit can move to a tile if the cost based on unit is greater than 0
    public bool CanUnitMoveTo(UnitType type)
    {
        return _costBasedOnUnit[type] > 0;
    }

    public virtual void MarkAsReachable()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _blueTile;
    }

    public virtual void UnMark()
    {
        HideCursor();
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }

    public virtual void MarkAsHighlighted()
    {
        ShowCursor();
    }

    public virtual void MarkAsDeHighlighted()
    {
        HideCursor();
    }

    public virtual void MarkAsAttackableTile()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _redTile;       
    }

    public virtual void MarkAsHealableTile()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _greenTile;
    }

    public virtual void MarkArrowPath(ArrowTranslator.ArrowDirection direction)
    {
        var arrow = GetComponentsInChildren<SpriteRenderer>()[1];
        if (direction == ArrowTranslator.ArrowDirection.None)
        {
            arrow.color = new Color(1, 1, 1, 0);
        }
        else
        {
            arrow.color = new Color(1, 1, 1, 1);
            arrow.sprite = arrowImages[(int)direction];

        }
    }

    //Cursor stops animating when on a Unit
    public void HighlightedOnUnit()
    {
        CursorSprite.GetComponent<Animator>().SetBool("IsActive", false);

        //increase the size of the cursor when highlighted on a unit
        CursorSprite.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        CursorSprite.GetComponent<SpriteRenderer>().color = Color.white;       
    }

    public void DeHighlightedOnUnit()
    {
        CursorSprite.transform.localScale = Vector3.one;
        HideCursor();
    }

    //Animates Cursor and makes the cursor on the tile visible
    public void ShowCursor()
    {
        CursorSprite.GetComponent<Animator>().SetBool("IsActive", true);
        CursorSprite.GetComponent<SpriteRenderer>().color = Color.white;      
    }

    //Stops Cursor animation and make it invisible
    public void HideCursor()
    {
        CursorSprite.GetComponent<Animator>().SetBool("IsActive", false);
        CursorSprite.GetComponent<SpriteRenderer>().color = Color.clear; //Make cursor invisible
    }


    public List<OverlayTile> GetNeighborTiles(TileGrid tileGrid)
    {

        if(_neighbors == null)
        {
            var tilesToSearch = tileGrid.Map;

            _neighbors = new List<OverlayTile>(4);

            foreach(Vector2Int direction in neighborDirections)
            {
                var locationToCheck = new Vector2Int(gridLocation2D.x + direction.x, gridLocation.y + direction.y);
                if(tilesToSearch.ContainsKey(locationToCheck))
                {
                    _neighbors.Add(tilesToSearch[locationToCheck]);
                }
            }
        }

        return _neighbors;

    }
}

