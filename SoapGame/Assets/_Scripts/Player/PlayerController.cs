using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Random = Freya.Random;
using Sequence = DG.Tweening.Sequence;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class PlayerController : MonoBehaviour
{

    [SerializeField] private bool _useUpgrades; 
    
    [SerializeField] private TrailRenderer _soapTrail;

    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private float _maxSpeedMultiplier;
    [SerializeField] private float _maxDistanceSpeed;
    
    [Header("Soap Usage")]
    [SerializeField] private float _soapRefillOnClean = 0.1f;
    [SerializeField] private float _maxSoapPower = 10;
    [SerializeField] private float _rampSoapCost = 2;
    [Range(0,1)] [SerializeField] private float _groundSoapUsageSecPercent = 0.1f;
    [SerializeField] private float _currentSoapPower;
    [SerializeField] private AnimationCurve _soapScaleCurve;
    [SerializeField] private float _scaleTweenDuration = 0.2f;
    [Range(0,1)] [SerializeField] private float _minSoapPercentForRamp = 0.1f;
    
    [Header("Cleaning")]
    [SerializeField] private Vector3 _cleanBoostDirection;
    [SerializeField] private float _cleanBoostStrength = 20;
    [SerializeField] private float _timeToClean = 2f;
    [Range(0,1)] [SerializeField] private float _cleanSoapUsageSec = 0.1f;
    [Range(0,1)] [SerializeField] private float _cleanSoapRefill = 0.1f;
    [SerializeField] private float _cleanHitStopTime = 0.15f;
    [SerializeField] private float _cleanAnimationDuration = 0.3f;
    [SerializeField] private Ease _cleanAnimationEase;
    [SerializeField] private float _cleanAnimationScale = 1.2f;
    [Range(0,1)] [SerializeField] private float _cleaningMinYScale = 0.3f;

    
    [Header("Ramp Spawning")] 
    [SerializeField] private float _rampSpawnParticlesTravelTime = 0.2f; 
    [SerializeField] private Vector3 _rampSpawnOffset = new Vector3(0f, -1, 0f);
    [SerializeField] private float _predictionTime = 0.25f;
    [SerializeField] private int _rampQueueSize;
    [SerializeField] private float _rampCheckRaycastLength = 20;
    [SerializeField] private float _rampCheckRaycastRadius = 0.2f;
    [Range(0,1)] [SerializeField] private float _badRampChance = 0.5f;
    
    [Header("Prefabs")]
    [SerializeField] private List<Ramp> _rampPrefabs;
    [SerializeField] private ParticleSystem _rampSpawnParticlesPrefab;
    
    [Header("References")] 
    [SerializeField] private Transform _playerVisual;
    [SerializeField] private Transform _playerVisualAnims;
    [SerializeField] private CinemachineCamera _playerCamera;

    [Header("Camera")] 
    [Range(0, 1)] [SerializeField] private float _minSoapFOVMultiplier = 0.6f; 
    [SerializeField] private float _maxAddedFov = 20f;
    [Range(0, 1f)] [SerializeField] private float _cameraFOVSmoothing;

    [Header("Movement")] 
    [SerializeField] private float _maxAirSpeed = 20;
    [SerializeField] private float _maxGroundSpeed = 10;
    [Tooltip("Brake speed for when player reaches a max speed")] [SerializeField] private float _maxSpeedBraking = 30f; 
    [SerializeField] private float _groundedThreshold = 0.5f;
    [SerializeField] private float _slamStrength = 10;
    [Range(0,1)] [SerializeField] private float _slamForwardMult = 0.1f;
    [SerializeField] private float _steerStrength = 5;
    [Range(0, 1)] [SerializeField] private float _airSteerMultiplier = 1;
    [Range(0, 1)] [SerializeField] private float _groundSteerMultiplier = 10;
    [Range(0, 1)] [SerializeField] private float _minForwardSpeedSteerMultiplier = 0.1f;
    [SerializeField] private float _rampForceForward, _rampForceDown;

    [Header("Visual rotations")] 
    [SerializeField] private float _maxXAngle = 20f;
    [SerializeField] private float _maxYAngle = 80f;
    [SerializeField] private float _maxZAngle = 20f;
    [SerializeField] private float _extraSlamAirAngle = 10f;
    [Range(0, 0.5f)] [SerializeField] private float smoothTime = 0.05f;

    [Header("Sounds")] 
    [SerializeField] private AudioSource _meltAudioSource;
    [SerializeField] private AudioSource _cleanAudioSource;

    [SerializeField] private AudioSource _rampStayAudioSource;
    [SerializeField] private float _rampStayFadeSpeed = 1;
    [SerializeField] private float _rampTimeForExitPlay = 0.1f;
    [Tooltip("Compared to max speed")] [Range(0,1)] [SerializeField] private float _rampExitSpeedThreshold = 0.3f;
    [SerializeField] private float _minimumRampCreationVolume = 0.3f;
    [SerializeField] private float _maxRampCreationDistance = 30;

    
    private Queue<Ramp> _rampQueue;
    private Rigidbody _rb;
    private SphereCollider _collider;
    private PhysicsMaterial _physicsMaterial;

    private Vector3 _startingPosition;
    private Quaternion _currentRotation;
    private float _currentFOV;
    private float _originalCameraFov;
    private Vector3 _originalPosition;
    private float _lastTimeOnGround, _timeOnGround;
    
    private float _timeCleaning;
    private bool _isMoving;
    private bool _isGrounded;
    private bool _isOnRamp, _wasOnRamp;
    private float _onRampAudioTimer;
    private bool _hitRampSoundExit;
    private Ramp _rampPlayerIsOn;
    private bool _isSlamPressed, _pressedSpawnRampThisFrame, _pressedDiscardRampThisFrame;
    private float _horizontalInput;
    private float _currentSpeed;
    private float _zSpeed, _xSpeed, _ySpeed;

    private Tween _cleanTween;
    
    private RampDirection _lastRampEndDirection = RampDirection.Down;
    private List<Ramp> _upFacingRamps = new List<Ramp>();
    private List<Ramp> _downFacingRamps = new List<Ramp>();
    private List<Ramp> _straightFacingRamps = new List<Ramp>();

    private RunStats _runStats;
    public Action<RunStats> OnSoapDeplete;

    private float GroundedThresholdScaled => _groundedThreshold * transform.localScale.y;
    public bool SoapDepleted => _currentSoapPower <= 0;
    public float TimeToClean => _timeToClean;
    

    bool IsGrounded()=>Physics.Raycast(transform.position, -Vector3.up, GroundedThresholdScaled);

    bool IsCleaning(){
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, GroundedThresholdScaled)){
            if(hit.collider.TryGetComponent(out PlayerInteractable playerInteractable)){
                return playerInteractable.Cleanable;
            }
        }
        return false;
    }
    
    Ramp IsOnRamp(){
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, GroundedThresholdScaled)){
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
        _startingPosition = transform.position;
        
        ResetPlayer();
    }

    private void Update(){
        //Control the sounds
        if (GameManager.CurrentGameState != GameState.GAMEPLAY) return;
        
        bool isCleaning = IsCleaning();
        
        SetControlVariables();
        HandleGroundSoapDeplete(isCleaning);
        HandleCameraFOV();
        HandleRampSpawningAndDiscarding();
        HandleSpeedRamping();
        
        //count the time on ground
        if (_isGrounded && !_rampPlayerIsOn)
            _timeOnGround += Time.deltaTime;
        else
            _timeOnGround = 0;
        
        //trail
        _soapTrail.emitting = _isGrounded && !_isOnRamp;
        _soapTrail.widthMultiplier = _currentSoapPower / _maxSoapPower;
        
        UpdateRunStats();
        
        GameManager.UIManager.UpdateCurrentRunStats(_runStats);
        GameManager.UIManager.UpdatePlayerSpeed((int) _currentSpeed);

        
    }

    private void HandleSpeedRamping(){
        float distanceTravelled = Mathf.Max(0,transform.position.z - _startingPosition.z);
        print(distanceTravelled);
        float t = Mathf.InverseLerp(0, _maxDistanceSpeed,distanceTravelled);
        float evaluatedT = _speedCurve.Evaluate(t);
        float speed = Mathf.Lerp(1, _maxAirSpeed, evaluatedT);
        print(speed);
    }


    private void FixedUpdate(){
        _currentSpeed = _rb.linearVelocity.magnitude;
        _ySpeed = Vector3.Dot(_rb.linearVelocity, Vector3.up);
        _xSpeed = Vector3.Dot(_rb.linearVelocity, Vector3.right);
        _zSpeed = Vector3.Dot(_rb.linearVelocity, Vector3.forward);
        
        _isGrounded = IsGrounded();
        _wasOnRamp = _isOnRamp;
        _rampPlayerIsOn = IsOnRamp();
        _isOnRamp = _rampPlayerIsOn;
        
        HandleSpeedCapping();
        HandleVisualRotations();
        HandleMovement();
        HandleRampAudio();
        HandleMeltCleanAudio();
        
        if (!SoapDepleted){
            SetPlayerVisualPosition();
        }
        
        if (GameManager.CurrentGameState is GameState.LAUNCH) {
            _rb.linearVelocity = Vector3.zero; 
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void HandleMeltCleanAudio(){
        if (GameManager.CurrentGameState != GameState.GAMEPLAY) return;
        if (IsCleaning() && _currentSoapPower > 0)
        {
            if (!_cleanAudioSource.isPlaying)
                _cleanAudioSource.Play();

            _meltAudioSource.Stop();
        }
        if (_isGrounded && !_isOnRamp && _currentSoapPower > 0)
        {
            if (!_meltAudioSource.isPlaying)
                _meltAudioSource.Play();

            _cleanAudioSource.Stop();
        }
        else{
            _cleanAudioSource.Stop();
            _meltAudioSource.Stop();
        }
    }

    private void UpdateRunStats(){
        _runStats.SetDistanceTravelled((int) (transform.position.z - _startingPosition.z));
        _runStats.UpdateTotalMoneyEarned();
    }

    private void OnClean(PlayerInteractable interactable) 
    {
        GameManager.Instance.HitStop(_cleanHitStopTime);
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        _rb.AddForce(_cleanBoostDirection * _cleanBoostStrength, ForceMode.Impulse);
        
        AddSoapPower(_cleanSoapRefill);
        _cleanTween?.Kill();
        _playerVisualAnims.localScale = Vector3.one;
        _cleanTween = _playerVisualAnims.DOPunchScale(_playerVisualAnims.localScale * _cleanAnimationScale, _cleanAnimationDuration).SetEase(_cleanAnimationEase);
        
        _runStats.AddCleaned(interactable.CleanedType);
        
        
        AudioManager.Instance.PlayOneShot("Clean");
    }

    public void ResetPlayer(){
        _horizontalInput = 0;
        
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.transform.position = _originalPosition;
        
        _playerVisual.position = _rb.position;
        _playerVisual.rotation = Quaternion.identity;
        
        
        
        //populate ramp queue
        _rampQueue.Clear();
        for (int i = 0; i < _rampQueueSize; i++){
            EnqueueRandomRamp(false);
        }
        
        _runStats = new RunStats();
        
        UpdateUpgrades();
        RefillSoap();
    }
    
    private void TakeSoap(float amount){
        if (SoapDepleted){
            _soapTrail.Clear();
            _soapTrail.emitting = false;
            OnSoapDeplete?.Invoke(_runStats);

            _cleanAudioSource.Stop();
            _meltAudioSource.Stop();
            return;
        }
        _currentSoapPower-=amount;

        
        GameManager.UIManager.UpdateSoapMeter(_currentSoapPower/_maxSoapPower);
        UpdatePlayerSize();
    }
    
    private void UpdatePlayerSize(){
        float scale = _soapScaleCurve.Evaluate(_currentSoapPower/_maxSoapPower);
        scale += 0.01f;
      
        transform.localScale = Vector3.one * scale;
        _playerVisual.transform.localScale = Vector3.one * scale;
    
    }

  

    private void SetPlayerVisualPosition(){
        _playerVisual.transform.position = transform.position;
    }

    private void SetControlVariables(){
        _horizontalInput = Keyboard.current.aKey.isPressed ? -1f : Keyboard.current.dKey.isPressed ? 1f : 0f;
        _isSlamPressed = Keyboard.current.spaceKey.isPressed;
        
        _pressedSpawnRampThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
        _pressedDiscardRampThisFrame = Mouse.current.rightButton.wasPressedThisFrame;
    }

    private void HandleGroundSoapDeplete(bool isCleaning){
        if (!_isGrounded || _isOnRamp) return;
        
        //Take soap percentage
        if (isCleaning){
            TakeSoap(_maxSoapPower * _cleanSoapUsageSec * Time.deltaTime);
            _timeCleaning += Time.deltaTime;
            
            //Animate scale - charge clean anim
            float yScaleTarget = Mathf.Lerp(1, _cleaningMinYScale, _timeCleaning/_timeToClean);
            _playerVisualAnims.localScale = new Vector3(1, yScaleTarget, 1);

            //set the volume of the cleanAudioSource based on the scale of the player
            _cleanAudioSource.volume = 1 - (yScaleTarget - _cleaningMinYScale) / (1 - _cleaningMinYScale);
        }
        else{
            TakeSoap(_maxSoapPower * _groundSoapUsageSecPercent * Time.deltaTime);
            _timeCleaning = 0;
            _playerVisualAnims.localScale = Vector3.one;
        }
    }

    private void HandleMovement(){
        // Horizontal control -  force based on if ground or air with a clamped multiplier by forward speed
        float steerStrength = _steerStrength * ((_isGrounded && !_isOnRamp) ? _groundSteerMultiplier : _airSteerMultiplier);
        float forwardSpeedSteerMult = Mathf.Max(Mathf.Max(0, _zSpeed), _minForwardSpeedSteerMultiplier);
        _rb.AddForce(new Vector3(_horizontalInput, 0, 0) * (forwardSpeedSteerMult * steerStrength), ForceMode.Force);
        // Slam Control, only when in air
        if (_isSlamPressed && !_isGrounded){
            _rb.AddForce(new Vector3(0, -1, _slamForwardMult) * _slamStrength, ForceMode.Force);
        }
        
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
        float targetMax = (_isGrounded && !_isOnRamp) ? _maxGroundSpeed : _maxAirSpeed;
        
        
        if (_currentSpeed > targetMax) {
            
            Vector3 targetVelocity = _rb.linearVelocity.normalized * targetMax;
            _rb.linearVelocity = Vector3.MoveTowards(
                _rb.linearVelocity, 
                targetVelocity, 
                _maxSpeedBraking * Time.fixedDeltaTime
            );
        }
        
    }
//
    private void DiscardRamp(){
        _rampQueue.Dequeue();
        EnqueueRandomRamp(true);
    }
    
    private void HandleRampSpawningAndDiscarding(){
        if (_pressedDiscardRampThisFrame){
            DiscardRamp();
            return;
        }
        if (!_pressedSpawnRampThisFrame || _currentSoapPower <= _rampSoapCost || _currentSoapPower/_maxSoapPower <= _minSoapPercentForRamp ) return;

        bool canPlaceRamp = true;
        
        Ramp attachRamp = _rampPlayerIsOn ? GetLastAttachedRamp(_rampPlayerIsOn) : null;
        
        Vector3 velocityYNegative = new Vector3(_rb.linearVelocity.x, -Mathf.Abs(_rb.linearVelocity.y), _rb.linearVelocity.z);
        Vector3 predictedPlayerPosition = transform.position + (velocityYNegative * _predictionTime);
        
        Vector3 rampSpawnPoint = predictedPlayerPosition + _rampSpawnOffset;
        Vector3 toRampSpawnPoint = rampSpawnPoint - transform.position;
        
        // Check for ramp and ramp ground clipping
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
            
            if (_rampQueue.Peek().StartingDirection is RampDirection.Down or RampDirection.Straight){
                canPlaceRamp = false;
            }
        }
        
        // set ramp rotation to movement, don't if player isnt moving forwards
        Vector3 moveDir = new Vector3(_xSpeed,0 ,_zSpeed).normalized;
        Quaternion rampRotation = (attachRamp) 
            ? attachRamp.transform.rotation 
            : (_zSpeed < 0.1f) ? Quaternion.identity : Quaternion.LookRotation(moveDir, Vector3.up);

        // final check if ramp will spawn under or on smothing ground
        if (attachRamp){
            Vector3 attachPoint = attachRamp.transform.TransformPoint(attachRamp.EndPoint);
            Vector3 playerToAttachPoint = (attachPoint - transform.position);
            if (Physics.SphereCast(transform.position, _rampCheckRaycastRadius, playerToAttachPoint.normalized, out RaycastHit hitSpawn)){
                Debug.DrawLine(transform.position, hitSpawn.point , Color.magenta, 10f);
                if (!hitSpawn.transform.CompareTag("Ground"))
                    rampSpawnPoint = attachRamp.transform.TransformPoint(attachRamp.EndPoint);
                else{
                    canPlaceRamp = false;
                }
            }
        }

        if (!canPlaceRamp){
            DiscardRamp(); // discards automatically if you can't place
            GameManager.UIManager.CantPlaceRamp();
            return;
        }
        
        Ramp rampPrefab = _rampQueue.Dequeue();
        EnqueueRandomRamp(false);
        
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
        
        float distanceToRamp = (transform.position - spawnPosition).magnitude;
        float distanceT = Mathf.InverseLerp(_maxRampCreationDistance, 0, distanceToRamp);
        AudioManager.Instance.PlayOneShot("Ramp_Create", Mathf.Lerp(_minimumRampCreationVolume, 1, distanceT));
        TakeSoap(_rampSoapCost);
    }

    private void HandleCameraFOV(){
        float maxFOV = _originalCameraFov + _maxAddedFov;
        
        float targetFov = Mathf.Lerp(_originalCameraFov, maxFOV, _currentSpeed / _maxAirSpeed);
        float soapAmountMult = Mathf.Lerp(_minSoapFOVMultiplier, 1f,_currentSoapPower / _maxSoapPower);
        targetFov *= soapAmountMult;
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

    private void HandleRampAudio(){
        if (_isOnRamp)
            _onRampAudioTimer += Time.fixedDeltaTime;
        else if (!_isOnRamp && _onRampAudioTimer >= _rampTimeForExitPlay && _currentSpeed >= _maxAirSpeed * _rampExitSpeedThreshold){
            AudioManager.Instance.PlayOneShot("Swoosh_EndRamp");
            _onRampAudioTimer = 0;
        }      
        
        float targetVolume = _isOnRamp ? 1f : 0f;
    
        _rampStayAudioSource.volume = Mathf.MoveTowards(_rampStayAudioSource.volume, targetVolume, Time.deltaTime * _rampStayFadeSpeed);

        if (_isOnRamp && !_rampStayAudioSource.isPlaying)
            _rampStayAudioSource.Play();
        else if (!_isOnRamp && _rampStayAudioSource.volume <= 0.01f)
            _rampStayAudioSource.Stop();
    }

    private void UpdateUpgrades(){
        if (!_useUpgrades) return;
        
        PlayerUpgradeManager upgrades = GameManager.PlayerUpgradeManager;
        
        _groundSoapUsageSecPercent = upgrades.SoapUpgrade.CurrentLevelData.GroundSoapUsage;
        _badRampChance = upgrades.RampUpgrade.CurrentLevelData.BadRampChance;
        _maxAirSpeed = upgrades.SpeedUpgrade.CurrentLevelData.MaxAirSpeed;
        _maxGroundSpeed = upgrades.SpeedUpgrade.CurrentLevelData.MaxGroundSpeed;
        _steerStrength = upgrades.TurnStrength.CurrentLevelData.Value;
        _maxSoapPower = upgrades.SoapUpgrade.CurrentLevelData.MaxSoap;
        _soapRefillOnClean = upgrades.CleanUpgrade.CurrentLevelData.SoapRefillOnClean;
        _timeToClean = upgrades.CleanUpgrade.CurrentLevelData.TimeToClean;
        _cleanBoostStrength = upgrades.CleanUpgrade.CurrentLevelData.CleanBoostStrength;
        _rampForceDown = upgrades.RampUpgrade.CurrentLevelData.RampSpeedBoost.x;
        _rampForceForward = upgrades.RampUpgrade.CurrentLevelData.RampSpeedBoost.y;
        _slamStrength = upgrades.SlamForce.CurrentLevelData.Value;
    }

    private void RefillSoap() => AddSoapPower(1);

    private void AddSoapPower(float normalizedAmount){
        float amountToAdd = _maxSoapPower * normalizedAmount;
        _currentSoapPower = Mathf.Clamp(_currentSoapPower + amountToAdd,0, _maxSoapPower);
        
        UpdatePlayerSize(); //works right now but beware if added soap is too little
        GameManager.UIManager.UpdateSoapMeter(_currentSoapPower/_maxSoapPower);
    }

    private Ramp GetLastAttachedRamp(Ramp start){
        while (start.ConnectedRamp){
            start = start.ConnectedRamp;
        }
        return start;
    }

    private void EnqueueRandomRamp(bool discarding){
        List<Ramp> ramps;
        
        
        //chance to queue a bad ramp
        if (Random.Value <= _badRampChance){ 
            RampDirection[] values = (RampDirection[])System.Enum.GetValues(typeof(RampDirection));
            _lastRampEndDirection = RampDirection.Down;
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
        
        if (_isGrounded) ramps = _upFacingRamps;
        
        int totalCount = ramps.Count;
        int randomIndex = Random.Range(0, totalCount);
        Ramp ramp = ramps[randomIndex];
        
        _rampQueue.Enqueue(ramp);
        GameManager.UIManager.SetRampSprites(_rampQueue,discarding ? RampSelectionAnimation.DISCARD : RampSelectionAnimation.PLACE);
        _lastRampEndDirection = ramp.EndingDirection;
    }

    
    private void ProcessInteraction(GameObject obj){
        if (!obj.TryGetComponent(out PlayerInteractable interactable)) return;
        
        
        interactable.Interact(this, OnClean);
    }
    
    private void OnTriggerStay(Collider other){
        ProcessInteraction(other.gameObject);
    }

    private void OnCollisionStay(Collision collision){
        ProcessInteraction(collision.gameObject);
    }

    private float minImpactForce = 1f;               
    private void OnCollisionEnter(Collision collision)
    {
        if (_isOnRamp) return; // don't play if hitting ground while on ramp, only when landing on it or hitting other things

        if (!collision.gameObject.TryGetComponent(out PlayerInteractable interactable))
        {
            float impactForce = collision.relativeVelocity.magnitude;
            if (impactForce > minImpactForce)
            {
                AudioManager.Instance.PlayOneShot("Hit_Ground", Mathf.InverseLerp(minImpactForce, minImpactForce * 5, impactForce));
            }
        }
    }


    [ContextMenu("Force Update Upgrades")]
    private void ForceUpdateUpgrades(){
        if (!Application.isPlaying) return;
        UpdateUpgrades();
    }
    
    
    private void OnDrawGizmos(){
        if(!Application.isPlaying) SetPlayerVisualPosition();
    
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -Vector3.up*GroundedThresholdScaled);
        
        Gizmos.DrawSphere(transform.position + _rampSpawnOffset, 0.3f);
    }
}
