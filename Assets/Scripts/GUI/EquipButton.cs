using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EquipButton : MonoBehaviour, IPointerEnterHandler
{
    private Action<Item> _mouseEntered;

    private Item _item;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseEntered?.Invoke(_item);
    }

    public void SetData(Item item, Action<Item> mouseEntered = null)
    {
        _item = item;

        _mouseEntered = mouseEntered;
    }  
}
