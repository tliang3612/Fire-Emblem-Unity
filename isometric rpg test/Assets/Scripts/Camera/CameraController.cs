using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;

public class CameraController : MonoBehaviour
{
    private Vector2Int _panDirection;
    public float translationSpeed;
    public float translationDuration;
    private bool _isPanning;

    private Camera _camera;
    private Bounds _cameraBounds;
    private Vector2 _halfViewPoint;

    [SerializeField] private ScreenBorder[] _screenBorders;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _cameraBounds = FindObjectOfType<Tilemap>().localBounds;

        var halfCameraWidth = _camera.orthographicSize * _camera.aspect;
        var halfCameraHeight = _camera.orthographicSize;

        _cameraBounds.max = new Vector3(_cameraBounds.max.x - halfCameraWidth, _cameraBounds.max.y - halfCameraHeight, -2);
        _cameraBounds.min = new Vector3(_cameraBounds.min.x + halfCameraWidth + 1.9f, _cameraBounds.min.y + halfCameraHeight, -2);

        foreach (var border in _screenBorders)
        {
            border.SetupActions(OnMouseEnterScreenBorder, OnMouseExitScreenBorder);
        }
    }  


    public void MoveToPoint(Vector3 position)
    {
        if (_camera.isActiveAndEnabled)
        {
            _isPanning = true;

            var duration = Vector3.Magnitude((position - _camera.transform.position) / translationSpeed);
            var moveTo = new Vector3(Mathf.Clamp(position.x, _cameraBounds.min.x, _cameraBounds.max.x),
                                     Mathf.Clamp(position.y, _cameraBounds.min.y, _cameraBounds.max.y),
                                     -2f);

            _camera.transform.DOMove(moveTo, duration).SetEase(Ease.Linear).OnComplete(() => _isPanning = false);
        }
    }

    public void FollowUnit(Unit unit)
    {

    }

    public void OnTileHighlighted(object sender, EventArgs e)
    {
        if (!_isPanning && _camera.isActiveAndEnabled && _panDirection != Vector2Int.zero)
        {
            var translationDirection = new Vector3(_panDirection.x, _panDirection.y, 0);

            var moveTo = new Vector3(Mathf.Clamp(_camera.transform.position.x +  translationDirection.x, _cameraBounds.min.x, _cameraBounds.max.x),
                                     Mathf.Clamp(_camera.transform.position.y + translationDirection.y, _cameraBounds.min.y, _cameraBounds.max.y),
                                     -2f);

            _camera.transform.DOMove(moveTo, translationDuration).SetEase(Ease.Linear);
        }
    }


    public void OnMouseEnterScreenBorder(Vector2Int direction)
    {
        _panDirection = direction;       
    }

    public void OnMouseExitScreenBorder()
    {
        _panDirection = Vector2Int.zero;
    }
}