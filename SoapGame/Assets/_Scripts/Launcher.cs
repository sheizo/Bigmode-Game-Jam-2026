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
    
    private float _chargeReleased;
    private float _chargeTimer;
    private float _evaluatedChargeTimer;
    private bool _isLaunchPressed, _wasLaunchReleased;
    private bool _reachedPeak;

    public Action OnLaunched;
        
        
    private void Awake(){
        _launchDirection = _launchDirection.normalized;
    }

    private void Start(){

        UpdateUpgrades();
    }

    private void Update(){
        if (GameManager.Instance.CurrentGameState != GameState.LAUNCH) return;
        
        _isLaunchPressed = Mouse.current.leftButton.isPressed;
        _wasLaunchReleased = Mouse.current.leftButton.wasReleasedThisFrame;

        if (_isLaunchPressed){
            if (_chargeTimer >= _timeToChargeLaunch) _reachedPeak = true;
            if (_chargeTimer <= 0) _reachedPeak = false;
            
            
            if (_reachedPeak) _chargeTimer -= Time.deltaTime;
            else _chargeTimer += Time.deltaTime;
            
            _evaluatedChargeTimer = _chargeBarCurve.Evaluate(_chargeTimer/_timeToChargeLaunch);
            
            UIManager.Instance.SetLaunchBar(_evaluatedChargeTimer);
        }
        if (_wasLaunchReleased){
            Launch();
        }
        
    }

    private void Launch(){
        float finalLaunchForce = Mathf.Lerp(_maxLaunchForce*_minLaunchForceMult, _maxLaunchForce, _evaluatedChargeTimer);
        _playerRb.AddForce(_launchDirection * finalLaunchForce, ForceMode.Impulse);
        
        _chargeTimer = 0;

        OnLaunched();
    }

    private void UpdateUpgrades(){
        if (!_useUpgrades) return;
        PlayerUpgradeManager upgrades = PlayerUpgradeManager.Instance;
        
        _maxLaunchForce = upgrades.LaunchForce.CurrentValue;
    }
    
}