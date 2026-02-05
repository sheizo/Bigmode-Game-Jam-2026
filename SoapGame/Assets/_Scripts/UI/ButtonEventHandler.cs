using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonEventHandler : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Hover Events")]
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;

    [Header("Pointer Events")]
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
    }
}
