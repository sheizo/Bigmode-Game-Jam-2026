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
    private List<UpgradeCounter> _upgradeCounters = new List<UpgradeCounter>();

    private Tween _visualRotateTween;
    
    private IUpgrade _upgrade;
    public IUpgrade Upgrade => _upgrade;
    
    void Awake()
    {
        _clickAction = _inputActionAsset.FindActionMap("Player").FindAction("Click");
        _clickAction.Enable();

        _outline.enabled = false;
        
        _canvasGroup.alpha = 0;
        _costCanvasGroup.alpha = 0;
    }

    void OnEnable()
    {
        _mainCamera = Camera.main;
    }

    public void Init()
    {
        PlayerUpgradeManager upgradeManager = GameManager.PlayerUpgradeManager;
        foreach (var upgrade in upgradeManager.AllUpgrades)
        {
            if (upgrade.Id == _upgradeId)
            {
                _upgrade = upgrade;
                break;
            }
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
            float targetAlpha = _isMouseHovered ? 1 : 0;
            _canvasGroup.FadeGroup(targetAlpha, _upgradeFadeDuration, ease: Ease.InOutCubic);
            
            if (_upgrade != null && _upgrade.CanUpgrade) {
                _costCanvasGroup.FadeGroup(targetAlpha, _upgradeFadeDuration, ease: Ease.InOutCubic);
            } else {
                _costCanvasGroup.alpha = 0; // Hide if maxed out
            }

            if (_isMouseHovered) 
                _visualRotateTween = _upgradeVisual.DORotate(new Vector3(0,-360,0), 8, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            else 
                _visualRotateTween?.Rewind();
    
            _wasMouseHovered = _isMouseHovered;
        }
    }

    private void CreateUI(){
        //for creating upgrade counts
        _upgradeCounters.Clear();
        for (int i = 1; i < _upgrade?.MaxLevel; i++){
            UpgradeCounter upgradeCounter = Instantiate(_upgradeCounterPrefab, _upgradeCountContainer);
            upgradeCounter.SetEnabled(_upgrade.CurrentLevel >= i);
            _upgradeCounters.Add(upgradeCounter);
        }
        
        UpdateUI();
    }

    public void RefreshUI()
    {
        int level = _upgrade.CurrentLevel-1;
        for (int i = 0; i < _upgradeCounters.Count; i++){
            if (i == level){
                _upgradeCounters[i].SetEnabled(true);
                _upgradeCounters[i].transform.DOPunchScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBounce);
            }
            else{
                _upgradeCounters[i].SetEnabled(i < level);
            }
        }
        
        _costCanvasGroup.alpha = 1;
        _costText.text = _upgrade?.NextLevelCost().ToString();
    }

    public void UpdateUI(){
        _upgradeText.text = _upgrade?.Name;
        _costText.text = _upgrade?.NextLevelCost().ToString();

        int level = _upgrade.CurrentLevel-1;
        if (level < 0) return;
        UpgradeCounter currentLevelCounter = _upgradeCounters[level];
        if (currentLevelCounter){
            currentLevelCounter.SetEnabled(true);
            currentLevelCounter.transform.DOPunchScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.OutBounce);
        }
        _costCanvasGroup.alpha = _upgrade.CanUpgrade ? 1 : 0;
    }

    private void OnFailedPurchase(){
        //TODO: ADD SOUND TO THIS
        AudioManager.Instance.PlayOneShot("Error_Purchase");
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
