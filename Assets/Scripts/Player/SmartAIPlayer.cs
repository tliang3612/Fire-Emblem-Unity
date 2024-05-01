using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;


public class SmartAIPlayer : Player
{
    private System.Random _random;
    private TileGrid _tileGrid;
    private CameraController _cameraController;
    private Unit _unitToAttack;
    private Unit _unitToHeal;
    private OverlayTile _tileToMoveTo;
    private Dictionary<Unit, List<Tuple<OverlayTile, float>>> _encounterScores;
    [SerializeField] private bool _debugMode;
    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
    }

    public override void Play(TileGrid grid)
    {
        _random = new System.Random();
        _tileGrid = grid;
        _tileGrid.GridState = new TileGridStateBlockInput(_tileGrid);
        _encounterScores = new Dictionary<Unit, List<Tuple<OverlayTile, float>>>();
        _unitToAttack = null;
        _tileToMoveTo = null;
        _unitToHeal = null;

        StartCoroutine(Play());
    }

    public IEnumerator Play()
    {
        var myUnits = _tileGrid.GetCurrentPlayerUnits().OrderByDescending(u => u.HitPoints); // highest health unit goes first
        foreach (Unit unit in myUnits)
        {
            yield return StartCoroutine(ExecuteUnitTurn(unit));
        }

        _tileGrid.EndTurn();
    }

    private IEnumerator ExecuteUnitTurn(Unit unit)
    {
        _cameraController.MoveToPoint(unit.transform.position);
        yield return new WaitForSeconds(1f);

        var moveAbility = unit.GetComponentInChildren<MoveAbility>();
        var attackAbility = unit.GetComponentInChildren<AttackAbility>();
        var healAbility = unit.GetComponentInChildren<HealAbility>();

        if (CalculateTileToMoveTo(unit))
        {
            // pause to see the tile scores
            if (_debugMode)
            {
                while (!Input.GetKeyDown(KeyCode.Space))
                {
                    yield return null;
                }
            }
            unit.SetState(new UnitStateNormal(unit));
            yield return StartMove(unit, moveAbility);
        }


        // if unit can and should perform a heal ability or attack ability
        if (attackAbility && CalculateUnitToAttack(unit))
        {
            if (_unitToAttack != null)
                yield return StartAttack(unit, attackAbility);
        }
        else
        {
            unit.SetState(new UnitStateFinished(unit));
        }

        yield return new WaitForSeconds(0.3f);

    }

    private IEnumerator StartAttack(Unit unit, AttackAbility attackAbility)
    {
        //Equip the weapon that can match the range of the distance bewtween the two units
        /*var weaponToEquip = unit.AvailableWeapons.Where(w => w.Range >= _tileGrid.GetManhattenDistance(unit.Tile, _unitToAttack.Tile)).FirstOrDefault();
        unit.EquipItem(weaponToEquip);*/


        var idealRange = _tileGrid.GetManhattenDistance(unit.Tile, _unitToAttack.Tile);
        var weaponThatMatchesRange = unit.AvailableWeapons.Aggregate(unit.EquippedWeapon,
                                                                                (weapon, next) =>
                                                                                    next.Range == idealRange ? next : weapon);
        unit.EquipItem(weaponThatMatchesRange);

        var direction = unit.GetDirectionToFace(_unitToAttack.Tile.transform.position);
        unit.SetState(new UnitStateMoving(unit, direction));

        attackAbility.UnitToAttack = _unitToAttack;
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(attackAbility.AIExecute(_tileGrid));
        while (_tileGrid.IsBattling)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StartHeal(Unit unit, HealAbility healAbility)
    {
        var staffToEquip = unit.AvailableWeapons.Where(w => w.Range >= _tileGrid.GetManhattenDistance(unit.Tile, _unitToHeal.Tile)).FirstOrDefault();
        unit.EquipItem(staffToEquip);

        var direction = unit.GetDirectionToFace(_unitToHeal.Tile.transform.position);
        unit.SetState(new UnitStateMoving(unit, direction));

        healAbility.UnitToHeal = _unitToHeal;
        healAbility.OnAbilitySelected(_tileGrid);

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(healAbility.AIExecute(_tileGrid));
        while (_tileGrid.IsBattling)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StartMove(Unit unit, MoveAbility moveAbility)
    {
        unit.GetAvailableDestinations(_tileGrid).ToList().ForEach(t => t.HideTileScore());
        moveAbility.OnAbilitySelected(_tileGrid);
        moveAbility.Destination = _tileToMoveTo;

        StartCoroutine(moveAbility.AIExecute(_tileGrid));
        while (unit.IsMoving)
            yield return null;

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
        var attackableEnemies = new List<Unit>();
        var attackableTiles = unit.GetTilesInAttackRange(_tileGrid);

        foreach (var tile in attackableTiles)
        {
            var enemy = tile.CurrentUnit;
            //we check if the unit exists and can be attacked
            if (enemy && unit.PlayerNumber != enemy.PlayerNumber)
                attackableEnemies.Add(enemy);
            else
                continue;
        }

        // Simulate encounter value for each enemy that unit can attack
        foreach(var enemy in attackableEnemies)
        {
            //each tile is going to have a different encounter score, as different tiles can change the outcome 
            var tilesToAttackEnemyFrom = attackableTiles.Where(t => unit.IsUnitAttackableFromTile(t, enemy, false)).ToList();
            foreach (var tile in tilesToAttackEnemyFrom)
            {
                if (!_encounterScores.ContainsKey(enemy))
                    _encounterScores.Add(enemy, new List<Tuple<OverlayTile, float>>());

                _encounterScores[enemy].Add(GetEncounterScore(unit, enemy, tile));
            }
                
        }

        foreach (var evaluator in evaluators)
        {
            evaluator.PreEvaluate(availableDestinations, unit, _tileGrid, _encounterScores);
        }

        foreach (var tile in availableDestinations)
        {
            if (tile.IsOccupied)
                continue;

            tileScores.Add(tile, 0f);
            foreach (var evaluator in evaluators)
            {
                var score = evaluator.Evaluate(tile, unit, _tileGrid);
                tileScores[tile] += score;     
            }
            if (_debugMode)
            {
                unit.SetState(new UnitStateHovered(unit));
                tile.SetTileScore(tileScores[tile]);
            }
        }

        var bestScore = tileScores.Max(t => t.Value);
        List<OverlayTile> bestTiles = tileScores.Keys.Where(t => tileScores[t] == bestScore).ToList();
        OverlayTile bestTile = bestTiles.FirstOrDefault();

        //If multiple high scoring tiles, choose the one that is closest to the enemy
        if (bestTiles.Count > 1)
        {
            var closestEnemy = _tileGrid.GetEnemyUnits(unit.Player).OrderBy(e => _tileGrid.GetManhattenDistance(e.Tile, unit.Tile)).FirstOrDefault();
            if (closestEnemy != null)
                bestTile = bestTiles.OrderBy(t => _tileGrid.GetManhattenDistance(t, closestEnemy.Tile)).FirstOrDefault();
        }


        //if the bestTile has a poor score, don't move at all
        if (bestTile == null)
        {
            Debug.Log("Best tile is null for some reason");
            return false;
        }

        if (!tileScores.ContainsKey(bestTile))
            return false;
       

        _tileToMoveTo = bestTile;
        return true;

    }
    private bool CalculateUnitToAttack(Unit unit)
    {
        var enemyUnits = _tileGrid.GetEnemyUnits(this);
        var enemiesInRange = enemyUnits.Where(e => unit.IsUnitAttackable(e, false)).ToList();

        if (enemiesInRange.Count <= 0 || unit.ActionPoints <= 0)
        {
            return false;
        }

        float bestScore = 0f;
        foreach(var enemy in enemiesInRange)
        {
            if (!_encounterScores.ContainsKey(enemy))
                continue;

            // get the score of the current unit's tile
            var score = 0f;
            var tupleWithMatchingTile = _encounterScores[enemy].Where(pair => pair.Item1.gridLocation2D == unit.Tile.gridLocation2D).FirstOrDefault();
            if(tupleWithMatchingTile != null)
                score = tupleWithMatchingTile.Item2;

            if (score > bestScore)
            {
                bestScore = score;
                _unitToAttack = enemy;
            }                   
        }
        if (_unitToAttack == null)
        {
            return false;
        }

        return true;

    }
    
    private bool CalculateUnitToHeal(Unit unit)
    {
        var allyUnits = _tileGrid.GetCurrentPlayerUnits();
        var alliesInRange = allyUnits.Where(a => _tileGrid.GetManhattenDistance(unit.Tile, a.Tile) <= unit.EquippedStaff.Range).ToList();

        if (alliesInRange.Count <= 0 || unit.ActionPoints <= 0 || alliesInRange.Any(a => a.HitPoints == a.TotalHitPoints))
        {
            Debug.Log("No Allies in range for heal");
            _unitToHeal = null;
            return false;
        }

        var healEvaluator = unit.GetComponentsInChildren<UnitEvaluator>().OfType<UnitToHealUnitEvaluator>().FirstOrDefault();

        if (healEvaluator == null)
        {
            Debug.Log("heal eval is null");
            _unitToHeal = null;
            return false;
        }

        //healEvaluator.PreEvaluate(unit, _tileGrid, _encounterScores);

        Dictionary<Unit, float> allyScores = new Dictionary<Unit, float>();
        foreach (var ally in alliesInRange)
        {
            allyScores.Add(ally, 0f);
            var score = healEvaluator.Evaluate(ally, unit, _tileGrid);
            allyScores[ally] += score;
        }

        //get the best unitToHeal based on how high their score is
        var bestUnit = allyScores.OrderByDescending(s => s.Value).FirstOrDefault().Key;

        //if the bestUnit has a poor score, don't heal
        if (allyScores[bestUnit] <= 0)
        {
            Debug.Log("score is 0");
            return false;
        }

        _unitToHeal = bestUnit;
        return true;

    }
    

    // The score for the attacking unit when attacking the enemy
    private Tuple<OverlayTile, float> GetEncounterScore(Unit evaluatingUnit, Unit enemy, OverlayTile tileToAttackFrom)
    {
        var encounterScore = 0f;

        var originalUnitTile = evaluatingUnit.Tile;
        evaluatingUnit.Tile = tileToAttackFrom; // we temporarily move the unit to the new tile, and simulate the encounter. This is to determine each tiles encounter score

        var idealRange = _tileGrid.GetManhattenDistance(tileToAttackFrom, enemy.Tile);
        var weaponThatMatchesRange = evaluatingUnit.AvailableWeapons.Aggregate(evaluatingUnit.EquippedWeapon,
                                                                                (weapon, next) =>
                                                                                    next.Range == idealRange ? next : weapon);     
                                                                                
        evaluatingUnit.EquipItem(weaponThatMatchesRange);

        // the more units left the player has that can move, the more aggressive the unit
        var playerUnitsInRangeOfEnemy = _tileGrid.GetCurrentPlayerUnits().Where(u => u.ActionPoints > 0 && u.IsUnitWithinAttackRange(enemy)).ToList();
        float actionableUnitsMultiplier = playerUnitsInRangeOfEnemy.Count * .15f + 1; // arbitrary formula

        CombatStats unitStats = new CombatStats(evaluatingUnit, enemy, evaluatingUnit.AttackRange);
        CombatStats enemyStats = new CombatStats(enemy, evaluatingUnit, evaluatingUnit.AttackRange);
        CombatCalculator calculator = new CombatCalculator(unitStats, enemyStats, evaluatingUnit.AttackRange);

        List<bool> attackOrder = calculator.GetAttackOrder();

        var unitDamageDealt = 0f;
        var enemyDamageDealt = 0f;
        var unitAccuracyModifier = GetBattleAccuracy(evaluatingUnit, enemy) / 100f;
        var enemyAccuracyModifier = GetBattleAccuracy(enemy, evaluatingUnit) / 100f;

        float tempUnitHealth = evaluatingUnit.HitPoints;
        float tempEnemyHealth = enemy.HitPoints;

        foreach (var isAIAttacking in attackOrder)
        {
            if(isAIAttacking)
            {
                var damage = GetDryAttackDamage(evaluatingUnit, enemy);
                unitDamageDealt += damage;
                tempEnemyHealth -= unitDamageDealt;
                if (tempEnemyHealth < 0)
                {
                    encounterScore += 100f; //if the enemy is dead during this attack, this is considered very good move
                    evaluatingUnit.Tile = originalUnitTile; //reset position after simulated encounter
                    return new Tuple<OverlayTile, float>(tileToAttackFrom, encounterScore);
                }            
            }
            else
            {
                var damage = GetDryAttackDamage(enemy, evaluatingUnit);
                enemyDamageDealt += damage;
                tempUnitHealth -= enemyDamageDealt;
                if (tempUnitHealth < 0)
                {
                    encounterScore = 0f; //if the unit dies during this encounter, this is considered a horrible move
                    evaluatingUnit.Tile = originalUnitTile; //reset position after simulated encounter
                    return new Tuple<OverlayTile, float>(tileToAttackFrom, encounterScore);
                }
            }
        }
        float unitHealthLostPorportional = tempUnitHealth/evaluatingUnit.HitPoints;
        float enemyHealthLostPorportional = tempEnemyHealth / enemy.HitPoints;


        // we multiply accuracy at the end because we want the AI to take a chance at attacking if it means killing the enemy, even if there is bad accuracy
        encounterScore += Mathf.Clamp((unitDamageDealt * unitAccuracyModifier * unitHealthLostPorportional) 
                                    - (enemyDamageDealt * enemyAccuracyModifier * enemyHealthLostPorportional), 0f , 100f);
                                            // encounter score should be really high if unit doesnt take damage after the encounter. Could mean that the unit outranges
                                            // the enemy, or the enemy is dead before attacking
        encounterScore *= actionableUnitsMultiplier; 

        evaluatingUnit.Tile = originalUnitTile; //reset position after simulated encounter
        return new Tuple<OverlayTile, float>(tileToAttackFrom, encounterScore);
    }

    private int GetDryAttackDamage(Unit attacker, Unit defender)
    {
        //if the attacker can double attack, the multipler is *2
        int multiplier = (attacker.GetAttackSpeed() - defender.GetAttackSpeed()) >= 4 ? 2 : 1;

        var attackerAttackStat = attacker.GetAttack(defender.UnitType, defender.EquippedWeapon.Type);
        var defenderDefenseStat = defender.GetDefense();
        return Mathf.Clamp((attackerAttackStat - defenderDefenseStat) * multiplier, 0, 100);
    }

    private int GetBattleAccuracy(Unit attacker, Unit defender)
    {
        var attackerHitStat = attacker.GetHitChance(defender.UnitType, defender.EquippedWeapon.Type);
        var defenderDodgeStat = defender.GetDodgeChance();
        return Mathf.Clamp(attackerHitStat - defenderDodgeStat, 0, 100);
    }
}


