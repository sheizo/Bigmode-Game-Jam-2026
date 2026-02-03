
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform _rectTransform;
    private Tween _currentTween;
    
    [SerializeField] private float _scaleHoverDuration = 1f, _scaleClickDuration, _scaleSize = 10;
    [SerializeField] private Ease _hoverEase = Ease.Linear;
    [SerializeField] private Ease _clickEase = Ease.Linear;
    
    private void OnEnable(){
        _rectTransform = GetComponent<RectTransform>();
        _currentTween.SetUpdate(true);
    }

    public void OnPointerEnter(PointerEventData eventData){
        _currentTween.Rewind();
        _currentTween = _rectTransform.DOScale(_rectTransform.localScale + Vector3.one* _scaleSize, _scaleHoverDuration).SetLoops(-1, LoopType.Yoyo).SetEase(_hoverEase);
    }

    public void OnPointerClick(PointerEventData eventData){
        _currentTween.Rewind();
        _currentTween?.Kill();
        _currentTween = _rectTransform.DOScale(_rectTransform.localScale - Vector3.one* _scaleSize*2, _scaleClickDuration).SetEase(_clickEase);
    }

    public void OnPointerExit(PointerEventData eventData){
        _currentTween.SmoothRewind();
        //_rectTransform.DOShakeAnchorPos(0.1f, 200, 10, 90, false, true);
    }
}
