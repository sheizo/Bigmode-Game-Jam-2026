using System;
using System.Collections.Generic;
using DG.Tweening;
using Freya;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = Freya.Random;
using Sequence = DG.Tweening.Sequence;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private bool _useUpgrades; 
    
    [SerializeField] private float _soapRefillOnClean = 0.1f;
    [SerializeField] private int _maxSoap = 10;
    [SerializeField] private int _rampSoapCost = 1;
    
    [Header("Prefabs")]
    [SerializeField] private List<Ramp> _rampPrefabs;
    [SerializeField] private ParticleSystem _rampSpawnParticlesPrefab;
    
    [Header("References")] 
    [SerializeField] private PlayerProgressHubSO _playerHubSO;
    [SerializeField] private UpgradeValuesSO _upgradeValuesSo;
    [SerializeField] private Transform _playerVisual;
    [SerializeField] private CinemachineCamera _playerCamera;

    [Header("Camera")] 
    [SerializeField] private float _maxAddedFov = 20f;
    [Range(0, 1f)] [SerializeField] private float _cameraFOVSmoothing;

    [Header("Movement")] 
    [SerializeField] private float _maxAirSpeed = 20;
    [SerializeField] private float _maxGroundSpeed = 10;
    [SerializeField] private float _minYSpeed = -90;
    [SerializeField] private float _groundedThreshold = 0.5f;
    [SerializeField] private float _slamStrength = 10;
    [SerializeField] private float _steerStrength = 5;
    [Range(0, 1)] [SerializeField] private float _airSteerMultiplier = 5;
    [Range(0, 1)] [SerializeField] private float _groundSteerMultiplier = 10;
    [Range(0, 1)] [SerializeField] private float _minForwardSpeedSteerMultiplier = 0.1f;
    [SerializeField] private float _rampForceForward, _rampForceDown;

    [Header("Ramp Spawning")] 
    [SerializeField] private float _rampSpawnParticlesTravelTime = 0.2f; 
    [SerializeField] private Vector3 _rampSpawnOffset = new Vector3(0f, -1, 0f);
    [SerializeField] private float _predictionTime = 0.25f;
    [SerializeField] private int _rampQueueSize;
    [SerializeField] private float _rampCheckRaycastLength = 20;
    [SerializeField] private float _rampCheckRaycastRadius = 0.2f;
    [Range(0,1)] [SerializeField] private float _badRampChance = 0.5f;

    [Header("Visual rotations")] 
    [SerializeField] private float _maxXAngle = 20f;
    [SerializeField] private float _maxYAngle = 80f;
    [SerializeField] private float _maxZAngle = 20f;
    [SerializeField] private float _extraSlamAirAngle = 10f;
    [Range(0, 0.5f)] [SerializeField] private float smoothTime = 0.05f;

    private Queue<Ramp> _rampQueue;
    private Rigidbody _rb;
    private SphereCollider _collider;
    private PhysicsMaterial _physicsMaterial;
    
    private Quaternion _currentRotation;
    private float _currentFOV;
    private float _originalCameraFov;
    private Vector3 _originalPosition;
    private int _currentSoapPower;

    private float _lastTimeOnGround, _timeOnGround;

    private bool _isMoving;
    private bool _isGrounded;
    private bool _isOnRamp;
    private Ramp _rampPlayerIsOn;
    private bool _isSlamPressed, _pressedSpawnRampThisFrame, _pressedDiscardRampThisFrame;
    private float _horizontalInput;
    private float _currentSpeed;
    private float _zSpeed;
    private float _xSpeed;
    private float _ySpeed;

    private RampDirection _lastRampEndDirection = RampDirection.Down;
    private List<Ramp> _upFacingRamps = new List<Ramp>();
    private List<Ramp> _downFacingRamps = new List<Ramp>();
    private List<Ramp> _straightFacingRamps = new List<Ramp>();
    
    


    bool IsGrounded() => Physics.Raycast(transform.position, -Vector3.up, _groundedThreshold);

    Ramp IsOnRamp(){
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, _groundedThreshold)){
            if(hit.collider.CompareTag("Ramp") && hit.collider.TryGetComponent(out Ramp ramp)){
                return ramp;
            }
        }
        return null;
    }

    private void Awake(){
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        _physicsMaterial = _collider.material;
            
        _originalPosition = transform.position;
        _originalCameraFov = _playerCamera.Lens.FieldOfView;

        _rampQueue = new Queue<Ramp>(_rampQueueSize);

        
    }

    private void Start(){
        foreach (Ramp ramp in _rampPrefabs){
            if(ramp.StartingDirection == RampDirection.Down) _downFacingRamps.Add(ramp);
            else if(ramp.StartingDirection == RampDirection.Up) _upFacingRamps.Add(ramp);
            else if(ramp.StartingDirection == RampDirection.Straight) _straightFacingRamps.Add(ramp);
        }
        //populate ramp queue
        for (int i = 0; i < _rampQueueSize; i++){
            EnqueueRandomRamp();
        }

        if(_useUpgrades) UpdateUpgrades();
        RefillSoap();
    }

    private void Update(){
        SetInputVariables();
        HandleCameraFOV();
        
        HandleRampSpawning();
        HandleRampDespawning();
        
        
        //TODO: delete
        if (Keyboard.current.rKey.wasPressedThisFrame){
            _playerCamera.Lens.FieldOfView += 10;

            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = _originalPosition;
        }

        //count the time on ground
        if (_isGrounded && !_rampPlayerIsOn)
            _timeOnGround += Time.deltaTime;
        else
            _timeOnGround = 0;
    }

    private void FixedUpdate(){
        _currentSpeed = _rb.linearVelocity.magnitude;
        _ySpeed = Vector3.Dot(_rb.linearVelocity, Vector3.up);
        _xSpeed = Vector3.Dot(_rb.linearVelocity, Vector3.right);
        _zSpeed = Vector3.Dot(_rb.linearVelocity, Vector3.forward);
        
        _isGrounded = IsGrounded();
        _rampPlayerIsOn = IsOnRamp();
        
        HandleSpeedCapping();
        HandleVisualRotations();
        HandleMovement();
        
        SetPlayerVisualPosition();
    }

    private void SetPlayerVisualPosition(){
        _playerVisual.transform.position = transform.position;
    }

    //TODO: Migrate to InputSystem, maybe
    private void SetInputVariables(){
        _horizontalInput = Keyboard.current.aKey.isPressed ? -1f : Keyboard.current.dKey.isPressed ? 1f : 0f;
        _isSlamPressed = Keyboard.current.spaceKey.isPressed;
        
        _pressedSpawnRampThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
        _pressedDiscardRampThisFrame = Mouse.current.rightButton.wasPressedThisFrame;
    }
    

    private void HandleMovement(){
        // Horizontal control -  force based on if ground or air with a clamped multiplier by forward speed
        float steerStrength = _steerStrength * (_isGrounded ? _groundSteerMultiplier : _airSteerMultiplier);
        float forwardSpeedSteerMult = Mathf.Max(Mathf.Max(0, _zSpeed), _minForwardSpeedSteerMultiplier);
        _rb.AddForce(new Vector3(_horizontalInput, 0, 0) * (forwardSpeedSteerMult * steerStrength), ForceMode.Force);
        // Slam Control, only when in air
        if (_isSlamPressed && !_isGrounded){
            _rb.AddForce(new Vector3(0, -1, 0) * _slamStrength, ForceMode.Force);
        }
        // Good angle check -0.7 - to 0.7 +, good angle is anything above 0
        // Vector3 normal = hit.normal;
        // float dot = Vector3.Dot(normal, transform.forward);
        
        // Push along ramp
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, _groundedThreshold) && _rampPlayerIsOn)
        {
            Vector3 stickDirection = -hit.normal; 

            Vector3 slopeForward = Vector3.ProjectOnPlane(_rampPlayerIsOn.transform.forward, hit.normal).normalized;

            Vector3 force = (stickDirection * _rampForceDown) + (slopeForward * _rampForceForward);
            _rb.AddForce(force, ForceMode.Force);
        }
    }

    private void HandleSpeedCapping(){
        // maybe not needed
        if (_ySpeed < _minYSpeed)
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _minYSpeed, _rb.linearVelocity.z);
        
        if (_currentSpeed > _maxGroundSpeed && _isGrounded && !_rampPlayerIsOn){
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxGroundSpeed;
            return; // exit early so maxSeed doesn't cap air speed
        }

        if (_currentSpeed > _maxAirSpeed){
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxAirSpeed;
            return;
        }
        
    }

    private void HandleRampDespawning(){
        if (!_pressedDiscardRampThisFrame) return;
        
        _rampQueue.Dequeue();
        EnqueueRandomRamp();
    }
    
    private void HandleRampSpawning(){
        if (!_pressedSpawnRampThisFrame || _currentSoapPower <= 0) return;

        _currentSoapPower--;
        
        Ramp attachRamp = _rampPlayerIsOn ? GetLastAttachedRamp(_rampPlayerIsOn) : null;
        
        Vector3 velocityYNegative = new Vector3(_rb.linearVelocity.x, -Mathf.Abs(_rb.linearVelocity.y), _rb.linearVelocity.z);
        Vector3 predictedPlayerPosition = transform.position + (velocityYNegative * _predictionTime);
        
        Vector3 rampSpawnPoint = predictedPlayerPosition + _rampSpawnOffset;
        Vector3 toRampSpawnPoint = rampSpawnPoint - transform.position;
        
        // Check for ramp 
        if (Physics.SphereCast(transform.position, _rampCheckRaycastRadius, toRampSpawnPoint.normalized, out RaycastHit rampHit, _rampCheckRaycastLength)){ //prevent ramp spawning below ground
            if (rampHit.transform.CompareTag("Ramp")){
                Debug.DrawRay(transform.position, toRampSpawnPoint.normalized * rampHit.distance, Color.green, 10f);

                if (rampHit.transform.TryGetComponent(out Ramp hitRamp))
                    attachRamp = GetLastAttachedRamp(hitRamp);
                
            };
        }
        //Check for ground
        if (!attachRamp && Physics.Raycast(transform.position, toRampSpawnPoint.normalized, out RaycastHit groundHit, toRampSpawnPoint.magnitude)){ //prevent ramp spawning below ground
            rampSpawnPoint = groundHit.point;
            Debug.DrawRay(transform.position, toRampSpawnPoint.normalized * groundHit.distance, Color.red, 10f);
            if (_rampQueue.Peek().StartingDirection == RampDirection.Down){
                print("cant spawn that");
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
        
        //Visuals - particle line
        Vector3 startPos = transform.position;
        Vector3 endPos = spawnPosition;

        ParticleSystem spawnedParticles = Instantiate(_rampSpawnParticlesPrefab, startPos, Quaternion.identity);
        
        Sequence particleSequence = DOTween.Sequence();
        particleSequence.Append(spawnedParticles.transform.DOMove(endPos, _rampSpawnParticlesTravelTime));
        particleSequence.AppendInterval(spawnedParticles.main.startLifetime.constantMax  + _rampSpawnParticlesTravelTime);
        particleSequence.AppendCallback(() => Destroy(spawnedParticles.gameObject));
    }

    private void HandleCameraFOV(){
        float maxFOV = _originalCameraFov + _maxAddedFov;

        float targetFov = Mathf.Lerp(_originalCameraFov, maxFOV, _currentSpeed / _maxAirSpeed);
        float smoothedTarget = Mathf.SmoothDamp(_playerCamera.Lens.FieldOfView, targetFov, ref _currentFOV, 0.1f);

        _playerCamera.Lens.FieldOfView = smoothedTarget;
    }

    private void HandleVisualRotations(){
        Quaternion finalRotation = Quaternion.identity;
        // rotate to ground, order important to other rotations
        if (IsGrounded()){
            if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, _groundedThreshold)){
                Vector3 up = hit.normal;
                Vector3 forward = Vector3.ProjectOnPlane(Vector3.forward, up).normalized;
                Quaternion groundAlignedRotation = Quaternion.LookRotation(forward, up);

                finalRotation = groundAlignedRotation;
            }
        } //else apply air rotation
        else{
            float normalizedVerticalSpeed = Mathf.Clamp(_ySpeed / _maxAirSpeed, -1, 1);
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
        float normalizedHorizontalSpeed = Mathf.Clamp(_xSpeed / _maxAirSpeed, -1, 1);
        float horizontalPitchAngle = normalizedHorizontalSpeed * _maxYAngle;

        Quaternion horizontalRotation = Quaternion.Euler(0, horizontalPitchAngle, 0);
        finalRotation *= horizontalRotation;

        _playerVisual.rotation = QuaternionUtil.SmoothDamp(_playerVisual.rotation, finalRotation, ref _currentRotation,
            smoothTime, true);
    }

    public void UpdateUpgrades(){
        PlayerStats stats = _playerHubSO.LiveData;
        _maxAirSpeed = _upgradeValuesSo.MaxSpeed[stats.MaxSpeed];
        _steerStrength = _upgradeValuesSo.TurnStrength[stats.TurnStrength];
        _maxSoap = _upgradeValuesSo.MaxSoap[stats.MaxSoap];
        _soapRefillOnClean = _upgradeValuesSo.SoapRefillOnClean[stats.SoapRefillOnClean];
        _rampForceDown = _upgradeValuesSo.RampBoostSpeed[stats.RampBoostSpeed].x;
        _rampForceForward = _upgradeValuesSo.RampBoostSpeed[stats.RampBoostSpeed].y;
        _slamStrength = _upgradeValuesSo.SlamForce[stats.SlamForce];
        _physicsMaterial.bounciness = _upgradeValuesSo.Bounciness[stats.Bounciness];
    }

    public void RefillSoap() => AddSoapPower(1);
    public void AddSoapPower(float normalizedAmount) => _currentSoapPower = Mathf.Lerp(0, _maxSoap, normalizedAmount).CeilToInt();
    
    private Ramp GetLastAttachedRamp(Ramp start){
        while (start.ConnectedRamp){
            start = start.ConnectedRamp;
        }
        return start;
    }

    private void EnqueueRandomRamp(){
        List<Ramp> ramps;
        
        
        //chance to queue a bad ramp
        if (Random.Value < _badRampChance){ 
            RampDirection[] values = (RampDirection[])System.Enum.GetValues(typeof(RampDirection));
            _lastRampEndDirection = values[Random.Range(0, values.Length)];
        } 
        
        if (_lastRampEndDirection == RampDirection.Down){
            ramps = _downFacingRamps;
        }
        else if(_lastRampEndDirection == RampDirection.Up){
            ramps = _upFacingRamps;
        }
        else{
            ramps = _straightFacingRamps;
        }
        
        int totalCount = ramps.Count;
        int randomIndex = Random.Range(0, totalCount);
        Ramp ramp = ramps[randomIndex];
        
        _rampQueue.Enqueue(ramp);
        UIManager.Instance.SetRampSprites(_rampQueue);
        _lastRampEndDirection = ramp.EndingDirection;
    }

    [ContextMenu("Force Update Upgrades")]
    private void ForceUpdateUpgrades(){
        if (!Application.isPlaying) return;
        UpdateUpgrades();
    }
    
    private void OnDrawGizmos(){
        if(!Application.isPlaying) SetPlayerVisualPosition();
    
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -Vector3.up*_groundedThreshold);
        
        Gizmos.DrawSphere(transform.position + _rampSpawnOffset, 0.3f);
    }
}
