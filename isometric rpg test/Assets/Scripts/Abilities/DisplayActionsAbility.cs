using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DisplayActionsAbility : Ability
{
    public GameObject UnitCanvas;
    [HideInInspector]
    public GameObject SelectedButton;

    public GameObject menuActions;
    public GameObject ActionButton;

    Dictionary<GameObject, Ability> abilitiesMap;

    private List<GameObject> ButtonList;

    private void Start()
    {
        UnitCanvas.SetActive(false);
    }

    public override IEnumerator Act(TileGrid tileGrid)
    {
        yield return 0;
    }

    public override void Display(TileGrid tileGrid)
    {
        UnitCanvas.SetActive(true);

        foreach(Ability ability in GetComponentsInParent<Ability>())
        {
            ability.UnitReference = UnitReference;
            if(ability.IsDisplayable && ability.CanPerform(tileGrid))
            {
                var actionButton = Instantiate(ActionButton, menuActions.transform);
                actionButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.Name;
                actionButton.GetComponent<Button>().onClick.AddListener(() => ActWrapper(actionButton, tileGrid));
                abilitiesMap.Add(actionButton, ability);
                actionButton.SetActive(true);
                ButtonList.Add(actionButton);
            }    
        }
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

    void ActWrapper(GameObject button, TileGrid tileGrid)
    {
        SelectedButton = button;
        StartCoroutine(Execute(tileGrid,
                _ => tileGrid.GridState = new TileGridStateBlockInput(tileGrid),
                _ => tileGrid.GridState = new TileGridStateUnitSelected(tileGrid, UnitReference, abilitiesMap[SelectedButton])));
    }

    public override void CleanUp(TileGrid tileGrid)
    {
        foreach (var button in ButtonList)
        {
            Destroy(button);
        }
        UnitCanvas.SetActive(false);

    }

    public override void OnAbilitySelected(TileGrid tileGrid)
    {
        abilitiesMap = new Dictionary<GameObject, Ability>();
        ButtonList = new List<GameObject>();
        UnitCanvas.SetActive(true);
    }

    public override bool CanPerform(TileGrid tileGrid)
    {
        return true;
    }

}
