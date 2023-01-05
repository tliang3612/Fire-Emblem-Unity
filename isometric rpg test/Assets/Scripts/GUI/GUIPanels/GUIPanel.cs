using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUIPanel : MonoBehaviour
{
    public static GUIState State;

    protected Vector2 topRightPosition;
    protected Vector2 rightPanelPosition;
    protected Vector2 halfViewPoint;

    protected Camera mainCamera;

    public GameObject Panel;

    protected void SetState(GUIState state)
    {
        State = state;
    }

    protected virtual void Awake()
    {       
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        halfViewPoint.x = mainCamera.pixelWidth * 2/3;
        halfViewPoint.y = mainCamera.pixelHeight / 2;
    }

    private void Update()
    {
        Debug.DrawRay(halfViewPoint, Vector3.right);
    }

    protected virtual void Start()
    {
        Panel.SetActive(false);
    }

    public virtual void ReceivePanelPosition(Vector2 topPosition, Vector2 rightPosition)
    {
        topRightPosition = topPosition;
        rightPanelPosition = rightPosition;
    }
    protected virtual void OnAbilitySelected(object sender, EventArgs e)
    {
        SetState(GUIState.InAbilitySelection);
        Panel.SetActive(true);

        //Gets the screen point of the ability's position
        var worldToScreenPoint = mainCamera.WorldToScreenPoint((sender as Ability).UnitReference.transform.position);

        //Check to see if the the ability's transform.x is greater than the center of the screen
        if (worldToScreenPoint.x > halfViewPoint.x)
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
        SetState(GUIState.Clear);
        Panel.SetActive(false);
    }
}
