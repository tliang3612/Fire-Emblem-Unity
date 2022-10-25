using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayAttackStatsAbility : Ability
{
    public GameObject UnitCanvas;
    public GameObject StatsDisplay;

    public Button CancelButton;

    public List<OverlayTile> tilesInRange;
    public List<Unit> enemiesInAttackRange { get; set; }
    private List<OverlayTile> tilesInAttackRange;

    private void Start()
    {
        UnitCanvas.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        Name = "Attack";
        IsDisplayable = true;
    }

    public override void Display(TileGrid tileGrid)
    {
        if (tileGrid.InSelectionMenu)
            UnitCanvas.SetActive(true);

        tilesInAttackRange = UnitReference.GetTilesInRange(tileGrid, UnitReference.AttackRange).Where(t => t != UnitReference.Tile).ToList();
        tilesInAttackRange.ForEach(t => t.MarkAsAttackableTile());

        /*foreach (Ability ability in GetComponentsInParent<Ability>())
        {
            ability.UnitReference = UnitReference;
            if (ability.IsDisplayable && ability.CanPerform(tileGrid))
            {
                var actionButton = Instantiate(ActionButton, menuActions.transform);
                actionButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.Name;
                actionButton.GetComponent<Button>().onClick.AddListener(() => ActWrapper(actionButton, tileGrid));
                abilitiesMap.Add(actionButton, ability);
                actionButton.SetActive(true);
                ButtonList.Add(actionButton);
            }
        }*/

        CancelButton.onClick.AddListener(() => )
    }

    public override void OnUnitHighlighted(Unit unit, TileGrid tileGrid)
    {
        unit.Tile.HideCursor();
    }

    public override void OnTileSelected(OverlayTile tile, TileGrid tileGrid)
    {
        tile.UnMark();
    }

    public override void OnTileDeselected(OverlayTile tile, TileGrid tileGrid)
    {
        tile.UnMark();
    }

  

    public override void CleanUp(TileGrid tileGrid)
    {
        
        UnitCanvas.SetActive(false);

    }
    
    public void CancelButtonClicked()
    {

    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return tileGrid.InSelectionMenu;
    }

}
