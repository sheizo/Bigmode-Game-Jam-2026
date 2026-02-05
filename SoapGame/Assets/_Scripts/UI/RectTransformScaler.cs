using UnityEngine;
using DG.Tweening;

public class RectTransformScaler : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 normalScale = Vector3.one;
    
    [SerializeField] private Vector3 targetScale = Vector3.one * 1.1f;

    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration = 0.2f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private bool ignoreTimeScale = true;

    private Tween currentTween;


    public void ScaleToTarget()
    {
        ScaleTo(targetScale);
    }

    public void ScaleToNormal()
    {
        ScaleTo(normalScale);
    }

    private void ScaleTo(Vector3 scale)
    {
        if (target == null)
            return;

        currentTween?.Kill();

        currentTween = target
            .DOScale(scale, tweenDuration)
            .SetEase(ease)
            .SetUpdate(ignoreTimeScale);
    }
}
