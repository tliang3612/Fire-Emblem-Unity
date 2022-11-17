using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EquipButton : MonoBehaviour, IPointerEnterHandler
{
    private Action<Weapon, Unit> _mouseEntered;

    private Weapon _weapon;
    private Unit _unit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseEntered?.Invoke(_weapon, _unit);
    }

    public void SetData(Unit unit, Weapon w, Action<Weapon, Unit> mouseEntered)
    {
        _weapon = w;
        _unit = unit;

        _mouseEntered = mouseEntered;
    }  
}
