using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SmartAIPlayer : Player
{
	private System.Random _random;
	private TileGrid _tileGrid;
	private CameraController _cameraController;
	private Unit _unitToAttack;
	private OverlayTile _tileToMoveTo;

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

			_unitToAttack = null;
			_tileToMoveTo = null;

			if(CalculateTileToMoveTo(unit))
            {
				yield return StartMove(unit, moveAbility);
            }

			if(CalculateUnitToAttack(unit))
            {
				yield return StartAttack(unit, attackAbility);
				continue;
            }
            else
            {
				unit.SetState(new UnitStateFinished(unit));
			}

			yield return new WaitForSeconds(0.3f);
			
		}
		_tileGrid.EndTurn();
	}

	private IEnumerator StartAttack(Unit unit, AttackAbility attackAbility)
	{
		//Equip the weapon that can match the range of the distance bewtween the two units
		var weaponToEquip = unit.AvailableWeapons.Where(w => w.Range == _tileGrid.GetManhattenDistance(unit.Tile, _unitToAttack.Tile)).FirstOrDefault();
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

	private IEnumerator StartMove(Unit unit, MoveAbility moveAbility)
	{
		moveAbility.OnAbilitySelected(_tileGrid);
		moveAbility.Destination = _tileToMoveTo;

		StartCoroutine(moveAbility.AIExecute(_tileGrid));
		while (unit.IsMoving)
			yield return 0;

		unit.ConfirmMove();
		yield return new WaitForSeconds(0.5f);
	}

	//returns false if there is no tileToMoveTo
	private bool CalculateTileToMoveTo(Unit unit)
    {
		Dictionary<OverlayTile, float> tileScores = new Dictionary<OverlayTile, float>();

		if (unit.GetComponentInChildren<MoveAbility>() == null)
		{
			return false;
		}

		var evaluators = unit.GetComponentsInChildren<TileEvaluator>();

		var availableDestinations = unit.GetAvailableDestinations(_tileGrid);
		foreach (var evaluator in evaluators)
		{
			evaluator.PreEvaluate(availableDestinations, unit, _tileGrid);
		}

		foreach(var tile in availableDestinations)
        {
			if (tile.IsOccupied)
				continue;

			tileScores.Add(tile, 0f);
			foreach(var evaluator in evaluators)
            {
				var score = evaluator.Evaluate(tile, unit, _tileGrid);
				//Debug.Log(evaluator.ToString() + ": " + score);
				tileScores[tile] += score;
            }
        }		
		var bestTile = tileScores.OrderByDescending(s => s.Value).FirstOrDefault().Key;

		

		//if the bestTile has a poor score, don't move at all
		if (tileScores[bestTile] <= 0)
			return false;

		_tileToMoveTo = bestTile;
		return true;

	}

	private bool CalculateUnitToAttack(Unit unit)
    {
		if (unit.GetComponentInChildren<AttackAbility>() == null)
		{
			return false;
		}

		var enemyUnits = _tileGrid.GetEnemyUnits(this);
		var enemiesInRange = enemyUnits.Where(e => unit.IsUnitAttackable(e, unit.Tile)).ToList();

		if (enemiesInRange.Count <= 0 || unit.ActionPoints <= 0)
        {
			return false;
        }

		var evaluators = unit.GetComponentsInChildren<UnitEvaluator>();
		foreach (var evaluator in evaluators)
		{
			evaluator.PreEvaluate(unit, _tileGrid);
		}

		Dictionary<Unit, float> enemyScores = new Dictionary<Unit, float>();
		foreach(var enemy in enemiesInRange)
        {
			enemyScores.Add(enemy, 0f);
			foreach(var evaluator in evaluators)
            {
				var score = evaluator.Evaluate(enemy, unit, _tileGrid);
				enemyScores[enemy] += score;
            }
        }

		//get the best unitToAttack based on how high their score is
		var bestUnit = enemyScores.OrderByDescending(s => s.Value).FirstOrDefault().Key;

		//if the bestUnit has a poor score, don't attack
		if(enemyScores[bestUnit] <= 0)
			return false;

		_unitToAttack = bestUnit;
		return true;

	}
}

