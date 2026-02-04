using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuyableUpgrade : MonoBehaviour
{
    [SerializeField] private Transform _upgradeVisual;
    [SerializeField] private string _upgradeId;
    [SerializeField] private InputActionAsset _inputActionAsset;  // The object to be highlighted and clicked.
    [SerializeField] private Outline _outline;
    [SerializeField] private GameObject _objectToHighlight;  // The object to be highlighted and clicked.
    
    [SerializeField] private float _upgradeFadeDuration = 0.05f;
    [SerializeField] private TextMeshProUGUI _upgradeText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private CanvasGroup _canvasGroup, _costCanvasGroup;
    [SerializeField] private Transform _upgradeCountContainer;
    [SerializeField] private UpgradeCounter _upgradeCounterPrefab;

    [SerializeField]private float _failedPurchaseAnimDuration, _failedPurchaseShakeAmount;
    
    private InputAction _clickAction;
    private bool _isMouseHovered = false, _wasMouseHovered = false;
    private Camera _mainCamera;

    public Func<BuyableUpgrade, bool> OnClicked;
    private UpgradeBase _upgradeBase;
    private List<UpgradeCounter> _upgradeCounters = new List<UpgradeCounter>();

    private Tween _visualRotateTween;
    
    public UpgradeBase UpgradeBase => _upgradeBase;
    
    
    
    void Awake()
    {
        _clickAction = _inputActionAsset.FindActionMap("Player").FindAction("Click");
        _clickAction.Enable();

        _outline.enabled = false;
        
        _mainCamera = Camera.main;
        
        
        
    }

    private void Start(){
        PlayerUpgradeManager upgradeManager = GameManager.PlayerUpgradeManager;
        _upgradeBase = null;
        foreach (FieldInfo upgradeBaseField in upgradeManager.GetAllUpgradeFields()){
            UpgradeBase upgradeBase = upgradeBaseField.GetValue(upgradeManager) as UpgradeBase;
            if(upgradeBase?.Id == _upgradeId) _upgradeBase = upgradeBase;
        }
        
        CreateUI();
    }

    private void Update()
    {
        bool inShop = (GameManager.CurrentGameState == GameState.SHOP);

        if (!inShop)
        {
            _isMouseHovered = false;
            _wasMouseHovered = false;
            return;
        }
        
        // Perform raycast to detect mouse over the object.
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        _isMouseHovered = false;
        if (Physics.Raycast(ray, out hit)){
            if (hit.transform.gameObject == _objectToHighlight){
                _isMouseHovered = true;
                if (_clickAction.triggered)
                    OnObjectClicked();
                
                
            }
            else{
                
            }
        }

        _outline.enabled = _isMouseHovered;

        if (_isMouseHovered != _wasMouseHovered){
            _canvasGroup.FadeGroup(_isMouseHovered ? 1 : 0 , _upgradeFadeDuration, ease: Ease.InOutCubic);
            
            if(_isMouseHovered) _visualRotateTween = _upgradeVisual.DORotate(new Vector3(0,-360,0), 8, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            else _visualRotateTween?.Rewind();
            
            _wasMouseHovered = _isMouseHovered;
        }
    }

    private void CreateUI(){
        //for creating upgrade counts
        _upgradeCounters.Clear();
        print(_upgradeBase.CurrentLevel);
        for (int i = 1; i < _upgradeBase?.MaxLevel; i++){
            UpgradeCounter upgradeCounter = Instantiate(_upgradeCounterPrefab, _upgradeCountContainer);
            upgradeCounter.SetEnabled(_upgradeBase.CurrentLevel >= i);
            _upgradeCounters.Add(upgradeCounter);
        }
        
        UpdateUI();
    }

    public void UpdateUI(){
        _upgradeText.text = _upgradeBase?.Name;
        _costText.text = _upgradeBase?.NextLevelCost().ToString();

        int level = _upgradeBase.CurrentLevel-1;
        if (level < 0) return;
        UpgradeCounter currentLevelCounter = _upgradeCounters[level];
        if (currentLevelCounter){
            currentLevelCounter.SetEnabled(true);
            currentLevelCounter.transform.DOPunchScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBounce);
        }
        _costCanvasGroup.alpha = _upgradeBase.CanUpgrade ? 1 : 0;
    }

    private void OnFailedPurchase(){
        //TODO: ADD SOUND TO THIS
        transform.DOShakePosition(_failedPurchaseAnimDuration, new Vector2(1, 0)*_failedPurchaseShakeAmount, 15, 0, false, true,
            ShakeRandomnessMode.Harmonic);
    }
    
    
    // Custom method to handle the click event
    private void OnObjectClicked()
    {
        // Perform the desired action when the object is clicked
        Debug.Log("Object clicked: " + _objectToHighlight.name);
        bool purchaseSuccessful = OnClicked?.Invoke(this) ?? false;
        if(!purchaseSuccessful) OnFailedPurchase();
        
        if(purchaseSuccessful) Debug.Log("Success on: " + _objectToHighlight.name);
        
    }
    
    
}
