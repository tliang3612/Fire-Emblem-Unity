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

			var tilesToMove = unit.GetAvailableDestinations(_tileGrid);

			foreach(OverlayTile tile in tilesToMove)
            {

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

	private void CalculateAttack(Unit unit)
    {
		if (unit.GetComponent<AttackAbility>() == null)
		{
			return;
		}

		var enemyUnits = _tileGrid.GetEnemyUnits(this);
		var enemiesInRange = enemyUnits.Where(e => unit.IsUnitAttackable(e, unit.Tile)).ToList();

		if (enemiesInRange.Count <= 0 && unit.ActionPoints <= 0)
        {
			return;
        }

		var evaluators = unit.GetComponentsInChildren<UnitEvaluator>();
		foreach (var evaluator in evaluators)
		{
			evaluator.PreEvaluate(unit, _tileGrid);
		}

		List<(Unit, float)> enemyScores = new List<(Unit, float)>();
		foreach(var e in enemiesInRange)
        {
			foreach(var evaluator in evaluators)
            {
				var score = evaluator.Evaluate(e, unit, _tileGrid);
				enemyScores.Add((e, score));
            }
        }

		//get the best unitToAttack based on how high their score is
		var bestUnit = enemyScores.OrderByDescending(s => s.Item2).First().Item1;

		_unitToAttack = bestUnit;

	}
}

