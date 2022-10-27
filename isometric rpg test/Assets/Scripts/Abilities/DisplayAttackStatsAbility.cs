using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayAttackStatsAbility : Ability
{
    public class DisplayStats
    {
        public string name { get; private set; }
        public int hp { get; private set; }
        public int dmg { get; private set; }
        public int hit { get; private set; }
        public int crit { get; private set; }

        public DisplayStats(int health, int damage, int hitChance, int critChance, string unitName)
        {
            name = unitName;
            hp = health;
            dmg = damage;
            hit = hitChance;
            crit = critChance;
        }           
    }

    public GameObject DisplayCanvas;
    public GameObject StatsDisplay;

    private List<OverlayTile> tilesInAttackRange;
    
    public Text DefenderName;
    public Text DefenderHealth;
    public Text DefenderDamage;
    public Text DefenderHitChance;
    public Text DefenderCritChance;

    public Text AttackerName;
    public Text AttackerHealth;
    public Text AttackerDamage;
    public Text AttackerHitChance;
    public Text AttackerCritChance;

    private void Start()
    {
        DisplayCanvas.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        Name = "Attack";
        IsDisplayable = true;
    }
    public override void Display(TileGrid tileGrid)
    {
        DisplayCanvas.SetActive(true);                             
        tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());     
    }

    public override void CleanUp(TileGrid tileGrid)
    {
        DisplayCanvas.SetActive(false);
        tilesInAttackRange.ForEach(t => t.UnMark());
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        if (UnitReference.IsUnitAttackable(unit))
        {
            unit.Tile.HighlightedOnUnit();
            //Change StatsDisplay stats
            var attackerStats = GetStats(UnitReference, unit);
            var defenderStats = GetStats(unit, UnitReference);

            SetStats(attackerStats, defenderStats);
        }
    }

    public override void OnTileClicked(OverlayTile tile, TileGrid tileGrid)
    {
        StartCoroutine(Execute(tileGrid,
            _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
            _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, UnitReference.GetComponent<DisplayActionsAbility>())));
        
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        if (UnitReference.IsUnitAttackable(unit))
        {
            UnitReference.GetComponent<AttackAbility>().UnitToAttack = unit;
            UnitReference.GetComponent<AttackAbility>().OnAbilitySelected(tileGrid);
        }
    }


    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        tilesInAttackRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.AttackRange).Where(t => t != UnitReference.Tile).ToList();
        DisplayCanvas.SetActive(true);
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        var enemyUnits = tileGrid.GetEnemyUnits(tileGrid.CurrentPlayer);
        var enemiesInRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u)).ToList();

        return enemiesInRange.Count > 0 && UnitReference.ActionPoints > 0;
    }

    private DisplayStats GetStats(Unit unit, Unit unitToAttack)
    {
        return new DisplayStats(unit.HitPoints, unit.GetTotalDamage(unitToAttack),
            unit.GetBattleAccuracy(unitToAttack), unit.GetCritChance(), unit.UnitName);
    }

    public void SetStats(DisplayStats attackerStats, DisplayStats defenderStats)
    {
        DefenderName.text = defenderStats.name;
        DefenderHealth.text = defenderStats.hp.ToString();
        DefenderDamage.text = defenderStats.dmg.ToString();
        DefenderCritChance.text = defenderStats.crit.ToString();
        DefenderHitChance.text = defenderStats.hit.ToString();

        AttackerName.text = attackerStats.name;
        AttackerHealth.text = attackerStats.hp.ToString();
        AttackerDamage.text = attackerStats.dmg.ToString();
        AttackerCritChance.text = attackerStats.crit.ToString();
        AttackerHitChance.text = attackerStats.hit.ToString();
    }

}
