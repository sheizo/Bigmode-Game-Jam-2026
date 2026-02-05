using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public enum InteractionType
{
    INSTANT_CLEAN = 0,
    SMOOTH_CLEAN = 1
}

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] InteractionType _interactionType;
    [SerializeField] private bool _isDecal;
    
    private Renderer _renderer;
    private DecalProjector _decalProjector;
    
    public UnityEvent onPlayerInteract;
    public bool _interacted;
    public InteractionType InteractionType => _interactionType;

    void Awake()
    {
        _interacted = false;
        gameObject.tag = GameManager.InteractableTag;
        _renderer = GetComponent<Renderer>();
        
        if(_isDecal) _decalProjector = GetComponent<DecalProjector>();
    }

    public void Interact(PlayerController player){
        if (_interacted) return;
        
        _interacted = true;
        onPlayerInteract?.Invoke();

        if (_isDecal){
            DOTween.To(() => _decalProjector.fadeFactor, x => _decalProjector.fadeFactor = x, 0f, 0.5f).OnComplete(() =>
            {
               gameObject.SetActive(false);
            });
        }
        
    }
}
