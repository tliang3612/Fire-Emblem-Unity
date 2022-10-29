using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Linq;

public class OverlayTile : MonoBehaviour, IClickable
{

    // TileClicked event is invoked when user clicks on the tile. 
    public event EventHandler TileClicked;
    // TileHighlighed event is invoked when cursor enters the tile's collider. 
    public event EventHandler TileHighlighted;
    // TileDehighlighted event is invoked when cursor exits the tile's collider. 
    public event EventHandler TileDehighlighted;


    public bool IsBlocked { get; set; }

    public Vector3Int gridLocation;
    public Vector2Int gridLocation2D { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public Tilemap tileMap;
    public List<Sprite> arrowImages;
    public GameObject CursorSprite;
    public Sprite blueTile;
    public Sprite redTile;

    public string TileName;
    public int MovementCost;
    public int DefenseBoost;
    public int AvoidBoost;

    public int MovementRating;

    public Unit CurrentUnit { get; set; }

    public void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (TileHighlighted != null)
                TileHighlighted.Invoke(this, EventArgs.Empty);
        }
        else
        {
            HideCursor();
        }
    }

    public void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (TileDehighlighted != null)
                TileDehighlighted.Invoke(this, EventArgs.Empty);
        }
        else
        {
            HideCursor();
        }
    }

    public void OnPointerDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (TileClicked != null)
                TileClicked.Invoke(this, EventArgs.Empty);
        }
        else
        {
            HideCursor();
        }
    }

    public void InitializeTile(Tilemap tilemap, Vector3Int tileLocation, Dictionary<TileBase, TileData> tileDataMap)
    {

        //Make sure the overlay tile appears ontop of the tilemap
        GetComponent<SpriteRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;

        gridLocation = tileLocation;
        tileMap = tilemap;


        var cellWorldPosition = tilemap.GetCellCenterWorld(tileLocation);
        transform.position = new Vector2(cellWorldPosition.x, cellWorldPosition.y);

        //If the tilemap contains a TileBase at the given tileLocation
        if(tileDataMap.TryGetValue(tilemap.GetTile(tileLocation), out TileData val)){
            TileName = val.TileName;

            if (val.MovementCost >= 10)
                IsBlocked = true;
            MovementCost = val.MovementCost;
            DefenseBoost = val.DefenseBoost;
            AvoidBoost = val.AvoidBoost;
        }
        else if(tilemap.GetTile(tileLocation) != null)
        {
            var tileData = FindObjectOfType<TileGrid>().TileDataList.Where(x => x.TileName == "Plains").First();

            TileName = tileData.TileName;
            MovementCost = tileData.MovementCost;
            DefenseBoost = tileData.DefenseBoost;
            AvoidBoost = tileData.AvoidBoost;
        }
        else
        {
            Debug.Log("No TileBase at postion " + tileLocation);
        }
    }

    public virtual void MarkAsReachable()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = blueTile;
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
        gameObject.GetComponent<SpriteRenderer>().sprite = redTile;       
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
        CursorSprite.GetComponent<SpriteRenderer>().color = Color.white;       
    }

    public void DeHighlightedOnUnit()
    {
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
        var tilesToSearch = tileGrid.Map;

        List<OverlayTile> neighbors = new List<OverlayTile>();

        //checks left and right neighbors
        for (int i = 1; i >= -1; i -= 2)
        {
            var locationToCheck = new Vector2Int(gridLocation.x + i, gridLocation.y);
            if (tilesToSearch.ContainsKey(locationToCheck))
            {
                neighbors.Add(tilesToSearch[locationToCheck]);
            }
        }

        //checks top and down neighbors
        for (int i = 1; i >= -1; i -= 2)
        {
            var locationToCheck = new Vector2Int(gridLocation.x, gridLocation.y + i);
            if (tilesToSearch.ContainsKey(locationToCheck))
            {
                neighbors.Add(tilesToSearch[locationToCheck]);
            }
        }

        return neighbors;
    }
}

