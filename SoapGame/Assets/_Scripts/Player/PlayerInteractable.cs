using UnityEngine;
using UnityEngine.Events;

public enum InteractionType
{
    NPC = 0,
    STAIN,
    SLOW,
    SPEED
}

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] InteractionType _interactionType;

    public UnityEvent onPlayerInteract;
    public bool _interacted;
    public InteractionType InteractionType => _interactionType;

    void Awake()
    {
        _interacted = false;
        gameObject.tag = GameManager.InteractableTag;
    }

    public void Interact(PlayerController player)
    {
        if (!_interacted)
        {
            _interacted = true;
            onPlayerInteract?.Invoke();
        }
    }
}
