﻿using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }
    
    private void LateUpdate()
    {
        transform.LookAt(
            transform.position + _camera.transform.rotation * Vector3.forward,
            _camera.transform.rotation * Vector3.up);
    }
}
