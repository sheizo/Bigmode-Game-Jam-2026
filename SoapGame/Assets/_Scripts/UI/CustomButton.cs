using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{

    [SerializeField] private Vector3 hoverScale = Vector3.one * 1.1f;
    [SerializeField] private Vector3 clickScale = Vector3.one * 0.9f;

    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration = 0.2f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private bool ignoreTimeScale = true;


    private Vector3 originalScale;
    private Tween currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        ScaleTo(hoverScale);
        AudioManager.Instance.PlayOneShot("Button_Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleTo(originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ScaleTo(clickScale);
        AudioManager.Instance.PlayOneShot("Button_Click");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ScaleTo(originalScale);
    }

    private void ScaleTo(Vector3 scale)
    {
        currentTween?.Kill();

        currentTween = transform
            .DOScale(scale, tweenDuration)
            .SetEase(ease)
            .SetUpdate(ignoreTimeScale);
    }
}
