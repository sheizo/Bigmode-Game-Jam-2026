using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum RampSelectionAnimation
{
    NONE,
    PLACE,
    DISCARD
}
public class UIManager : MonoBehaviour
{
    private static readonly int Fill = Shader.PropertyToID("_Fill");
    [SerializeField] private Volume _globalVolume;
    
    [Header("Displays")]
    [SerializeField] StatsDisplay _statsDisplay;

    [Header("UI References")]
    [SerializeField] private RectTransform _selectedRamp, _waitingRamp;
    [SerializeField] private CanvasGroup _launchCanvasGroup, _gameplayCanvasGroup, _shopCanvasGroup, _lossCanvasGroup;
    [SerializeField] private Slider _launchBarSlider;
    [SerializeField] private Image _launchBarImage;
    [SerializeField] private Image _soapFill;
    [SerializeField] private Button _restartButton, _enterShopButton, _exitShopButton;
    [SerializeField] private TextMeshProUGUI _moneyCount;
    
    
    [Header("Effects")]
    [Range(0,1)][SerializeField] private float _soapFillDuration;
    [Range(0, 1)] [SerializeField] private float _vignetteSoapStart = 0.5f;
    [Range(0,1)][SerializeField] private float _vignetteSmoothTime = 0.5f;
    [SerializeField] private float _maxVignetteIntensity;
    
    [Space(15)]
    [SerializeField] private float _canvasGroupFadeTime = 0.5f;
    [SerializeField] private float _selectedAnimStrength,_selectedAnimDuration = 0.1f;
    [SerializeField] private float _discardAnimStrength, _discardAnimDuration;
    
    [SerializeField] private float _invalidSelectionAnimDuration = 0.4f, _invalidSelectionShakeAmount = 30;
    
    
    private float _selectedRampOrigScale, _waitingRampOrigScale;
    private Vector3 _waitingRampOrigPos;
    private Vector3 _selectedRampOrigPos;
    private Sequence _selectedRampPlaceSeq, _discardSeq, _invalidSeq;
    
    private Vignette _vignette;
    private float _startingVignetteIntensity;

    private Material _soapFillMat;

    public Action OnRestartClick;
    public Action OnShopClick;
    public Action OnExitShopClick;
    

    public void Init(){
        _selectedRampOrigPos = _selectedRamp.anchoredPosition;
        _selectedRampOrigScale = _selectedRamp.localScale.x;
        _waitingRampOrigPos = _waitingRamp.anchoredPosition;
        _waitingRampOrigScale = _waitingRamp.localScale.x;
        
        _globalVolume.profile.TryGet<Vignette>(out _vignette);
        _startingVignetteIntensity = _vignette.intensity.value;
        
        _soapFillMat = _soapFill.materialForRendering;
        CreateSequences();
    }

    private void Start(){
        _restartButton.onClick.AddListener(()=> OnRestartClick?.Invoke());
        _enterShopButton.onClick.AddListener(()=> OnShopClick?.Invoke());
        _exitShopButton.onClick.AddListener(()=> OnExitShopClick?.Invoke());
    }


    public void SetLossScreen(bool active){
        float canvasAlphaTarget = active ? 1f : 0f;
        _lossCanvasGroup.DOFade(canvasAlphaTarget, 0.1f).SetEase(Ease.InCubic);
    }
    
    private float _vignetteSpeed = 0;

    public void UpdateMoney(int money){
        _moneyCount.text = money.ToString();
    }

    public void UpdateSoapMeter(float tNormalized){
        
        float vignetteT = Mathf.InverseLerp(0,_vignetteSoapStart, tNormalized);
        
        _vignette.intensity.value = Mathf.SmoothDamp(
            _vignette.intensity.value,
            Mathf.Lerp(_maxVignetteIntensity, _startingVignetteIntensity, vignetteT),
            ref _vignetteSpeed,
            _vignetteSmoothTime
        );

        _soapFillMat.DOKill();
        _soapFillMat.DOFloat(tNormalized,"_Fill", _soapFillDuration).SetEase(Ease.OutExpo);    
        //_soapFill.SetMaterialDirty();
    }

    public void ResetSoapMeter(){
        _vignette.intensity.value = _startingVignetteIntensity;
        
        _soapFillMat.SetFloat("_Fill", 1);
    }
    
