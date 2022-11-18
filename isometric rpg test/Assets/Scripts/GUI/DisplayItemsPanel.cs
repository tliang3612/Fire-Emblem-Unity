using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class DisplayItemsPanel : GUIPanel
{
    [Header("DisplayActions")]
    [SerializeField] private GameObject menuActions;

    [Header("WeaponStats")]
    [SerializeField] private GameObject weaponStats;
    [SerializeField] private Text AtkText;
    [SerializeField] private Text CritText;
    [SerializeField] private Text HitText;
    [SerializeField] private Text AvoText;

    [SerializeField] private Text Description;

    [SerializeField] private GameObject _weaponStatPanel;
    [SerializeField] private GameObject _itemDescriptionPanel;

    private List<GameObject> ButtonList;
    public GameObject ActionButton;

    public Action<Weapon, Unit> MouseEntered;

    public void Bind(SelectItemToUseAbility ability)
    {
        ability.AbilitySelected += OnAbilitySelected;
        ability.AbilityDeselected += OnAbilityDeselected;
        ability.EquipButtonCreated += OnButtonAdded;
    }

    protected void OnButtonAdded(object sender, EquipButtonCreatedEventArgs e)
    {
        var actionButton = Instantiate(ActionButton, menuActions.transform);
        actionButton.GetComponentInChildren<Text>().text = e.ButtonName;
        actionButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(e.ButtonAction));
        actionButton.GetComponent<EquipButton>().SetData(e.Unit, e.Item, OnItemStatsChanged);
        ButtonList.Add(actionButton);
    }

    private void OnItemStatsChanged(Item i, Unit unit)
    {
        if(i is Weapon)
        {
            DisplayWeaponStats(i as Weapon, unit);
        }      

        if(i is Staff)
        {
            DisplayStaffStats(i as Staff, unit);
        }
    }

    private void DisplayWeaponStats(Weapon w, Unit unit)
    {
        ShowWeaponStats();

        var weaponStats = unit.GetPreviewWeaponStats(w);

        AtkText.text = weaponStats.WeaponAttack;
        CritText.text = weaponStats.WeaponCrit;
        HitText.text = weaponStats.WeaponHit;
        AvoText.text = weaponStats.WeaponAvoid;
    }

    private void DisplayStaffStats(Staff s, Unit unit)
    {
        ShowItemDescription();
        Description.text = "Heal a unit for " + s.HealAmount + " health";
    }

    protected override void OnAbilitySelected(object sender, EventArgs e)
    {
        base.OnAbilitySelected(sender, e);
        ClearStats();
        ButtonList = new List<GameObject>();
    }

    protected override void OnAbilityDeselected(object sender, EventArgs e)
    {
        base.OnAbilityDeselected(sender, e);

        SetState(GUIState.InAbilitySelection);

        foreach (GameObject button in ButtonList)
        {
            Destroy(button);
        }
        ClearStats();
    }   

    private void ShowWeaponStats()
    {
        _weaponStatPanel.SetActive(true);
        _itemDescriptionPanel.SetActive(false);
    }

    private void ShowItemDescription()
    {
        _weaponStatPanel.SetActive(false);
        _itemDescriptionPanel.SetActive(true);
    }
    private void ClearStats()
    {
        AtkText.text = "";
        CritText.text = "";
        HitText.text = "";
        AvoText.text = "";

        _itemDescriptionPanel.SetActive(false);
        _weaponStatPanel.SetActive(false);
    }
}


