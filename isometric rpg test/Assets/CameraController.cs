using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    public IEnumerator FollowUnit(Unit unit)
    {
        
        yield return null;

    }

    public void ShiftPosition(Vector3 newPosition)
    {

    }
}
