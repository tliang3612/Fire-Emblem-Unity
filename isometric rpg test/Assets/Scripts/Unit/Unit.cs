using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IClickable
{
    // UnitClicked event is invoked when user clicks the unit. 
    public event EventHandler UnitClicked;

    // UnitSelected event is invoked when user clicks on unit that belongs to him. 
    public event EventHandler UnitSelected;

    // UnitDeselected event is invoked when user click outside of currently selected unit's collider.
    public event EventHandler UnitDeselected;
 
    // UnitHighlighted event is invoked when user moves cursor over the unit. 
    public event EventHandler UnitHighlighted;
       
    // UnitDehighlighted event is invoked when cursor exits unit's collider. 
    public event EventHandler UnitDehighlighted;

    // UnitAttacked event is invoked when the unit is attacked.
    public event EventHandler<AttackEventArgs> UnitAttacked;

    // UnitDestroyed event is invoked when unit's hitpoints drop below 0.
    public event EventHandler<AttackEventArgs> UnitDestroyed;

    // UnitMoved event is invoked when unit moves from one tile to another.
    public event EventHandler<MovementEventArgs> UnitMoved;

    public bool Obstructable = true;

    public UnitState UnitState { get; set; }
    public void SetState(UnitState state)
    {
        UnitState.TransitionState(state);
    }

    public int TotalHitPoints { get; private set; }
    public int TotalMovementPoints { get; private set; }
    public int TotalActionPoints { get; private set; }

    [SerializeField]
    [HideInInspector]
    public OverlayTile Tile { get; set; }
    public float MovementAnimationSpeed { get; private set; }

    public int HitPoints;
    public int AttackRange;
    public int AttackFactor;
    public int DefenceFactor;
    public string UnitName;
        
    public Image HealthBar;

    [SerializeField]
    private int movementPoints;
    public int MovementPoints
    {
        get { return movementPoints; }
        set { movementPoints = value; }
    }

    [SerializeField]
    private int actionPoints = 1;
    public int ActionPoints
    {
        get { return actionPoints; }
        set { actionPoints = value; }
    }

    public int PlayerNumber;
    public bool IsMoving { get; set; }

    private AStarPathfinder _pathfinder = new AStarPathfinder();
    private List<OverlayTile> tilesInRange;
    private RangeFinder rangeFinder;
    private AttackAnimation attackAnimation;

    //Initializes the unit. Called whenever a unit gets added into the game
    public virtual void Initialize()
    {
        UnitState = new UnitStateNormal(this);
        MovementAnimationSpeed = 7f;

        GetComponent<SpriteRenderer>().sortingOrder = 40;

        TotalHitPoints = HitPoints;
        TotalMovementPoints = MovementPoints;
        TotalActionPoints = ActionPoints;

        attackAnimation = GetComponent<AttackAnimation>();
        rangeFinder = new RangeFinder();

        if (!Tile)
        {
            Tile = GetStartingTile();
            Tile.IsBlocked = true;
        }

        tilesInRange = GetAvailableDestinations(FindObjectOfType<TileGrid>());
    }

    private OverlayTile GetStartingTile()
    {
        var tileGrid = FindObjectOfType<TileGrid>();
        var tileMap = tileGrid.tilemapArr[0];

        var tilePos = new Vector2Int(tileMap.WorldToCell(transform.position).x, tileMap.WorldToCell(transform.position).y);

        Debug.Log(tilePos);
        if (tileGrid.Map.TryGetValue(tilePos, out OverlayTile tile))
        {
            return tile;
        }
        return null;
    }
        
    public void OnPointerDown()
    {
        if (UnitClicked != null)
            UnitClicked.Invoke(this, EventArgs.Empty);
    }

    public void OnMouseEnter()
    {
            
        if (UnitHighlighted != null)
            UnitHighlighted.Invoke(this, EventArgs.Empty);
            
        Tile.MarkAsHighlighted();

    }
    public void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (UnitDehighlighted != null)
                UnitDehighlighted.Invoke(this, EventArgs.Empty);
        }
        Tile.UnMark();
    }

    //Called at the start of each turn
    public virtual void OnTurnStart()
    {
        tilesInRange.Clear();

        var name = this.name;
        var state = UnitState;

        if(HitPoints > 0)
        {
            SetState(new UnitStateFriendly(this));
        }     
    }

    //Called on the end of each turn
    public virtual void OnTurnEnd()
    {
        if (HitPoints > 0)
        {
            MovementPoints = TotalMovementPoints;
            ActionPoints = TotalActionPoints;

            SetState(new UnitStateNormal(this));
        }
        else
        {
            OnDestroyed();
        }
    }
        
    //Called when Unit is dead
    protected virtual void OnDestroyed()
    {
        TotalMovementPoints = 0;
        TotalActionPoints = 0;
        Tile.IsBlocked = false;
        Tile.CurrentUnit = null;
        SetState(new UnitStateDestroyed(this));
            
    }

    //Called when unit is selected
    public virtual void OnUnitSelected()
    {
        if (FindObjectOfType<TileGrid>().GetCurrentPlayerUnits().Contains(this))
        {
            SetState(new UnitStateSelected(this));
        }
        if (UnitSelected != null)
        {
            UnitSelected.Invoke(this, EventArgs.Empty);
        }
    }

    //Called when unit is deselected
    public virtual void OnUnitDeselected()
    {
        if (FindObjectOfType<TileGrid>().GetCurrentPlayerUnits().Contains(this))
        {
            SetState(new UnitStateFriendly(this));
        }
        if (UnitDeselected != null)
        {
            UnitDeselected.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Calculates whether the unit is attackable
    /// </summary>
    /// <param name="otherUnit">Unit to attack</param>
    /// <param name="tile">Tile to perform an attack from</param>
    /// <returns>A boolean that determines if otherUnit is attackable </returns>
    public virtual bool IsUnitAttackable(OverlayTile tile, Unit otherUnit)
    {
        return tile.GetManhattenDistance(tile, otherUnit.Tile) <= AttackRange
            && otherUnit.PlayerNumber != PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0;
    }

    /// <summary>
    /// Handles the attack event against the other unit
    /// </summary>
    /// <param name="unitToAttack"></param>
    public void AttackHandler(Unit unitToAttack)
    {
        AttackAction attackAction = GetAttackAndCost(unitToAttack);
        MarkAsAttacking(unitToAttack);
        unitToAttack.DefendHandler(this, attackAction.Damage);
        ActionPoints -= attackAction.ActionCost;
    }

    /// <summary>
    /// Gets damage given to the other unit and action point cost of the action
    /// </summary>
    /// <param name="unitToAttack"> The unit under attack </param>
    /// <returns>An AttackAction that contains the damage given and action cost taken </returns>
    protected virtual AttackAction GetAttackAndCost(Unit unitToAttack)
    {
        return new AttackAction(AttackFactor, 1);
    }

    /// <summary>
    /// Handles the defend event of the unit being attacked
    /// </summary>
    /// <param name="aggressor"> Unit that is attacking </param>
    /// <param name="damage"> Damage given by the other unit </param>
    public void DefendHandler(Unit aggressor, int damage)
    {
            
        int damageTaken = CalculateDamageTaken(aggressor, damage);
        HitPoints -= damageTaken;
            
        if (UnitAttacked != null)
        {
            UnitAttacked.Invoke(this, new AttackEventArgs(aggressor, this, damage));
        }
        if (HitPoints <= 0)
        {
            HitPoints = 0;          
            if (UnitDestroyed != null)
            {
                UnitDestroyed.Invoke(this, new AttackEventArgs(aggressor, this, damage));
            }
            OnDestroyed();
        }

        UpdateHpBar();

    }

    //Called whenever the unit's health changes
    private void UpdateHpBar()
    {   
        /*
         
        if (HealthBar != null)
        {
            HealthBar.transform.localScale = new Vector3((float)(HitPoints / (float)TotalHitPoints), 1, 1);
            HealthBar.color = Color.Lerp(Color.red, Color.black,
                (float)(HitPoints / (float)TotalHitPoints));
        }
        */
    }

    /// <summary>
    /// Calculates actual damage given
    /// </summary>
    /// <param name="aggressor">Unit that performed the attack</param>
    /// <param name="rawDamage">Raw damage that the attack caused</param>
    /// <returns>Actual damage the unit has taken</returns>        
    protected int CalculateDamageTaken(Unit aggressor, int rawDamage)
    {
        return (rawDamage - DefenceFactor);
    }

    /// <summary>
    /// Handles the movement event of the unit
    /// </summary>
    /// <param name="destinationTile">The tile the unit is moving to</param>
    /// <param name="path"> List of tiles that the unit will move through</param>
    public virtual void Move(OverlayTile destinationTile, List<OverlayTile> path)
    {

        MovementPoints -= path.Count();

        if (MovementAnimationSpeed > 0)
        {
            StartCoroutine(MovementAnimation(path));
        }
        else
        {
            OnMoveFinished(destinationTile);
        }

        if (UnitMoved != null)
        {
            UnitMoved.Invoke(this, new MovementEventArgs(Tile, destinationTile, path));             
        }
    }

    /// <summary>
    /// Procedurally moves unit along the path
    /// </summary>
    /// <param name="path"> List of tiles the unit will move through </param>
    protected virtual IEnumerator MovementAnimation(List<OverlayTile> path)
    {
        var tempPath = path;
        IsMoving = true;

        while (tempPath.Count > 0 && IsMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, tempPath[0].transform.position, Time.deltaTime * MovementAnimationSpeed);

            if (Vector2.Distance(transform.position, tempPath[0].transform.position) < 0.001f)
            {
                PositionCharacter(tempPath[0]);
                tempPath.RemoveAt(0);
            }
                
            yield return 0;
        }
        IsMoving = false;
        OnMoveFinished(Tile);
    }

    //Called when movement animation terminates
    protected virtual void OnMoveFinished(OverlayTile tile)
    {        
        PositionCharacter(tile);
    }

    /// <summary>
    /// Positions the unit at the given tile
    /// </summary>
    /// <param name="tile"> Tile to position unit on</param>
    private void PositionCharacter(OverlayTile tile)
    {
        transform.position = tile.transform.position;
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        Tile = tile;
    }

    public bool IsTileMovableTo(OverlayTile tile)
    {
        return !tile.IsBlocked;
    }

    //Get a list of tiles that the unit can move to
    public List<OverlayTile> GetAvailableDestinations(TileGrid tileGrid)
    {
        var inRangeTiles = GetTilesInRange(tileGrid, Tile, MovementPoints);

        return inRangeTiles;
    }

    //Get a list of tiles around the given tile given the range
    public List<OverlayTile> GetTilesInRange(TileGrid tileGrid, OverlayTile tile, int range)
    {
        return rangeFinder.GetTilesInRange(tileGrid, tile, range);
    }

    //Get a list of tiles within a set range
    public List<OverlayTile> GetTilesInSetRange(TileGrid tileGrid, OverlayTile tile, int rangeMin, int rangeMax)
    {
        return rangeFinder.GetTilesInSetRange(tileGrid, tile, rangeMin, rangeMax);
    }

    //Find the optimal path from the tile the unit is on currently, to the destination tile
    public List<OverlayTile> FindPath(OverlayTile destination, TileGrid tileGrid)
    {
        return _pathfinder.FindPath(Tile, destination, tilesInRange, tileGrid);
    }

    //Visual indication that the unit is under attack
    public virtual void MarkAsUnderAttack()
    {
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    /// <summary>
    /// Visual indication that the unit is starting an attack
    /// </summary>
    /// <param name="target"> Unit that is under attack </param>
    public virtual void MarkAsAttacking(Unit target)
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        attackAnimation.StartAttackAnimation(this, target);
    }

    //Visual indication that the unit is destroyed
    public virtual void MarkAsDestroyed()
    {
        GetComponent<SpriteRenderer>().color = Color.black;
    }

    //Visual indication that the unit is part of the current player's units
    public virtual void MarkAsFriendly()
    {
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    //Visual indication that the unit marked is within attack range
    public virtual void MarkAsReachableEnemy()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    //Visual indication that the current unit as selected
    public virtual void MarkAsSelected()
    {
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    //Visual indication that the unit has no more moves this turn
    public virtual void MarkAsFinished()
    {
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    //Return the unit back to its original appearance
    public virtual void UnMark()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
//End of unit class
    
public class AttackAction
{
    public readonly int Damage;
    public readonly int ActionCost;

    public AttackAction(int damage, int actionCost)
    {
        Damage = damage;
        ActionCost = actionCost;
    }
}

public class MovementEventArgs : EventArgs
{
    public OverlayTile StartingTile;
    public OverlayTile DestinationTile;
    public List<OverlayTile> Path;

    public MovementEventArgs(OverlayTile startingTile, OverlayTile destinationTile, List<OverlayTile> path)
    {
        StartingTile = startingTile;
        DestinationTile = destinationTile;
        Path = path;
    }
}
public class AttackEventArgs : EventArgs
{
    public Unit Attacker;
    public Unit Defender;

    public int Damage;

    public AttackEventArgs(Unit attacker, Unit defender, int damage)
    {
        Attacker = attacker;
        Defender = defender;

        Damage = damage;
    }
}

public class UnitCreatedEventArgs : EventArgs
{
    public Unit unit;

    public UnitCreatedEventArgs(Unit unit)
    {
        this.unit = unit;
    }
}

