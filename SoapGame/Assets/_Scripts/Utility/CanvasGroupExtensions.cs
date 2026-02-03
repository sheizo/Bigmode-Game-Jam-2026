using DG.Tweening;
using UnityEngine;

public static class CanvasGroupExtensions
{
    public static void FadeGroup(this CanvasGroup cg, float targetAlpha, float duration, bool ignoreTimeScale = false)
    {
        if (cg == null) return;

        cg.DOKill(); 
        cg.DOFade(targetAlpha, duration).SetUpdate(ignoreTimeScale);
        
        bool isActive = targetAlpha > 0;
        cg.interactable = isActive;
        cg.blocksRaycasts = isActive;
    }
}