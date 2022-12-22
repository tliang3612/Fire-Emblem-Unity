using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Diagnostics;

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

    private Animator _anim;

    [field: SerializeField]
    public OverlayTile Tile { get; set; }
    
    [SerializeField] private float MovementAnimationSpeed = 6f;
    [SerializeField] private Vector2Int _defaultDirection;
    [SerializeField] private UnitInfo baseInfo;

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
    public UnitType UnitType { get; private set; }

    public string UnitName { get; private set; }
    public Sprite UnitPortrait { get; private set; }
    public Sprite UnitMapSprite { get; private set; }
    public Sprite UnitMugshot { get; private set; }

    public RuntimeAnimatorController BattleAnimController { get; private set; }

    public List<Item> Inventory { get; private set; }   

    [field: SerializeField]
    public int MovementPoints { get; private set; }
    public int ActionPoints { get; private set; }

    public bool DeathAnimationPlaying { get; set; }

    public int AttackRange
    {
        get
        {
            if (AvailableWeapons.Count > 0)
                return AvailableWeapons.Max(w => w.Range);
            else
                return 0;
        }
    }

    public List<Staff> AvailableStaffs
    {
        get {return Inventory.OfType<Staff>().ToList();}
        private set { }
    }
    public List<Weapon> AvailableWeapons
    {
        get {return Inventory.OfType<Weapon>().ToList();}
        private set { }
    }

    public Weapon EquippedWeapon
    {
        get {return AvailableWeapons.FirstOrDefault();}
    }

    public Staff EquippedStaff
    {
        get {return AvailableStaffs.FirstOrDefault();}
    }

    public bool Unarmed { get { return AvailableWeapons.Count <= 0; } }


    private List<OverlayTile> _storedPath;
    public OverlayTile PreviousTile { 
        get {
            if (_storedPath.Count > 0)
            {
                return _storedPath[0];
            }
                
            return null; 
        } 
    }
    private Dictionary<OverlayTile, List<OverlayTile>> _cachedPaths = null;

    [field: SerializeField]
    public int PlayerNumber { get; set; }
    public Player Player { get; set; }
    public bool IsMoving { get; set; }

    private AStarPathfinder _pathfinder;
    private RangeFinder _rangeFinder; 

    public void Awake()
    {
        _anim = GetComponent<Animator>();
        _rangeFinder = new RangeFinder(this);
        _pathfinder = new AStarPathfinder(this);
        _storedPath = new List<OverlayTile>();
    }

    //Initializes the Unit. Called whenever a Unit gets added into the game
    public virtual void Initialize()
    {     
        Tile = GetStartingTile();
        Tile.IsOccupied = true;
        Tile.CurrentUnit = this;

        UnitState = new UnitStateNormal(this);

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
        UnitSpeed = baseInfo.BaseSpeed;
        UnitType = baseInfo.UnitType;

        UnitPortrait = baseInfo.Portrait;
        UnitMapSprite = baseInfo.MapSprite;
        UnitMugshot = baseInfo.Mugshot;

        BattleAnimController = baseInfo.BattleAnimController;
        Inventory = baseInfo.StartingItems;
    }

    public void SetState(UnitState state)
    {
        UnitState.OnStateExit();
        UnitState.TransitionState(state);
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

        _cachedPaths = null;
        _storedPath = new List<OverlayTile>();
        _anim.SetBool("IsFinished", false);

    }

    //Called on the end of each turn
    public virtual void OnTurnEnd()
    {
        SetState(new UnitStateNormal(this));
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
        if (UnitDeselected != null)
        {
            UnitDeselected.Invoke(this, EventArgs.Empty);
        }
    }

    //check if the other unit is attackable
    public virtual bool IsUnitAttackable(Unit otherUnit, bool isWeaponBased)
    {
        return FindObjectOfType<TileGrid>().GetManhattenDistance(Tile, otherUnit.Tile) <= (isWeaponBased ? EquippedWeapon.Range : AttackRange)
            && otherUnit.PlayerNumber != PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0;
    }

    //used to see if a unit can be attacked from a chosen tile
    public virtual bool IsTileAttackableFrom(OverlayTile tile, Unit otherUnit, bool isWeaponBased)
    {
        return FindObjectOfType<TileGrid>().GetManhattenDistance(tile, otherUnit.Tile) <= (isWeaponBased ? EquippedWeapon.Range : AttackRange)
            && otherUnit.PlayerNumber != PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0;
    }

    //check to see if the other unit is healable
    public bool IsUnitHealable(Unit otherUnit)
    {
        return FindObjectOfType<TileGrid>().GetManhattenDistance(Tile, otherUnit.Tile) <= EquippedStaff.Range
            && !otherUnit.Equals(this)
            && otherUnit.PlayerNumber == PlayerNumber
            && ActionPoints >= 1
            && otherUnit.HitPoints > 0
            && otherUnit.HitPoints < otherUnit.TotalHitPoints;

    }

    //Equips the given item. Assume that the unit inventory already contains that item
    public void EquipItem(Item i)
    {
        if (Inventory.Contains(i))
        {
            Inventory.Remove(i);
            Inventory.Insert(0, i);
        }
        else
        {
            UnityEngine.Debug.Log("Unit doesn't contain that item");
        }

    }

    public virtual void ReceiveDamage(Unit source, int dmg)
    {
        HitPoints -= dmg;

        if (HitPoints <= 0)
        {
            HitPoints = 0;
        }
    }

    public async Task ReceiveDeath(Unit source)
    {
        if (UnitDestroyed != null)
        {
            UnitDestroyed.Invoke(this, new AttackEventArgs(source, this));
        }

        TotalMovementPoints = 0;
        TotalActionPoints = 0;
        Tile.IsOccupied = false;
        Tile.CurrentUnit = null;

        SetState(new UnitStateFinished(this));

        //Wait until the death animation is finished playing
        var end = Time.time + 3f;
        GetComponent<SpriteRenderer>().DOFade(0, 2f);

        while (Time.time < end)
        {
            await Task.Yield();
        }

        Destroy(gameObject, 2f);
    }

    public HealingDetails ReceiveHealing(int healAmount)
    {
        HitPoints = Mathf.Clamp(HitPoints + healAmount, 0, TotalHitPoints);

        return new HealingDetails(healAmount, HitPoints);
    }

    //Gets weapon effectiveness against a Unit. -1 for ineffective, 0 for neutral, and > 1 for effective
    public virtual int GetEffectiveness(UnitType otherUnit, WeaponType otherWeapon)
    {
        return EquippedWeapon.GetEffectiveness(otherUnit, otherWeapon);
    }

    public int GetCritChance()
    {
        //Weapon Critical + (Skill / 2)
        return Mathf.Clamp(EquippedWeapon.Crit + (UnitSkill / 2), 0, 100);
    }

    public int GetHitChance(UnitType otherUnit, WeaponType otherWeapon)
    {
        //Accuracy formula = Weapon Hit + (Skill x 2) + (Luck / 2) + Weapon Triangle bonus * 15
        return EquippedWeapon.Hit + (UnitSkill * 2) + (UnitLuck / 2) + EquippedWeapon.GetEffectiveness(otherUnit, otherWeapon) * 15;
    }

    public int GetAttack(UnitType otherUnit, WeaponType otherWeapon)
    {
        //Strength + Weapon Might + Weapon Triangle bonus
        return UnitAttack + EquippedWeapon.Attack + EquippedWeapon.GetEffectiveness(otherUnit, otherWeapon);
    }

    public WeaponPreviewStats GetPreviewWeaponStats(Weapon w)
    {
        return new WeaponPreviewStats(UnitAttack + w.Attack, //Attack
            w.Hit + (UnitSkill * 2) + (UnitLuck / 2), //HitChance
            Mathf.Clamp(w.Crit + (UnitSkill / 2), 0, 100), //CritChance
            Mathf.Clamp((UnitSpeed - (EquippedWeapon.Weight - UnitConst))*2 + UnitLuck, 0, 100)); //DodgeChance
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
            _storedPath = path;
            StartCoroutine(MovementAnimation(path));
        }
    }

    //Moves the unit along a path and sets their sprite direction based on the tile they are heading towards
    protected virtual IEnumerator MovementAnimation(List<OverlayTile> path)
    {
        _anim.SetBool("IsMoving", true);

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
                _anim.SetFloat("MoveX", direction.x);
                _anim.SetFloat("MoveY", direction.y);

            }

            if (Vector2.Distance(transform.position, tempPath[0].transform.position) < Mathf.Epsilon)
            {
                PositionUnit(tempPath[0]);
                tempPath.RemoveAt(0);
            }

            yield return 0;
        }
        IsMoving = false;
    }

    //calculate the direction to face given the position they are heading towards
    public Vector2Int GetDirectionToFace(Vector3 headingPosition)
    {
        var heading = headingPosition - transform.position;
        var distance = heading.magnitude;

        return new Vector2Int((int)((heading / distance).normalized.x), (int)((heading / distance).normalized.y));
    }

    //confirms the unit's destination, and set's their movement points to 0
    public void ConfirmMove()
    {
        if (_storedPath.Count > 0)
        {
            if (UnitMoved != null)
            {
                UnitMoved.Invoke(this, new MovementEventArgs(_storedPath[0], _storedPath[_storedPath.Count - 1], _storedPath));
            }

            MovementPoints = 0;
        }
    }

    //Set's the unit's idle animation's current frame to that of the AnimationTimer's current frame
    public void SetAnimationToIdle()
    {
        _anim.Play("Idle", 0, FindObjectOfType<AnimationTimer>().GetCurrentCurrentTime());
    }

    //Set's the unit's animation to selected
    public void SetAnimationToSelected(bool canAnimate)
    {
        _anim.SetBool("IsSelected", canAnimate);
    }

    //used for whenever we want to start the Unit's movement animation
    public void SetMove(Vector2Int direction, bool canAnimate)
    {
        _anim.SetBool("IsMoving", canAnimate);

        if (canAnimate)
        {
            if (direction == Vector2Int.zero)
            {
                _anim.SetFloat("MoveX", _defaultDirection.x);
                _anim.SetFloat("MoveY", _defaultDirection.y);
            }
            else
            {
                _anim.SetFloat("MoveX", direction.x);
                _anim.SetFloat("MoveY", direction.y);
            }        
        }   
    }

    //reset's the unit's move, placing the unit at the beginning of the cached path
    public void ResetMove()
    {
        if (_storedPath.Count > 0)
        {
            MovementPoints = TotalMovementPoints;
            PositionUnit(_storedPath[0]);
            _storedPath = new List<OverlayTile>();
        }
    }

    //Set the unit to have zero action and movement points, and set their animation to finished
    public void SetFinished()
    {
        _anim.SetBool("IsMoving", false);
        _anim.SetBool("IsFinished", true);
        MovementPoints = 0;
        ActionPoints = 0;
    }

    //positions the unit to a given tile
    public void PositionUnit(OverlayTile tile)
    {
        Tile.CurrentUnit = null;
        Tile = tile;

        transform.position = tile.transform.position;       
        tile.CurrentUnit = this;
    }

    //checks to see if the tile is a valid destination
    public bool IsTileMovableTo(OverlayTile tile)
    {
        if (Tile == tile)
        {
            return true;
        }
        return !tile.IsOccupied && tile.CanUnitMoveTo(UnitType);
    }

    //Checks to see if the tile is moveable across
    public bool IsTileMoveableAcross(OverlayTile tile)
    {
        //if the tile is occupied by an enemy return false
        if (tile.CurrentUnit && tile.CurrentUnit.PlayerNumber != PlayerNumber)
            return false;

        return tile.CanUnitMoveTo(UnitType);
    }

    public void CachePaths(HashSet<OverlayTile> searchableTiles, TileGrid tileGrid)
    {
        _cachedPaths = _pathfinder.FindAllBestPaths(searchableTiles, tileGrid);
    }

    //Get a list of tiles that the Unit can move to
    public HashSet<OverlayTile> GetAvailableDestinations(TileGrid tileGrid)
    {
        var tilesInMoveRange = _rangeFinder.GetTilesInMoveRange(tileGrid);

        if (_cachedPaths == null)
        {
            CachePaths(tilesInMoveRange, tileGrid);
        }

        return tilesInMoveRange;
    }

    //Find the best path to take given cachedPaths
    public List<OverlayTile> FindPath(OverlayTile destination, TileGrid tileGrid)
    {
        return _cachedPaths.ContainsKey(destination) ? _cachedPaths[destination] : new List<OverlayTile>();
    }

    //Get a list of attackable tiles that doesn't include the tiles that a Unit can move to, given availableDestinations
    public HashSet<OverlayTile> GetTilesInAttackRange(HashSet<OverlayTile> availableDestinations, TileGrid tileGrid)
    {
        return _rangeFinder.GetTilesInAttackRange(availableDestinations, tileGrid, AttackRange);
    }

    public HashSet<OverlayTile> GetTilesInRange(TileGrid tileGrid, int range)
    {
        return _rangeFinder.GetTilesInRange(tileGrid, range);
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

    public AttackEventArgs(Unit attacker, Unit defender)
    {
        Attacker = attacker;
        Defender = defender;
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

