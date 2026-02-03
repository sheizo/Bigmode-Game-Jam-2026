using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    [SerializeField] private bool _useUpgrades;
    
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private Vector3 _launchDirection = new Vector3(0,0.8f,1);
    [SerializeField] private float _maxLaunchForce = 3, _minLaunchForceMult = 0.3f;
    [SerializeField] private float _timeToChargeLaunch=2;
    [SerializeField] private AnimationCurve _chargeBarCurve;
    
    private float _chargeTimer;
    private float _evaluatedChargeTimer;
    private bool _isLaunchPressed, _wasLaunchReleased;
    private bool _reachedPeak;

    public Action<float> OnLaunched;
        
        
    private void Awake(){
        _launchDirection = _launchDirection.normalized;
    }

    private void Start(){

        ResetLauncher();
    }

    private void Update(){
        if (GameManager.CurrentGameState != GameState.LAUNCH) return;
        
        _isLaunchPressed = Mouse.current.leftButton.isPressed;
        _wasLaunchReleased = Mouse.current.leftButton.wasReleasedThisFrame;

        if (_isLaunchPressed){
            if (_chargeTimer >= _timeToChargeLaunch) _reachedPeak = true;
            if (_chargeTimer <= 0) _reachedPeak = false;
            
            
            if (_reachedPeak) _chargeTimer -= Time.deltaTime;
            else _chargeTimer += Time.deltaTime;
            
            _evaluatedChargeTimer = _chargeBarCurve.Evaluate(_chargeTimer/_timeToChargeLaunch);
            
            GameManager.UIManager.SetLaunchBar(_evaluatedChargeTimer);
        }
        if (_wasLaunchReleased && _chargeTimer > 0){
            Launch();
        }
        
    }

    private void Launch(){
        float launchForce = Mathf.Lerp(_maxLaunchForce*_minLaunchForceMult, _maxLaunchForce, _evaluatedChargeTimer);
        _playerRb.AddForce(_launchDirection * launchForce, ForceMode.Impulse);
        
        _chargeTimer = 0;

        OnLaunched?.Invoke(launchForce);
    }

    public void ResetLauncher(){
        
        _reachedPeak = false;
        _chargeTimer = 0;
        UpdateUpgrades();
        GameManager.UIManager.SetLaunchBar(0);
    }

    private void UpdateUpgrades(){
        if (!_useUpgrades) return;
        _maxLaunchForce = GameManager.PlayerUpgradeManager.LaunchForce.CurrentValue;
    }
    
}