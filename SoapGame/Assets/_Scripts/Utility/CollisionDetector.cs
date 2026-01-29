using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour
{
    private Action<Collision> _onCollisionStay, _onCollisionEnter, _onCollisionExit;
    private Action<Collider> _onTriggerStay, _onTriggerEnter, _onTriggerExit;

    private Collider _collider;
    private Rigidbody _rigidBody;
    
    public void Init(
        Action<Collision> onCollisionStay = null, Action<Collision> onCollisionEnter = null, Action<Collision> onCollisionExit = null,
        Action<Collider> onTriggerStay = null, Action<Collider> onTriggerEnter = null, Action<Collider> onTriggerExit = null
        )
    {
        _collider = GetComponent<Collider>();
        if (TryGetComponent(out Rigidbody rb)){
            _rigidBody = rb;
        }
        
        _onCollisionStay = onCollisionStay;
        _onCollisionEnter = onCollisionEnter;
        _onCollisionExit = onCollisionExit;
        
        _onTriggerStay = onTriggerStay;
        _onTriggerEnter = onTriggerEnter;
        _onTriggerExit = onTriggerExit;
    }

    private void OnCollisionStay(Collision other)
    {
        _onCollisionStay?.Invoke(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        _onCollisionEnter?.Invoke(other);
    }

    private void OnCollisionExit(Collision other)
    {
        _onCollisionExit?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        _onTriggerStay?.Invoke(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        _onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _onTriggerExit?.Invoke(other);
    }
}