    // giga spaghet
    public void UpdateGameStateCanvas(GameState gameState) {
        switch (gameState){
            case GameState.GAMEPLAY:
                _lossCanvasGroup.FadeGroup(0, _canvasGroupFadeTime);
                _gameplayCanvasGroup?.FadeGroup(1, _canvasGroupFadeTime);
                _launchCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _shopCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                
                break;
            case GameState.SHOP:
                _lossCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _gameplayCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _shopCanvasGroup?.FadeGroup(1, _canvasGroupFadeTime);
                _launchCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                
                break;
            case GameState.LAUNCH:
                _lossCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _gameplayCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _shopCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _launchCanvasGroup?.FadeGroup(1, _canvasGroupFadeTime);
                
                break;
            case GameState.LOSSSCREEN:
                _lossCanvasGroup?.FadeGroup(1, _canvasGroupFadeTime);
                _gameplayCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _shopCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                _launchCanvasGroup?.FadeGroup(0, _canvasGroupFadeTime);
                
                break;
        }
    }
    
    
    public void SetLaunchBar(float tNormalized){
        _launchBarSlider.value = tNormalized;
    }

    public void SetRampSprites(Queue<Ramp> ramps, RampSelectionAnimation rampSelectionAnimation){
        int i = 0;
        foreach (Ramp ramp in ramps){
            RectTransform rampRect = i == 0 ? _selectedRamp : _waitingRamp; 
            if(rampRect.GetChild(0).TryGetComponent(out Image image))
                image.sprite = ramp.RampSprite;
            i++;
        }

        switch (rampSelectionAnimation){
            case RampSelectionAnimation.NONE:
                break;
            case RampSelectionAnimation.PLACE:
                _selectedRampPlaceSeq.Restart();
                break;
            case RampSelectionAnimation.DISCARD:
                _selectedRampPlaceSeq.Restart();
                _discardSeq.Restart();
                break;
        }
    }

    public void CantPlaceRamp(){
        _invalidSeq.Restart();
    }

    private void CreateSequences(){
        
        float scale = _selectedRampOrigScale * _selectedAnimStrength;
        _selectedRampPlaceSeq = DOTween.Sequence();
        _selectedRampPlaceSeq.Pause();
        _selectedRampPlaceSeq.SetAutoKill(false);
        _selectedRampPlaceSeq.Append(_selectedRamp.DOScale(Vector3.one * scale, _selectedAnimDuration));
        _selectedRampPlaceSeq.Append(_selectedRamp.DOScale(Vector3.one * _selectedRampOrigScale, _selectedAnimDuration).SetEase(Ease.OutCubic));

        scale = _waitingRampOrigScale * _discardAnimStrength; 
        _discardSeq = DOTween.Sequence();
        _discardSeq.Pause();
        _discardSeq.SetAutoKill(false);
        _discardSeq.Append(_waitingRamp.DOAnchorPos(_waitingRampOrigPos + Vector3.left * _discardAnimStrength, _discardAnimDuration));
        _discardSeq.Append(_waitingRamp.DOAnchorPos(_waitingRampOrigPos, _discardAnimDuration).SetEase(Ease.OutCubic));
        
        
        _invalidSeq = DOTween.Sequence();
        _invalidSeq.Pause();
        _invalidSeq.SetAutoKill(false);
        _invalidSeq.AppendCallback(()=>_selectedRamp.anchoredPosition = Vector3.zero);
        _invalidSeq.Append(_selectedRamp.DOShakeAnchorPos(_invalidSelectionAnimDuration, new Vector2(1, 0)*_invalidSelectionShakeAmount, 10, 0, false, true,
            ShakeRandomnessMode.Harmonic));
    }

    public void SetRunStats(RunStats runStats)
    {
        runStats.TotalMoneyEarned = 6767;
        _statsDisplay.ShowStats(runStats);
    }
    
    [ContextMenu("Player Restart")]
    private void Test1(){
        OnRestartClick?.Invoke();
        
    }
    [ContextMenu("Goto shop")]
    private void Test3(){
        OnShopClick?.Invoke();
    }
    [ContextMenu("Exit shop")]
    private void Test2(){
        OnExitShopClick?.Invoke();
    }

    private void OnDisable(){
        _vignette.intensity.value = _startingVignetteIntensity;
        
        _soapFillMat.SetFloat(Fill, 1);
    }
}
