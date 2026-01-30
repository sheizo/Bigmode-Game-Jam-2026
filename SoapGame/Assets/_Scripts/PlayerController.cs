using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _playerVisual;
    [SerializeField] private Transform _playerCollision;

    [SerializeField] private float _groundedThreshold = 0.5f;
    [SerializeField] private float _slamStrength = 10;
    [SerializeField] private float _airSteerStrength = 5;
    [SerializeField] private float _groundSteerStrength = 10;
    [SerializeField] private float _minimumSteerMultiplier = 0.1f;
    [SerializeField] private Vector2 _rampHitBonusAccel = new Vector2(-1f, 1f);
    [SerializeField] private float _rampHitDotMax = 0.7f;

    [SerializeField] private float smoothTime = 0.05f;
    
    private Quaternion _currentRotation;
    
    private Rigidbody _rb;
    private SphereCollider _collider;

    private bool _isMoving, _isSlamPressed;
    private float _horizontalInput;
    private bool _wasPreviouslyNotOnRamp = true;

    private Vector3 _originalPosition;
    
    
    
    bool IsGrounded() => Physics.Raycast(_playerCollision.transform.position, -Vector3.up, _groundedThreshold);

    bool IsOnRamp(){
        if(Physics.Raycast(_playerCollision.transform.position, -Vector3.up, out RaycastHit hit, _groundedThreshold))  return hit.collider.CompareTag("Ramp"); 
        
        return false;
    }
    
    private void Awake(){
        _rb = _playerCollision.GetComponent<Rigidbody>();
        _collider = _playerCollision.GetComponent<SphereCollider>();
        
        _originalPosition = transform.position;
    }

    private void Update(){
        _horizontalInput = Keyboard.current.aKey.isPressed ? -1f : Keyboard.current.dKey.isPressed ?  1f : 0f;
        
         _isSlamPressed = Keyboard.current.spaceKey.isPressed;

         if (Keyboard.current.rKey.wasPressedThisFrame){
             _rb.linearVelocity = Vector3.zero;
             _rb.angularVelocity = Vector3.zero;
             _playerCollision.transform.position = _originalPosition;
         }
         
    }

    private void FixedUpdate(){
        _playerVisual.transform.position = _playerCollision.transform.position;
        HandleVisualRotations();
        
        
        // Horizontal control -  force based on if ground or air with a clamped multiplier by forward speed
        float forwardSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward.normalized);
        float steerStrength = IsGrounded() == true ? _groundSteerStrength : _airSteerStrength;
        float steerMultiplier = Mathf.Min(Mathf.Max(0, forwardSpeed), _minimumSteerMultiplier);
        _rb.AddForce(new Vector3(_horizontalInput, 0, 0) * ( steerMultiplier * steerStrength), ForceMode.Force); 
        
        // Slam Control
        if (_isSlamPressed){
            _rb.AddForce(new Vector3(0, -1, 0) * _slamStrength, ForceMode.Force);   
        }
        
        //Good landing reward
        if (IsOnRamp() && _wasPreviouslyNotOnRamp){
            if (Physics.Raycast(_playerCollision.transform.position, -Vector3.up, out RaycastHit hit)){
                _wasPreviouslyNotOnRamp = false;
                
                Vector3 normal = hit.normal;
                float dot = Vector3.Dot(normal, transform.forward.normalized);
                
                Debug.Log("Hit ramp at a dot of: " + dot);
                
                float tDot = Mathf.InverseLerp(-_rampHitDotMax, _rampHitDotMax, dot);
                float multiplier = Mathf.Lerp(_rampHitBonusAccel.x, _rampHitBonusAccel.y, tDot);
                
                _rb.AddForce(transform.forward.normalized*multiplier, ForceMode.Impulse);
            }
        }
        else if(!IsOnRamp()){
            _wasPreviouslyNotOnRamp = true;
        }
        
        
    }

    private void HandleVisualRotations(){
        if (IsGrounded()){ // rotate to ground
            if (Physics.Raycast(_playerCollision.position, -Vector3.up, out RaycastHit hit, _groundedThreshold)){
                Vector3 up = hit.normal;
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward.normalized, up).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(forward, up);

                
                _playerVisual.rotation = QuaternionUtil.SmoothDamp(_playerVisual.rotation, targetRotation, ref _currentRotation, smoothTime, true);

            } 
        }
    }


    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_playerCollision.transform.position, -Vector3.up*_groundedThreshold);
    }
}
