using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private CanvasGroup _launchCanvasGroup, _gameplayCanvasGroup, _shopCanvasGroup;
    [SerializeField] private RectTransform _rampSelection;
    [SerializeField] private Slider _launchBarSlider;
    [SerializeField] private Image _launchBarImage;

    [SerializeField] private float _canvasGroupFadeTime = 0.5f;
    [SerializeField] private float _selectedRampScaleAniAmount;
    [SerializeField] private float _selectedRampScaleAniDuration = 0.1f;
    [SerializeField] private float _wrongSelectionShakeDuration = 0.4f, _wrongSelectionShakeAmount = 30;
    
    
    private RectTransform _selectedRamp;
    private float _selectedRampOrigScale;
    private Sequence _selectedRampScaleSeq, _selectedRampShakeSeq;

    protected override void Awake(){
        base.Awake();
        
        _selectedRamp = (RectTransform)_rampSelection.transform.GetChild(0);
        _selectedRampOrigScale = _selectedRamp.localScale.x;

        CreateSequences();
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

    public void SetRampSprites(Queue<Ramp> ramps, bool animate){
        int i = 0;
        foreach (Ramp ramp in ramps){
            if(_rampSelection.transform.GetChild(i).GetChild(0).TryGetComponent(out Image image))
                image.sprite = ramp.RampSprite;
            i++;
        }
        if(animate) _selectedRampScaleSeq.Restart();
    }

    public void CantPlaceRamp(){
        _selectedRampShakeSeq.Restart();
    }
    
    private void CreateSequences(){
        
        float scale = _selectedRampOrigScale + _selectedRampScaleAniAmount;
        _selectedRampScaleSeq = DOTween.Sequence();
        _selectedRampScaleSeq.Pause();
        _selectedRampScaleSeq.SetAutoKill(false);
        _selectedRampScaleSeq.AppendCallback(()=>_selectedRamp.localScale = Vector3.one * _selectedRampOrigScale);
        _selectedRampScaleSeq.Append(_selectedRamp.DOScale(Vector3.one * scale, _selectedRampScaleAniDuration));
        _selectedRampScaleSeq.Append(_selectedRamp.DOScale(Vector3.one * _selectedRampOrigScale, _selectedRampScaleAniDuration).SetEase(Ease.OutCubic));
        
        _selectedRampShakeSeq = DOTween.Sequence();
        _selectedRampShakeSeq.Pause();
        _selectedRampShakeSeq.SetAutoKill(false);
        _selectedRampShakeSeq.AppendCallback(()=>_selectedRamp.anchoredPosition = Vector3.zero);
        _selectedRampShakeSeq.Append(_selectedRamp.DOShakeAnchorPos(_wrongSelectionShakeDuration, new Vector2(1, 0)*_wrongSelectionShakeAmount, 10, 0, false, true,
            ShakeRandomnessMode.Harmonic));
    }
}
