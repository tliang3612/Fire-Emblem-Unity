using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.EventSystems;


public class OverlayTile : MonoBehaviour, IClickable
{

    // TileClicked event is invoked when user clicks on the tile. 
    public event EventHandler TileClicked;
    // TileHighlighed event is invoked when cursor enters the tile's collider. 
    public event EventHandler TileHighlighted;

    // TileDehighlighted event is invoked when cursor exits the tile's collider. 
    public event EventHandler TileDehighlighted;

    public int G;
    public int H;
    public int F { get { return G + H; } }

    public bool IsBlocked { get; set; }

    public OverlayTile previous;
    public Vector3Int gridLocation;
    public Vector2Int gridLocation2D { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    public Tilemap tileMap;
    public List<Sprite> arrowImages;
    public GameObject CursorSprite;

    public Unit CurrentUnit { get; set; }

    public void OnMouseEnter()
    {
        Debug.Log("Tile Clicked");
        if (TileHighlighted != null)
            TileHighlighted.Invoke(this, EventArgs.Empty);
        
    }

    public void OnMouseExit()
    {
       
        if (TileDehighlighted != null)
            TileDehighlighted.Invoke(this, EventArgs.Empty);
        
    }
    public void OnPointerDown()
    {
        Debug.Log("Tile Clicked");
        
        if (TileClicked != null)
            TileClicked.Invoke(this, EventArgs.Empty);
        
    }

    //Euclidean Distance (x,y) = sqrt((x1-x2)^2 + (y1-y2)^2)
    public int GetManhattenDistance(OverlayTile start, OverlayTile other)
    {
        return Mathf.Abs(start.gridLocation.x - other.gridLocation.x) + Mathf.Abs(start.gridLocation.y - other.gridLocation.y);
    }

    public virtual void MarkAsReachable()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
    }

    public virtual void MarkAsPath()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
    }
    public virtual void UnMark()
    {
        CursorSprite.GetComponent<SpriteRenderer>().color = Color.clear;
        gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
    }

    public virtual void MarkAsHighlighted()
    {
        CursorSprite.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
    }

    public virtual void MarkAsAttackableTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255,0,0,0.5f);
    }


    public void SetArrowSprite(ArrowTranslator.ArrowDirection direction)
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
            arrow.GetComponent<SpriteRenderer>().sortingOrder += 1;

        }
    }
}

