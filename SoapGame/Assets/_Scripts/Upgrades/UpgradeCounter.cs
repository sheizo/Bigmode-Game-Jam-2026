using System;
using UnityEngine;

public class UpgradeCounter : MonoBehaviour
{
    [SerializeField] private GameObject _enabledObject, _disabledObject;

    private bool _enabled;
    private RectTransform _rectTransform;
    
    public RectTransform RectTransform => _rectTransform;
    
    private void Awake(){
        _rectTransform = this._rectTransform;
    }

    public void SetEnabled(bool value){
        _enabled = value;
        
        _enabledObject.SetActive(_enabled);
        _disabledObject.SetActive(!_enabled);
    }
}
