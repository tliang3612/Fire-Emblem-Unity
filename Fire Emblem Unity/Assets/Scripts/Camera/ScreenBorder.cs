using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenBorder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Action<Vector2Int> MouseEntered;
    private Action MouseExited;

    [SerializeField] private Vector2Int _direction;

    public void SetupActions(Action<Vector2Int> mouseEntered, Action mouseExited)
    {
        MouseEntered = mouseEntered;
        MouseExited = mouseExited;
    }
   
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseEntered?.Invoke(_direction);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseExited?.Invoke();
    }
}
