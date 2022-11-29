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
    public float translationSpeed;
    public float translationDuration;

    private Vector2Int _panDirection;   
    private bool _isPanning;
    private bool _isInDisplayAbility;
    private TileGrid _tileGrid;

    private Camera _camera;
    private Bounds _cameraBounds;

    [SerializeField] private ScreenBorder[] _screenBorders;

    private void Awake()
    {
        _tileGrid = FindObjectOfType<TileGrid>();
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

        _tileGrid.UnitAdded += OnUnitAdded;
    }
    public void OnTileHighlighted(object sender, EventArgs e)
    {
        if (!_isPanning && !_isInDisplayAbility && _camera.isActiveAndEnabled && _panDirection != Vector2Int.zero)
        {
            var newPosition = new Vector3(_camera.transform.position.x + _panDirection.x, _camera.transform.position.y + _panDirection.y);

            var moveTo = ClampCameraToBounds(newPosition);

            _camera.transform.DOMove(moveTo, translationDuration).SetEase(Ease.Linear);
        }
    }

    public void MoveToPoint(Vector3 position)
    {
        if (_camera.isActiveAndEnabled && !_isPanning)
        {
            _isPanning = true;

            var duration = Vector3.Magnitude((position - _camera.transform.position) / translationSpeed);
            var moveTo = ClampCameraToBounds(position);

            _camera.transform.DOMove(moveTo, duration).SetEase(Ease.Linear).OnComplete(() => _isPanning = false);
        }
    }

    public void SetCameraOnUnit(Unit unit)
    {
        StartCoroutine(FollowUnit(unit));
    }

    public IEnumerator FollowUnit(Unit unit)
    {
        while (unit.IsMoving)
        {
            MoveToPoint(unit.transform.position);
            yield return null;
        }
    }

    private Vector3 ClampCameraToBounds(Vector2 position)
    {
        return new Vector3(Mathf.Clamp(position.x, _cameraBounds.min.x, _cameraBounds.max.x),
                           Mathf.Clamp(position.y, _cameraBounds.min.y, _cameraBounds.max.y),
                           -2f);
    }

    public void OnUnitAdded(object sender, UnitCreatedEventArgs e)
    {
        RegisterUnit(e.Unit, e.Abilities);
    }

    public void RegisterUnit(Unit unit, List<Ability> unitAbilities)
    {
        foreach(var ability in unitAbilities)
        {
            if(ability is DisplayAbility)
            {
                ability.AbilitySelected += OnDisplayAbilitySelected;
                ability.AbilityDeselected += OnDisplayAbilityDeselected;
            }
        }
    }

    public void OnDisplayAbilitySelected(object sender, EventArgs e)
    {
        _isInDisplayAbility = true;
    }

    public void OnDisplayAbilityDeselected(object sender, EventArgs e)
    {
        _isInDisplayAbility = false;
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