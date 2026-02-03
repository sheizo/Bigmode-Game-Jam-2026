using System;
using UnityEngine;

public enum InteractionType
{
    NPC = 0,
    SLOW,
    SPEED
}

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] InteractionType _interactionType;

    public InteractionType InteractionType => _interactionType;

    void Awake()
    {
        gameObject.tag = GameManager.InteractableTag;
    }
}
