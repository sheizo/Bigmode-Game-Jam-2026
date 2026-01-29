using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimationRandom : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private string _animationString;
    [SerializeField] private bool _invokeDelay;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (!_invokeDelay) _animator.Play(_animationString, 0, Random.Range(0f, 1f));
        else Invoke(nameof(PlayAnimation), Random.Range(0f, 2f));
    }

    private void PlayAnimation()
    {
        _animator.Play(_animationString);
    }

}
