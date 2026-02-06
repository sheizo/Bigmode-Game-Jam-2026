using System;
using System.Collections.Generic;
using DG.Tweening;
using Freya;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public enum InteractionType
{
    INSTANT_CLEAN = 0,
    SMOOTH_CLEAN = 1
}

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] private InteractionType _interactionType;
    [SerializeField] private float _cleanFadeSpeed = 0.3f;
    [Range(0,1)] [SerializeField] private float _dirtyChance = 0.5f;
    [SerializeField] private bool _isNpc; 
    
    
    private Collider _collider;
    private List<Material> _materials = new List<Material>();
    private DecalProjector _decalProjector;
    private bool _cleanable = true;
    private float _cleanTimer; 
    
    public UnityEvent onPlayerInteract; // TODO :  MAYBE DELETE AND HANDLE SOUNDS IN PLAYER ONCLEAN
    public InteractionType InteractionType => _interactionType;
    
    void Awake()
    {
        gameObject.tag = GameManager.InteractableTag;

        if (TryGetComponent(out DecalProjector projector)){
            _decalProjector = projector;
        }else{
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers){
                _materials.Add(r.material);
            }

            _collider = GetComponent<Collider>();
        }
        
        Reset();
    }

    public void Reset(){
        _cleanable = true;
        _cleanTimer = 0;
        
        
        if (_decalProjector){
            _decalProjector.fadeFactor = 1;
        }
        else{
            SetMaterialsDirty(0);
            SetTriggerCollider(false);
            if (_dirtyChance > Random.value){
                SetMaterialsDirty(1);
                _cleanable = true;
                SetTriggerCollider(true);
            }
            else{
                _cleanable = false;
                SetTriggerCollider(false);
            }
        }
    }

    /// <summary>
    /// Returns true if cleanable
    /// </summary>
    /// <param name="player"></param>
    /// <param name="onClean"></param>
    /// <returns></returns>
    public bool Interact(PlayerController player, Action<PlayerInteractable> onClean){
        if (!_cleanable) return false;
        switch (_interactionType){
            case InteractionType.INSTANT_CLEAN:
            {
                if (_decalProjector){
                    DOTween.To(() => _decalProjector.fadeFactor, x => _decalProjector.fadeFactor = x, 0f, _cleanFadeSpeed).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                }
                else{
                    foreach (Material mat in _materials){
                        mat.DOFloat(0, GameManager.MasterMaterialDirtAmount, _cleanFadeSpeed);
                    }
                }
            
                Clean(onClean);
                break;
            }
            case InteractionType.SMOOTH_CLEAN:
            {
                _cleanTimer += Time.deltaTime;

                float remainingDirt = Mathf.InverseLerp(player.TimeToClean, 0, _cleanTimer);
                
                SetMaterialsDirty(remainingDirt);
                
                if (remainingDirt <= 0 ){
                    Clean(onClean);
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return _cleanable;
    }

    private void SetTriggerCollider(bool value){
        if (!_isNpc) return;
        _collider.isTrigger = value;
    }

    private void Clean(Action<PlayerInteractable> onClean){
        onPlayerInteract?.Invoke();
        _cleanable = false;
        onClean?.Invoke(this);
    }

    private void SetMaterialsDirty(float value){
        foreach (Material mat in _materials){
            mat.SetFloat(GameManager.MasterMaterialDirtAmount, value);
        }
    }
}
