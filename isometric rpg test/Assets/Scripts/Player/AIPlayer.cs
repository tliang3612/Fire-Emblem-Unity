using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class AIPlayer : Player
{
	private System.Random _random;
	private TileGrid _tileGrid;
	private CameraController _cameraController;
	private Unit _unitToAttack;

	private void Awake()
	{
		_cameraController = FindObjectOfType<CameraController>();
	}

	public override void Play(TileGrid grid)
	{		
		_random = new System.Random();
		_tileGrid = grid;
		_tileGrid.GridState = new TileGridStateBlockInput(_tileGrid);

		StartCoroutine(Play());
	}

	public IEnumerator Play()
	{	
		var myUnits = _tileGrid.GetCurrentPlayerUnits();
		foreach (Unit unit in myUnits)
		{
			_cameraController.MoveToPoint(unit.transform.position);
			yield return new WaitForSeconds(1f);
			var moveAbility = unit.GetComponentInChildren<MoveAbility>();
			var attackAbility = unit.GetComponentInChildren<AttackAbility>();

			if (GetUnitToAttack(unit))
            {
				yield return StartAttack(unit, attackAbility);
				continue;
			}
			else if(GetDestination(unit))
            {
				moveAbility.OnAbilitySelected(_tileGrid);
				moveAbility.Destination = GetDestination(unit);

                StartCoroutine(unit.GetComponentInChildren<MoveAbility>().AIExecute(_tileGrid));				
				while (unit.IsMoving)
					yield return 0;
				unit.ConfirmMove();			
				yield return new WaitForSeconds(0.3f);
				
                if (GetUnitToAttack(unit))
                {
					yield return StartAttack(unit, attackAbility);

					continue;
				}
                else
                {
					unit.SetState(new UnitStateFinished(unit));
				}
			}
			_unitToAttack = null;
			unit.GetComponentInChildren<WaitAbility>().AIExecute(_tileGrid);
		}
		_tileGrid.EndTurn();
	}
	
	private IEnumerator StartAttack(Unit unit, AttackAbility attackAbility)
    {
		//Equip the weapon that can match the range of the distance bewtween the two units
		var weaponToEquip = unit.AvailableWeapons.Where(w => w.Range == _tileGrid.GetManhattenDistance(unit.Tile, _unitToAttack.Tile)).First();
		unit.EquipItem(weaponToEquip);

		var direction = unit.GetDirectionToFace(_unitToAttack.Tile.transform.position);
		unit.SetState(new UnitStateMoving(unit, direction));

		attackAbility.UnitToAttack = _unitToAttack;
		yield return new WaitForSeconds(0.3f);
		yield return StartCoroutine(attackAbility.AIExecute(_tileGrid));
		while (_tileGrid.IsBattling)
		{
			yield return 0;
		}
		yield return new WaitForSeconds(0.5f);
	}

	//Returns true if there is a unit that is in range, also stores the unitToAttack
	private bool GetUnitToAttack(Unit unit)
    {		
		var enemyUnits = _tileGrid.GetEnemyUnits(this);
		var unitsInRange = enemyUnits.Where(e => unit.IsUnitAttackable(e, true)).ToList();

		if (unitsInRange.Count != 0)
		{
			var index = _random.Next(0, unitsInRange.Count);
			_unitToAttack = unitsInRange[index];
			return true;
		}
		return false;
	}

	//returns the tile to move to. Returns the unit's tile if there are no available destinations
	private OverlayTile GetDestination(Unit unit)
    {
		List<OverlayTile> potentialDestinations = new List<OverlayTile>();
		//Order enemies units by how close they are to the Unit
		var enemyUnits = _tileGrid.GetEnemyUnits(this).OrderByDescending(e => _tileGrid.GetManhattenDistance(e.Tile, unit.Tile)).ToList();

		//Get closest enemy from the Unit, which we'll use to find the best movement path 
		var closestEnemy = enemyUnits.Last();

		var availableDestinations = unit.GetAvailableDestinations(_tileGrid);

		//Find all tiles the unit can attack an enemy from
		foreach(var tile in availableDestinations)
        {
			if (unit.IsTileAttackableFrom(tile, closestEnemy, true))
            {
				potentialDestinations.Add(tile);
            }        
        }

		//return a tile the unit can move to and attack from
		if (potentialDestinations.Count != 0)
			return potentialDestinations.FirstOrDefault();
		
		//If no potential destinations, set potentialDestinations to available destinations
		if (potentialDestinations.Count == 0 && availableDestinations.Count != 0)
		{
			potentialDestinations.AddRange(availableDestinations);
		}
		else if(availableDestinations.Count == 0)
        {
			return unit.Tile;
        }

		//order potential destinations by how close they are to the closest enemy
		potentialDestinations = potentialDestinations.OrderByDescending(t => _tileGrid.GetManhattenDistance(t, closestEnemy.Tile)).ToList();

		OverlayTile bestDestination = null;

		foreach (var potentialDestination in potentialDestinations)
		{ 
			var distanceFromEnemy = _tileGrid.GetManhattenDistance(potentialDestination, closestEnemy.Tile);

			//if there is no path
			if ((!bestDestination) || distanceFromEnemy < _tileGrid.GetManhattenDistance(bestDestination, closestEnemy.Tile))
				bestDestination = potentialDestination;		
		}

		return bestDestination;

	}
}

    