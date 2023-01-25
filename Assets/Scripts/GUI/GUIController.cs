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
    MenuGUISelection,
    BlockInput
}
public class GUIController : MonoBehaviour
{
    public TileGrid tileGrid;

    //[SerializeField] private DisplayItemsPanel weaponsPanel;
    [SerializeField] private DisplayActionsPanel actionsPanel;
    [SerializeField] private DisplayAttackStatsPanel attackStatsPanel;
    [SerializeField] private DisplayHealStatsPanel healStatsPanel;
    [SerializeField] private DisplayItemsPanel weaponsPanel;
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

        tileGrid.UnitAdded += OnUnitAdded;
        overlayPanel.Bind(tileGrid);

        foreach (GUIPanel panel in FindObjectsOfType<GUIPanel>())
        {
            panel.ReceivePanelPosition(topRightPosition, rightPanelPosition);
        }
    }

    protected virtual void Start()
    {
         
    }

    public virtual void OnUnitAdded(object sender, UnitCreatedEventArgs e)
    {
        RegisterUnit(e.Unit, e.Abilities);
    }


    protected virtual void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        var abilityList = unitAbilities.ToList();
        List<Ability> removedAbilities = new List<Ability>();

        //bind the ability 
        foreach (var ability in abilityList)
        {
            if (ability is DisplayActionsAbility) {
                actionsPanel.Bind(ability as DisplayActionsAbility);
                removedAbilities.Add(ability as DisplayActionsAbility);
            }

            if (ability is DisplayAttackStatsAbility)
            {
                attackStatsPanel.Bind(ability as DisplayAttackStatsAbility);
                removedAbilities.Add(ability as DisplayAttackStatsAbility);
            }
            if (ability is DisplayHealStatsAbility)
            {
                healStatsPanel.Bind(ability as DisplayHealStatsAbility);
                removedAbilities.Add(ability as DisplayHealStatsAbility);
            }
            if (ability is SelectItemToUseAbility) 
            {
                weaponsPanel.Bind(ability as SelectItemToUseAbility);
                removedAbilities.Add(ability as SelectItemToUseAbility);
            }
        }

        //the overlay panel handles abilities that aren't binded
        overlayPanel.BindUnit(unit, abilityList.Except(removedAbilities).ToList());
    }
}

