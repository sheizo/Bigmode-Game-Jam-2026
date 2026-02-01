using System;
using System.Collections.Generic;
using DG.Tweening;
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
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Volume _globalVolume;
    
    [Header("UI References")]
    [SerializeField] private RectTransform _selectedRamp, _waitingRamp;
    [SerializeField] private CanvasGroup _launchCanvasGroup, _gameplayCanvasGroup, _shopCanvasGroup;
    [SerializeField] private Slider _launchBarSlider;
    [SerializeField] private Image _launchBarImage;

    [Header("Effects")]
    [Range(0,1)] [SerializeField] private float _vignetteSmoothTime = 0.5f;
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


    protected override void Awake(){
        base.Awake();
        
        _selectedRampOrigPos = _selectedRamp.anchoredPosition;
        _selectedRampOrigScale = _selectedRamp.localScale.x;
        _waitingRampOrigPos = _waitingRamp.anchoredPosition;
        _waitingRampOrigScale = _waitingRamp.localScale.x;
        
        
        CreateSequences();

        _globalVolume.profile.TryGet<Vignette>(out _vignette);
        _startingVignetteIntensity = _vignette.intensity.value;
    }


    private float _vignetteSpeed = 0;
    public void UpdateSoapMeter(float tNormalized){
        _vignette.intensity.value = Mathf.SmoothDamp(
            _vignette.intensity.value,
            Mathf.Lerp(_maxVignetteIntensity, _startingVignetteIntensity, tNormalized),
            ref _vignetteSpeed,
            _vignetteSmoothTime
        );
        //Mathf.SmoothDamp()
    }
    
    public void UpdateGameStateCanvas(GameState gameState){
        if (gameState == GameState.GAMEPLAY){
            _gameplayCanvasGroup?.DOFade(1, _canvasGroupFadeTime);
            _launchCanvasGroup?.DOFade(0, _canvasGroupFadeTime);
            _shopCanvasGroup?.DOFade(0, _canvasGroupFadeTime);
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

    private void OnDisable(){
        _vignette.intensity.value = _startingVignetteIntensity;
    }
}
