using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;

    void Awake()
    {
        if (!_targetCamera)
            _targetCamera = Camera.main;
    }

    void LateUpdate() 
    { 
        if (_targetCamera)
        {
            transform.LookAt(_targetCamera.transform);
        }
    }
}
