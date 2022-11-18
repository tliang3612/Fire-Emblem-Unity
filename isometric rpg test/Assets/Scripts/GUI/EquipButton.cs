using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EquipButton : MonoBehaviour, IPointerEnterHandler
{
    private Action<Item, Unit> _mouseEntered;

    private Item _item;
    private Unit _unit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseEntered?.Invoke(_item, _unit);
    }

    public void SetData(Unit unit, Item item, Action<Item, Unit> mouseEntered = null)
    {
        _item = item;
        _unit = unit;

        _mouseEntered = mouseEntered;
    }  
}
