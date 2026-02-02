using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimateText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    [SerializeField] private TextEffect _effect;
    [SerializeField] private bool _useRange;
    [SerializeField] private Vector2Int _range;
    [SerializeField] private bool _onAwake = true;

    private bool _enabled = false;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_onAwake || _enabled)
        {
            Animate();
        }
        
    }

    public void Animate()
    {
        if (_useRange)
            _effect.ApplyEffect(_text, _range.x, _range.y);
        else
            _effect.ApplyEffect(_text);
    }


    public void SetAnimation(bool enabled)
    {
        _enabled = enabled;
        //hacky way to disable text animation
        if (!enabled)
        {
            string originalText = _text.text;
            _text.text = _text.text + " ";
            _text.text = originalText;
        }
        
    }
}
