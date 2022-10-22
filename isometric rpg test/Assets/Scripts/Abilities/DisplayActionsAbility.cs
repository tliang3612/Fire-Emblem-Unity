using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class DisplayActionsAbility : Ability
{
    /*public GameObject menuActions;
    public Button cancelButton;*/

    private bool attackSelected;
    private bool cancelSelected;
    private bool waitSelected;

    private TileGrid _tileGrid;

    private void Start()
    {
        //cancelButton.onClick.AddListener(OnAttackClicked);
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        if (CanPerform(tileGrid) && cancelSelected)
        {
            var storedMoveDetails = UnitReference.GetStoredMovementDetails();
            UnitReference.ResetMove(storedMoveDetails);
            Debug.Log("Move Resetted");

            UnitReference.InSelectionMenu = false;
            cancelSelected = false;
            yield return 0;
        }
    }

    public override void Display(TileGrid tileGrid)
    {
        
    }
    public override void CleanUp(TileGrid tileGrid)
    {
        
    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        _tileGrid = tileGrid;
        Debug.Log(UnitReference.InSelectionMenu);
    }

    public override void OnAbilityDeselected(TileGrid tileGrid)
    {
        
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return UnitReference.InSelectionMenu;
    }

    public void OnAttackClicked()
    {

    }

    public override void OnUnitClicked(Unit unit, TileGrid tileGrid)
    {
        cancelSelected = true;
        StartCoroutine(HumanExecute(tileGrid));
    }

    public void OnWaitClicked()
    {

    }

}