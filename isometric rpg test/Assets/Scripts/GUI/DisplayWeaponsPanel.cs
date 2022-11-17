using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayWeaponsPanel : GUIPanel
{
    [Header("DisplayActions")]
    [SerializeField] private GameObject menuActions;

    [Header("WeaponStats")]
    [SerializeField] private GameObject weaponStats;
    [SerializeField] private Text AtkText;
    [SerializeField] private Text CritText;
    [SerializeField] private Text HitText;
    [SerializeField] private Text AvoText;

    private List<GameObject> ButtonList;
    public GameObject ActionButton;

    public Action<Weapon, Unit> MouseEntered;

    public void Bind(DisplayWeaponsAbility ability)
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
        actionButton.GetComponent<EquipButton>().SetData(e.unit, e.Weapon, OnWeaponStatsChanged);
        ButtonList.Add(actionButton);
    }

    private void OnWeaponStatsChanged(Weapon w, Unit unit)
    {
        var weaponStats = unit.GetPreviewWeaponStats(w);

        AtkText.text = weaponStats.WeaponAttack;
        CritText.text = weaponStats.WeaponCrit;
        HitText.text = weaponStats.WeaponHit;
        AvoText.text = weaponStats.WeaponAvoid;
    }

    protected override void OnAbilitySelected(object sender, EventArgs e)
    {
        base.OnAbilitySelected(sender, e);
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

    private void ClearStats()
    {
        AtkText.text = "";
        CritText.text = "";
        HitText.text = "";
        AvoText.text = "";
    }
}


