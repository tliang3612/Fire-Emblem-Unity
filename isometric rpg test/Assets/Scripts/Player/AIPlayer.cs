using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class AIPlayer : Player
{
	Unit currentUnit;

	public AIPlanOfAttack Evaluate(TileGrid tileGrid, Unit unit)
	{

		currentUnit = unit;
		AIPlanOfAttack poa = new AIPlanOfAttack();

		PlanPositionIndependent(poa, tileGrid);


		return poa;
	}

	public override void Play(TileGrid tileGrid)
	{
		tileGrid.GridState = new TileGridStateBlockInput(tileGrid);
		var myUnits = tileGrid.GetCurrentPlayerUnits();
		var poa = new AIPlanOfAttack();
		
		foreach (Unit unit in myUnits)
		{
			poa = Evaluate(tileGrid, unit);
			unit.GetComponent<MoveAbility>().Destination = poa.tileToMove;
			StartCoroutine(unit.GetComponent<MoveAbility>().AIExecute(tileGrid));
			unit.GetComponent<AttackAbility>().UnitToAttack = poa.target;
			StartCoroutine(unit.GetComponent<AttackAbility>().AIExecute(tileGrid));
		}
		

	}

	
	void PlanPositionIndependent(AIPlanOfAttack poa, TileGrid tileGrid)
	{
		List<OverlayTile> moveOptions = GetMoveOptions(poa, tileGrid);
		OverlayTile tile = moveOptions[Random.Range(0, moveOptions.Count)];
		poa.tileToMove = poa.target.Tile;

	}



	IEnumerator MoveUnit(AIPlanOfAttack poa, TileGrid tileGrid)
	{
		List<OverlayTile> potentialDestinations = GetMoveOptions(poa, tileGrid);

		var nearestEnemy = FindNearestEnemy(tileGrid);
		if (nearestEnemy != null)
		{
			OverlayTile destinationTile = nearestEnemy.Tile;

			currentUnit.GetComponent<MoveAbility>().Destination = destinationTile;
			StartCoroutine(currentUnit.GetComponent<MoveAbility>().Act(tileGrid));
			while (currentUnit.IsMoving)
				yield return 0;
		}

		poa.tileToMove = currentUnit.Tile;
	}

	Unit FindNearestEnemy(TileGrid tileGrid)
	{
		var enemyUnits = tileGrid.GetEnemyUnits(this);
		var enemiesInRange = new List<Unit>();
		Unit nearestEnemy;

		foreach (Unit enemy in enemyUnits)
		{
			if (currentUnit.IsUnitAttackable(enemy))
			{
				enemiesInRange.Add(enemy);
			}
		}
		//Finds nearest enemy by comparing their mahatten distances
		nearestEnemy = enemiesInRange.OrderByDescending(e => tileGrid.GetManhattenDistance(currentUnit.Tile, e.Tile)).FirstOrDefault();

		return nearestEnemy;
	}

	List<OverlayTile> GetMoveOptions(AIPlanOfAttack poa, TileGrid tileGrid)
	{
		var potentialDestinations = new List<OverlayTile>();
		var enemyUnits = tileGrid.GetEnemyUnits(this);

		foreach (var enemyUnit in enemyUnits)
		{
			potentialDestinations.AddRange(tileGrid.TileList.FindAll(t => currentUnit.IsTileMovableTo(t) && currentUnit.IsUnitAttackable(enemyUnit)));
		}

		return potentialDestinations;
	}
}

    