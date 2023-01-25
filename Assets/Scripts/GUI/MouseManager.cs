using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private TileGrid _tileGrid;

    private void Update()
    {
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                IClickable objectHit = hit.collider.GetComponent<IClickable>();
                objectHit.OnPointerDown();
            }         
        }

        if (Input.GetMouseButtonDown(1))
        {
            _tileGrid.OnRightMouseClicked();
        }

    }
}
