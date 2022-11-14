using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GUIState
{
    Clear,
    InAbilitySelection,
    InGameGUISelection,
    BlockInput
}
public class GUIController : MonoBehaviour
{
    public TileGrid tileGrid;

    //[SerializeField] private DisplayWeaponsPanel weaponsPanel;
    [SerializeField] private DisplayActionsPanel actionsPanel;
    [SerializeField] private DisplayAttackStatsPanel attackStatsPanel;
    [SerializeField] private DisplayHealStatsPanel healStatsPanel;
    [SerializeField] private OverlayPanel overlayPanel;

    [SerializeField] public GameObject RightPanelHolder;
    [SerializeField] public GameObject TopRightPanelHolder;

    protected Vector2 topRightPosition;
    protected Vector2 rightPanelPosition;

    protected virtual void Awake()
    {
        topRightPosition = TopRightPanelHolder.GetComponent<RectTransform>().anchoredPosition;
        rightPanelPosition = RightPanelHolder.GetComponent<RectTransform>().anchoredPosition;

        RightPanelHolder.SetActive(false);
        TopRightPanelHolder.SetActive(false);               
    }

    protected virtual void Start()
    {
        tileGrid.UnitAdded += OnUnitAdded;

        overlayPanel.Bind(tileGrid);

        foreach (GUIPanel panel in GetComponentsInChildren<GUIPanel>())
        {
            panel.ReceivePanelPosition(topRightPosition, rightPanelPosition);
        }     
    }

    protected virtual void OnUnitAdded(object sender, UnitCreatedEventArgs e)
    {
        RegisterUnit(e.Unit, e.Abilities);
    }


    protected virtual void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        overlayPanel.BindUnit(unit, unitAbilities);

        //bind the ability 
        foreach (var ability in unitAbilities)
        {
            //f (ability is DisplayWeaponsAbility) weaponsPanel.Bind(ability as DisplayWeaponsAbility);
            if (ability is DisplayActionsAbility) actionsPanel.Bind(ability as DisplayActionsAbility);
            if (ability is DisplayAttackStatsAbility) attackStatsPanel.Bind(ability as DisplayAttackStatsAbility);
            if (ability is DisplayHealStatsAbility) healStatsPanel.Bind(ability as DisplayHealStatsAbility);
        }      
    }
}

