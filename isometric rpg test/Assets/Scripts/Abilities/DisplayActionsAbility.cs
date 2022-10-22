using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DisplayActionsAbility : Ability
{
    public GameObject menuActions;
    public Button attackButton;

    public bool attackActionClicked;

    private void Start()
    {
        attackActionClicked = false;
        attackButton.onClick.AddListener(MoveIsClicked);
    }
    public override void Display(TileGrid cellGrid)
    {
        menuActions.SetActive(true);
    }
    public override void CleanUp(TileGrid cellGrid)
    {
        menuActions.SetActive(false);
    }

    public void MoveIsClicked()
    {
        attackActionClicked = true;
    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        if (attackActionClicked)
        {
            tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, unit, unit.GetComponent<AttackAbility>());
        }
        attackActionClicked = false;
    }
}