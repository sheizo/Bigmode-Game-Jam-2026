using System;
using UnityEngine;

public class RagdollEnabler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _ragdollRoot;
    [SerializeField] private bool _startRagdoll = false;
    
    private Rigidbody[] _rigidbodies;
    private CharacterJoint[] _joints;
    private Collider[] _colliders;


    private void Awake(){
        _rigidbodies = _ragdollRoot.GetComponentsInChildren<Rigidbody>();
        _joints = _ragdollRoot.GetComponentsInChildren<CharacterJoint>();
        _colliders = _ragdollRoot.GetComponentsInChildren<Collider>();

        if (_startRagdoll)
            EnableRagdoll();
        else
            EnableAnimator();
    }

    public void EnableRagdoll(){
        _animator.enabled = false;

        foreach (CharacterJoint joint in _joints){
            joint.enableCollision = true;
        }
        foreach (Collider col in _colliders){
            col.enabled = true;
        }
        foreach (Rigidbody rb in _rigidbodies){
            rb.linearVelocity = Vector3.zero;
            rb.detectCollisions = true;
            rb.useGravity = true;
        }
        
        // raise the ragdoll root transform a bit
        _ragdollRoot.position += Vector3.up * 0.2f; 
    }

    public void EnableAnimator(){
        _animator.enabled = true;
        
        foreach (CharacterJoint joint in _joints){
            joint.enableCollision = false;
        }
        foreach (Collider col in _colliders){
            if (col.TryGetComponent(out CollisionDetector colD))
                continue; 
            col.enabled = false;
        }
        foreach (Rigidbody rb in _rigidbodies){
            if (rb.TryGetComponent(out CollisionDetector colD))
                continue; 
            rb.detectCollisions = false;
            rb.useGravity = false;
        }
    }
}
