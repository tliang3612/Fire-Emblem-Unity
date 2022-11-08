using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AbilityGUIController : GUIController
{
    protected override void Awake()
    {
        base.Awake();
        Panel.SetActive(false);
    }

    protected virtual void OnAbilitySelected(object sender, EventArgs e)
    {
        //Gets the screen point of the ability's position
        var worldToScreenPoint = mainCamera.WorldToScreenPoint((sender as Ability).transform.position);
        //Get the center of the screen
        var halfViewPoint = mainCamera.pixelWidth / 2;
        //Check to see if the the ability's transform.x is greater than the center of the screen
        if (worldToScreenPoint.x > halfViewPoint)
        {
            Panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-rightPanelPosition.x, rightPanelPosition.y);
        }
        else
        {
            Panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(rightPanelPosition.x, rightPanelPosition.y);
        }
             
    }

    protected virtual void OnAbilityDeselected(object sender, EventArgs e)
    {

    }
}