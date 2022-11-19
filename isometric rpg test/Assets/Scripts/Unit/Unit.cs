using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour, IClickable
{
    // UnitClicked event is invoked when user clicks the Unit. 
    public event EventHandler UnitClicked;

    // UnitSelected event is invoked when user clicks on Unit that belongs to him. 
    public event EventHandler UnitSelected;

    // UnitDeselected event is invoked when user click outside of currently selected Unit's collider.
    public event EventHandler UnitDeselected;

    // UnitHighlighted event is invoked when user moves cursor over the Unit. 
    public event EventHandler UnitHighlighted;

    // UnitDehighlighted event is invoked when cursor exits Unit's collider. 
    public event EventHandler UnitDehighlighted;

    // UnitDestroyed event is invoked when Unit's hitpoints drop below 0.
    public event EventHandler<AttackEventArgs> UnitDestroyed;

    // UnitMoved event is invoked when Unit moves from one tile to another.
    public event EventHandler<MovementEventArgs> UnitMoved;

    public UnitState UnitState { get; set; }
    public void SetState(UnitState state)
    {
        UnitState.TransitionState(state);
    }

    public OverlayTile Tile { get; set; }
    private Animator Anim;
    public float MovementAnimationSpeed = 7f;

    [SerializeField]
    private UnitInfo baseInfo;

    public int TotalHitPoints { get; private set; }
    public int TotalMovementPoints { get; private set; }
    public int TotalActionPoints { get; private set; }

    public int HitPoints { get; private set; }
    public int UnitAttack { get; private set; }
    public int UnitSkill { get; private set; }
    public int UnitSpeed { get; private set; }
    public int UnitLuck { get; private set; }
    public int UnitDefence { get; private set; }
    public int UnitConst { get; private set; }

    public string UnitName { get; private set; }
    public Sprite UnitPortrait { get; private set; }
    public Sprite UnitBattleSprite { get; private set; }
    public RuntimeAnimatorController BattleAnimController { get; private set; }

    public List<Item> Inventory { get; private set; }   

    [field: SerializeField]
    public int MovementPoints { get; private set; }
    public int ActionPoints { get; private set; }

    public int AttackRange
    {
        get
        {
            if (AvailableWeapons.Count > 0)
                return AvailableWeapons.Max(w => w.Range);
            else
                return 0;
        }
        private set { }
    }

    public List<Staff> AvailableStaffs
    {
        get{
            return Inventory.OfType<Staff>().ToList();
        }
        private set { }
    }
    public List<Weapon> AvailableWeapons
    {
        get
        {
            return Inventory.OfType<Weapon>().ToList();
        }
        private set { }
    }

    public Weapon EquippedWeapon
    {
        get
        {
            return AvailableWeapons.FirstOrDefault();
        }
        private set { }
    }

    public Staff EquippedStaff
    {
        get
        {
            return AvailableStaffs.FirstOrDefault();
        }
        private set { }
    }

    public bool Unarmed => AvailableWeapons.Count <= 0;

    private List<OverlayTile> cachedPath;

    public int PlayerNumber;
    public bool IsMoving { get; set; }

    private AStarPathfinder _pathfinder = new AStarPathfinder();
    private RangeFinder rangeFinder;

    //Initializes the Unit. Called whenever a Unit gets added into the game
    public virtual void Initialize()
    {
        UnitState = new UnitStateNormal(this);

        Anim = GetComponent<Animator>();
        rangeFinder = new RangeFinder();

        Tile = GetStartingTile();
        Tile.IsBlocked = true;
        Tile.CurrentUnit = this;

        cachedPath = new List<OverlayTile>();

        InitializeUnitInfo();

    }

    private void InitializeUnitInfo()
    {
        UnitName = baseInfo.Name;
        TotalHitPoints = baseInfo.TotalHitPoints;
        TotalActionPoints = baseInfo.TotalActionPoints;
        TotalMovementPoints = baseInfo.TotalMovementPoints;
        MovementPoints = baseInfo.TotalMovementPoints;
        ActionPoints = baseInfo.TotalActionPoints;

        HitPoints = baseInfo.TotalHitPoints-1;
        UnitAttack = baseInfo.BaseAttack;
        UnitDefence = baseInfo.BaseDefence;
        UnitLuck = baseInfo.BaseLuck;
        UnitSkill = baseInfo.BaseSkill;
        UnitConst = baseInfo.BaseConst;
        UnitPortrait = baseInfo.Portrait;
        UnitBattleSprite = baseInfo.BattleSprite;
        BattleAnimController = baseInfo.BattleAnimController;
        Inventory = baseInfo.StartingItems;
    }

    private OverlayTile GetStartingTile()
    {
        var tileGrid = FindObjectOfType<TileGrid>();
        var tileMap = tileGrid.Tilemap;

        var tilePos = new Vector2Int(tileMap.WorldToCell(transform.position).x, tileMap.WorldToCell(transform.position).y);

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

    }
    public void OnMouseExit()
    {
        if (UnitDehighlighted != null)
            UnitDehighlighted.Invoke(this, EventArgs.Empty);

    }

    //Called at the start of each turn
    public virtual void OnTurnStart()
    {
        MovementPoints = TotalMovementPoints;
        ActionPoints = TotalActionPoints;

        cachedPath = new List<OverlayTile>();
        Anim.SetBool("IsFinished", false);

    }

    //Called on the end of each turn
    public virtual void OnTurnEnd()
    {
        SetState(new UnitStateNormal(this));
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

    //Called when Unit is selected
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

    //Called when Unit is deselected
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
    /// Calculates whether the Unit is attackable
    /// </summary>
    /// <param name="otherUnit">Unit to attack</param>
    /// <param name="tile">Tile to perform an attack from</param>
    /// <returns>A boolean that determines if otherUnit is attackable </returns>
    public virtual bool IsUnitAttackable(Unit otherUnit, bool isWeaponBased)
    {
        return FindObjectOfType<TileGrid>().GetManhattenDistance(Tile, otherUnit.Tile) <= (isWeaponBased ? EquippedWeapon.Range : AttackRange)
            && otherUnit.PlayerNumber != PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0;
    }

    public bool IsUnitHealable(Unit otherUnit)
    {
        return FindObjectOfType<TileGrid>().GetManhattenDistance(Tile, otherUnit.Tile) <= EquippedStaff.Range
            && !otherUnit.Equals(this)
            && otherUnit.PlayerNumber == PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0
            && otherUnit.HitPoints < otherUnit.TotalHitPoints;

    }

    public void EquipItem(Item i)
    {
        if (Inventory.Contains(i))
        {
            Inventory.Remove(i);
            Inventory.Insert(0, i);
        }
        else
        {
            Debug.Log("Unit have contain that item");
        }
        
    }

    public virtual void ReceiveDamage(Unit source, int dmg)
    {
        HitPoints -= dmg;

        if (HitPoints <= 0)
        {
            HitPoints = 0;
            if (UnitDestroyed != null)
            {
                UnitDestroyed.Invoke(this, new AttackEventArgs(source, this, dmg));
            }
            OnDestroyed();
        }
    }

    public HealingDetails ReceiveHealing(int healAmount)
    {
        HitPoints = Mathf.Clamp(HitPoints + healAmount, 0, TotalHitPoints);

        return new HealingDetails(healAmount, HitPoints);
    }

    //Gets weapon effectiveness against a Unit. -1 for ineffective, 0 for neutral, and 1 for effective
    public virtual int GetEffectiveness(WeaponType other)
    {
        return EquippedWeapon.GetEffectiveness(other);
    }

    public int GetCritChance()
    {
        //Weapon Critical + (Skill / 2)
        return Mathf.Clamp(EquippedWeapon.Crit + (UnitSkill / 2), 0, 100);
    }

    public int GetHitChance(WeaponType other)
    {
        //Accuracy formula = Weapon Hit + (Skill x 2) + (Luck / 2) + Weapon Triangle bonus * 15
        return EquippedWeapon.Hit + (UnitSkill * 2) + (UnitLuck / 2) + EquippedWeapon.GetEffectiveness(other) * 15;
    }

    public int GetAttack(WeaponType other)
    {
        //Strength + Weapon Might + Weapon Triangle bonus
        return UnitAttack + EquippedWeapon.Attack + EquippedWeapon.GetEffectiveness(other);
    }

    public WeaponPreviewStats GetPreviewWeaponStats(Weapon w)
    {
        return new WeaponPreviewStats(UnitAttack + w.Attack, //Attack
            w.Hit + (UnitSkill * 2) + (UnitLuck / 2), //HitChance
            Mathf.Clamp(w.Crit + (UnitSkill / 2), 0, 100), //CritChance
            (UnitSpeed - (EquippedWeapon.Weight - UnitConst))*2 + UnitLuck); //DodgeChance
    }
    

    public int GetDefense()
    {
        return UnitDefence + Tile.DefenseBoost;
    }

    public int GetDodgeChance() 
    {
        //(Attack Speed x 2) + Luck + TerrainBoost
        return (GetAttackSpeed() * 2) + UnitLuck + Tile.AvoidBoost;
    }

    public int GetAttackSpeed()
    {
        return Mathf.Clamp(UnitSpeed - (EquippedWeapon.Weight - UnitConst), 0, 100);
    }

    public void Move(List<OverlayTile> path)
    {
        if (MovementAnimationSpeed > 0 && path.Count > 1)
        {
            cachedPath = path;
            StartCoroutine(MovementAnimation(path));
        }
    }

    /// <summary>
    /// Procedurally moves Unit along the path
    /// </summary>
    /// <param name="path"> List of tiles the Unit will move through </param>
    protected virtual IEnumerator MovementAnimation(List<OverlayTile> path)
    {
        Anim.SetBool("IsMoving", true);

        if (path.Count <= 1)
        {
            yield return null;
        }

        List<OverlayTile> tempPath = path.ToList();

        IsMoving = true;

        while (tempPath.Count > 0 && IsMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, tempPath[0].transform.position, Time.deltaTime * MovementAnimationSpeed);

            var direction = GetDirectionToFace(tempPath[0].transform.position);

            //this prevents blend tree parameters from ever going to (0,0)
            if (direction.x != direction.y)
            {
                Anim.SetFloat("MoveX", direction.x);
                Anim.SetFloat("MoveY", direction.y);

            }

            if (Vector2.Distance(transform.position, tempPath[0].transform.position) < Mathf.Epsilon)
            {
                PositionCharacter(tempPath[0]);
                tempPath.RemoveAt(0);
            }

            yield return 0;
        }
        IsMoving = false;
    }

    public Vector2Int GetDirectionToFace(Vector3 headingPosition)
    {
        var heading = headingPosition - transform.position;
        var distance = heading.magnitude;

        return new Vector2Int((int)(heading / distance).normalized.x, (int)(heading / distance).normalized.y);
    }

    public void ConfirmMove()
    {
        if (cachedPath.Count > 0)
        {
            foreach (var tile in cachedPath)
            {
                MovementPoints -= tile.MovementCost;
            }

            if (UnitMoved != null)
            {
                UnitMoved.Invoke(this, new MovementEventArgs(cachedPath[0], cachedPath[cachedPath.Count - 1], cachedPath));
            }

            MovementPoints = 0;
        }
    }

    public void SetAnimationToIdle()
    {
        Anim.SetBool("IsMoving", false);
        Anim.Play("Idle", 0, FindObjectOfType<AnimationTimer>().GetCurrentCurrentTime());
    }

    //used for whenver we want to start the Unit's movement animation
    public void SetMove(Vector2Int direction = default)
    {
        Anim.SetBool("IsMoving", true);

        //start move down animation
        if(direction == default)
        {
            Anim.SetFloat("MoveX", 0);
            Anim.SetFloat("MoveY", -1);
        }
        else
        {
            Anim.SetFloat("MoveX", direction.x);
            Anim.SetFloat("MoveY", direction.y);
        }       
    }

    public void ResetMove()
    {
        SetState(new UnitStateNormal(this));

        if (cachedPath.Count > 0)
        {
            MovementPoints = TotalMovementPoints;
            PositionCharacter(cachedPath[0]);
            cachedPath = new List<OverlayTile>();
        }
    }

    public void SetFinished()
    {
        Anim.SetBool("IsMoving", false);
        Anim.SetBool("IsFinished", true);
        MovementPoints = 0;
        ActionPoints = 0;
    }

    /// <summary>
    /// Positions the Unit at the given tile
    /// </summary>
    /// <param name="tile"> Tile to position Unit on</param>
    public void PositionCharacter(OverlayTile tile)
    {
        transform.position = tile.transform.position;
        Tile = tile;
    }

    public bool IsTileMovableTo(OverlayTile tile)
    {
        if (Tile == tile)
        {
            return true;
        }
        return !tile.IsBlocked;
    }

    //Get a list of tiles that the Unit can move to
    public List<OverlayTile> GetAvailableDestinations(TileGrid tileGrid)
    {
        return rangeFinder.GetTilesInMoveRange(this, tileGrid, GetTilesInRange(tileGrid, MovementPoints));
    }

    //Get a list of tiles within the Unit's range. Doesn't take tile cost into consideration
    public List<OverlayTile> GetTilesInRange(TileGrid tileGrid, int range)
    {
        return rangeFinder.GetTilesInRange(this, tileGrid, range);
    }

    //Get a list of attackable tiles that doesn't include the tiles that a Unit can move to
    public List<OverlayTile> GetTilesInAttackRange(List<OverlayTile> availableDestinations, TileGrid tileGrid)
    {
        return rangeFinder.GetTilesInAttackRange(availableDestinations, tileGrid, AttackRange);
    }

    //Find the optimal path from the tile the Unit is on currently, to the destination tile
    public List<OverlayTile> FindPath(OverlayTile destination, TileGrid tileGrid)
    {
        return _pathfinder.FindPath(Tile, destination, GetAvailableDestinations(tileGrid), tileGrid);
    }

    //Visual indication that the Unit is destroyed
    public virtual void MarkAsDestroyed()
    {
        GetComponent<SpriteRenderer>().color = Color.black;
        Destroy(gameObject);
    }

    //Visual indication that the Unit has no more moves this turn
    public virtual void MarkAsFinished()
    {
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    public virtual void MarkAsEnemy(Player player)
    {
        GetComponent<SpriteRenderer>().color = player.Color;
    }

    //Return the Unit back to its original appearance
    public virtual void UnMark()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
//End of Unit class

public struct WeaponPreviewStats
{
    public string WeaponAttack;
    public string WeaponHit;
    public string WeaponCrit;
    public string WeaponAvoid;

    public WeaponPreviewStats(int attack, int hit, int crit, int avoid) : this()
    {
        WeaponAttack = attack.ToString();
        WeaponHit = hit.ToString();
        WeaponCrit = crit.ToString();
        WeaponAvoid = avoid.ToString();
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
    public Unit Unit;
    public List<Ability> Abilities;

    public UnitCreatedEventArgs(Unit unit, List<Ability> unitAbilities)
    {
        Unit = unit;
        Abilities = unitAbilities;
    }
}

