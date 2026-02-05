using System;
using UnityEngine;
using UnityEngine.Events;

public enum InteractionType
{
    NPC = 0,
    SLOW,
    SPEED
}

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] InteractionType _interactionType;

    public UnityEvent onPlayerInteract;

    public InteractionType InteractionType => _interactionType;

    void Awake()
    {
        gameObject.tag = GameManager.InteractableTag;
    }

    public void Interact(PlayerController player)
    {
        onPlayerInteract?.Invoke();
    }
}
