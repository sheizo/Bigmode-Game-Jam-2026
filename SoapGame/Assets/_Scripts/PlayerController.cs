using System;
using System.Collections.Generic;
using DG.Tweening;
using Freya;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = Freya.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float _currentSpeed;
    [SerializeField] public float _zSpeed;
    [SerializeField] public float _xSpeed;
    [SerializeField] public float _ySpeed;

    [SerializeField] private List<Ramp> _rampPrefabs;

    [Header("References")] [SerializeField]
    private Transform _playerVisual;

    [SerializeField] private Transform _playerCollision;
    [SerializeField] private CinemachineCamera _playerCamera;

    [Header("Camera")] [SerializeField] private float _maxAddedFov = 20f;
    [SerializeField] private float _maxFOVSpeed = 30f;
    [Range(0, 0.5f)] [SerializeField] private float _cameraFOVSmoothing;

    [Header("Movement")] 
    [SerializeField] private float _maxSpeed = 20;
    [SerializeField] private float _minYSpeed = -90;
    [SerializeField] private float _groundedThreshold = 0.5f;
    [SerializeField] private float _slamStrength = 10;
    [SerializeField] private float _steerStrength = 5;
    [Range(0, 1)] [SerializeField] private float _airSteerMultiplier = 5;
    [Range(0, 1)] [SerializeField] private float _groundSteerMultiplier = 10;
    [Range(0, 1)] [SerializeField] private float _minForwardSpeedSteerMultiplier = 0.1f;
    [SerializeField] private Vector2 _rampHitBonusAccel = new Vector2(-1f, 1f);
    [SerializeField] private float _rampHitDotMax = 0.7f;
    [SerializeField] private float _rampForceDown, _rampForceForward;

    [Header("Ramp Spawning")] 
    [SerializeField] private Vector3 _rampSpawnOffset = new Vector3(0f, -1, 0f);
    [SerializeField] private float _predictionTime = 0.25f;
    [SerializeField] private int _rampQueueSize;
    [SerializeField] private float _rampCheckRaycastLength = 20;
    [SerializeField] private float _rampCheckRaycastRadius = 0.2f;

    [Header("Visual rotations")] 
    [SerializeField] private float _maxXAngle = 20f;
    [SerializeField] private float _maxYAngle = 80f;
    [SerializeField] private float _maxZAngle = 20f;
    [SerializeField] private float _extraSlamAirAngle = 10f;
    [Range(0, 0.5f)] [SerializeField] private float smoothTime = 0.05f;
    [SerializeField] private float _maxRotationXSpeed = 40;
    [SerializeField] private float _maxRotationYSpeed = 8;

    
    private Quaternion _currentRotation;
    private float _currentFOV;

    private Rigidbody _rb;
    private SphereCollider _collider;

    private bool _isMoving;
    private bool _isSlamPressed, _pressedSpawnRampThisFrame, _pressedDiscardRampThisFrame;
    private float _horizontalInput;
    private bool _wasPreviouslyNotOnRamp = true;

    private float _originalCameraFov;
    private Vector3 _originalPosition;

    private Queue<Ramp> _rampQueue;

    bool IsGrounded() => Physics.Raycast(_playerCollision.transform.position, -Vector3.up, _groundedThreshold);

    Ramp IsOnRamp(){
        if (Physics.Raycast(_playerCollision.transform.position, -Vector3.up, out RaycastHit hit, _groundedThreshold)){
            if(hit.collider.CompareTag("Ramp") && hit.collider.TryGetComponent(out Ramp ramp)){
                return ramp;
            }
        }
        return null;
    }


    private void Awake(){
        _rb = _playerCollision.GetComponent<Rigidbody>();
        _collider = _playerCollision.GetComponent<SphereCollider>();

        _originalPosition = transform.position;
        _originalCameraFov = _playerCamera.Lens.FieldOfView;

        _rampQueue = new Queue<Ramp>(_rampQueueSize);
    }

    private void Start(){
        //populate ramp queue
        for (int i = 0; i < _rampQueueSize; i++){
            EnqueueRandomRamp();
        }
    }

    private void Update(){
        SetInputVariables();
        HandleCameraFOV();
        
        HandleRampSpawning();
        HandleRampDespawning();
        

        if (Keyboard.current.rKey.wasPressedThisFrame){
            _playerCamera.Lens.FieldOfView += 10;

            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _playerCollision.transform.position = _originalPosition;
        }
        
    }

    private void FixedUpdate(){
        _currentSpeed = _rb.linearVelocity.magnitude;
        _playerVisual.transform.position = _playerCollision.transform.position;
    
        
        _ySpeed = Vector3.Dot(_rb.linearVelocity, transform.up);
        _xSpeed = Vector3.Dot(_rb.linearVelocity, transform.right);
        _zSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);

        HandleSpeedCapping();
        HandleVisualRotations();

        // Horizontal control -  force based on if ground or air with a clamped multiplier by forward speed
        float steerStrength = _steerStrength * (IsGrounded() ? _groundSteerMultiplier : _airSteerMultiplier);
        float forwardSpeedSteerMult = Mathf.Max(Mathf.Max(0, _zSpeed), _minForwardSpeedSteerMultiplier);
        print(forwardSpeedSteerMult);
        _rb.AddForce(new Vector3(_horizontalInput, 0, 0) * (forwardSpeedSteerMult * steerStrength), ForceMode.Force);

        // Slam Control, only when in air
        if (_isSlamPressed && !IsGrounded()){
            _rb.AddForce(new Vector3(0, -1, 0) * _slamStrength, ForceMode.Force);
        }
        
        Ramp currentRamp = IsOnRamp();
        /*
        //Good landing reward //maybe change to also work on ground
        if (currentRamp && _wasPreviouslyNotOnRamp && _isSlamPressed){ //rewards slamming on good angles, right now slamming while on ramp also gives boost maybe keep
            if (Physics.Raycast(_playerCollision.position, -Vector3.up, out RaycastHit hit)){
                _wasPreviouslyNotOnRamp = false;

                Vector3 normal = hit.normal;
                float dot = Vector3.Dot(normal, transform.forward);

                Debug.Log("Hit ramp at a dot of: " + dot);

                float tDot = Mathf.InverseLerp(-_rampHitDotMax, _rampHitDotMax, dot);
                float multiplier = Mathf.Lerp(_rampHitBonusAccel.x, _rampHitBonusAccel.y, tDot);

                _rb.AddForce(transform.forward * multiplier, ForceMode.Impulse);
            }
        }
        else if (!IsOnRamp()){
            _wasPreviouslyNotOnRamp = true;
        }
        */
        
        if (Physics.Raycast(_playerCollision.transform.position, -Vector3.up, out RaycastHit hit, _groundedThreshold) && currentRamp)
        {
            Vector3 stickDirection = -hit.normal; 

            Vector3 slopeForward = Vector3.ProjectOnPlane(currentRamp.transform.forward, hit.normal).normalized;

            Vector3 force = (stickDirection * _rampForceDown) + (slopeForward * _rampForceForward);
            _rb.AddForce(force, ForceMode.Force);
        }
        
    }
    
    private Ramp GetLastRamp(Ramp start){
        while (start.ConnectedRamp){
            start = start.ConnectedRamp;
        }
        return start;
    }

    //TODO: Migrate to InputSystem, maybe
    private void SetInputVariables(){
        _horizontalInput = Keyboard.current.aKey.isPressed ? -1f : Keyboard.current.dKey.isPressed ? 1f : 0f;
        _isSlamPressed = Keyboard.current.spaceKey.isPressed;
        
        _pressedSpawnRampThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
        _pressedDiscardRampThisFrame = Mouse.current.rightButton.wasPressedThisFrame;

    }

    private void HandleSpeedCapping(){
        if(_rb.linearVelocity.magnitude > _maxSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxSpeed;
        
        // maybe not needed
        if (_ySpeed < _minYSpeed)
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _minYSpeed, _rb.linearVelocity.z);
    }

    private void HandleRampDespawning(){
        if (!_pressedDiscardRampThisFrame) return;
        
        _rampQueue.Dequeue();
        EnqueueRandomRamp();
    }
    
    private void HandleRampSpawning(){
        if (!_pressedSpawnRampThisFrame) return;
        
        Ramp playerOnRamp = IsOnRamp();
        Ramp attachRamp = playerOnRamp ? GetLastRamp(playerOnRamp) : null;
        
        Vector3 velocityYNegative = new Vector3(_rb.linearVelocity.x, -Mathf.Abs(_rb.linearVelocity.y), _rb.linearVelocity.z);
        Vector3 predictedPlayerPosition = _playerCollision.position + (velocityYNegative * _predictionTime);
        
        Vector3 rampSpawnPoint = predictedPlayerPosition + _rampSpawnOffset;
        Vector3 toRampSpawnPoint = rampSpawnPoint - _playerCollision.position;
        
        // Check for ramp 
        if (Physics.SphereCast(_playerCollision.position, _rampCheckRaycastRadius, toRampSpawnPoint.normalized, out RaycastHit rampHit, _rampCheckRaycastLength)){ //prevent ramp spawning below ground
            if (rampHit.transform.CompareTag("Ramp")){
                Debug.DrawRay(_playerCollision.position, toRampSpawnPoint.normalized * rampHit.distance, Color.green, 10f);

                if (rampHit.transform.TryGetComponent(out Ramp hitRamp))
                    attachRamp = GetLastRamp(hitRamp);
                
            };
        }
        
        //Check for ground
        if (Physics.Raycast(_playerCollision.position, toRampSpawnPoint.normalized, out RaycastHit groundHit, toRampSpawnPoint.magnitude)){ //prevent ramp spawning below ground
            rampSpawnPoint = groundHit.point;
            Debug.DrawRay(_playerCollision.position, toRampSpawnPoint.normalized * groundHit.distance, Color.red, 10f);
            if (_rampQueue.Peek().StartingDirection == RampStartingDirection.Down){
                Debug.Log("cant spawn that");
                return;
            }
        }
        
        // set ramp rotation to movement, don't if player isnt moving forwards
        Vector3 moveDir = new Vector3(_xSpeed,0 ,_zSpeed).normalized;
        Quaternion rampRotation = (attachRamp) 
            ? attachRamp.transform.rotation 
            : (_zSpeed < 0.1f) ? Quaternion.identity : Quaternion.LookRotation(moveDir, Vector3.up);

        if (attachRamp) rampSpawnPoint = attachRamp.transform.TransformPoint(attachRamp.EndPoint);
        
        
        Ramp rampPrefab = _rampQueue.Dequeue();
        EnqueueRandomRamp();
        
        Matrix4x4 spawnMatrix = Matrix4x4.TRS(rampSpawnPoint, rampRotation, Vector3.one);
        Vector3 spawnPosition = spawnMatrix.MultiplyPoint(-rampPrefab.StartPoint); //Vector.zero gives position in previous ramp start point
        
        Ramp spawnedRamp = Instantiate(rampPrefab, spawnPosition, rampRotation );
        
        if (attachRamp){
            attachRamp.ConnectedRamp = spawnedRamp;
        }

    }

    private void HandleCameraFOV(){
        float maxFOV = _originalCameraFov + _maxAddedFov;

        float targetFov = Mathf.Lerp(_originalCameraFov, maxFOV, _currentSpeed / _maxFOVSpeed);
        float smoothedTarget = Mathf.SmoothDamp(_playerCamera.Lens.FieldOfView, targetFov, ref _currentFOV, 0.1f);

        _playerCamera.Lens.FieldOfView = smoothedTarget;
    }

    private void HandleVisualRotations(){
        Quaternion finalRotation = Quaternion.identity;
        // rotate to ground, order important to other rotations
        if (IsGrounded()){
            if (Physics.Raycast(_playerCollision.position, -Vector3.up, out RaycastHit hit, _groundedThreshold)){
                Vector3 up = hit.normal;
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
                Quaternion groundAlignedRotation = Quaternion.LookRotation(forward, up);

                finalRotation = groundAlignedRotation;
            }
        } //else apply air rotation
        else{
            float normalizedVerticalSpeed = Mathf.Clamp(_ySpeed / _maxRotationYSpeed, -1, 1);
            Vector2 upDirection = new Vector2(normalizedVerticalSpeed, 0);
            float airPitchAngle =
                -normalizedVerticalSpeed *
                (_maxXAngle + (_isSlamPressed ? _extraSlamAirAngle : 0)); // extra angle if slamming

            // add to final
            Quaternion airTurnRotation = Quaternion.Euler(airPitchAngle, 0, 0);
            finalRotation *= airTurnRotation;
        }


        // rotate to turn direction, math can be same as air rotation
        //Vector2 turnDirection = new Vector2(finalRotation.x,-_horizontalInput); //turn direction with x axis to make it rotate less on ramps, could be 0 instead not that much difference
        //float turnAngle = Mathfs.DirToAng(turnDirection) * Mathf.Rad2Deg;
        //float tAngle = Mathf.InverseLerp(-90, 90, turnAngle);
        //turnAngle = Mathf.LerpAngle(-_maxTurnAngle, _maxTurnAngle, tAngle);
        float turnPitchAngle = -_horizontalInput * _maxZAngle;
        // add to final
        //Quaternion turnRotation = Quaternion.Euler(0,0,turnAngle);
        Quaternion turnRotation = Quaternion.Euler(0, 0, turnPitchAngle);
        finalRotation *= turnRotation;

        // Horizontal - Y rotation
        float normalizedHorizontalSpeed = Mathf.Clamp(_xSpeed / _maxRotationXSpeed, -1, 1);
        float horizontalPitchAngle = normalizedHorizontalSpeed * _maxYAngle;

        Quaternion horizontalRotation = Quaternion.Euler(0, horizontalPitchAngle, 0);
        finalRotation *= horizontalRotation;

        _playerVisual.rotation = QuaternionUtil.SmoothDamp(_playerVisual.rotation, finalRotation, ref _currentRotation,
            smoothTime, true);
    }

    private void EnqueueRandomRamp() =>
        _rampQueue.Enqueue(_rampPrefabs[Random.Range(0, _rampPrefabs.Count)]);



private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_playerCollision.transform.position, -Vector3.up*_groundedThreshold);
        
        Gizmos.DrawSphere(_playerCollision.position + _rampSpawnOffset, 0.3f);
    }
}
