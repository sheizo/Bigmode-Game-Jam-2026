using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAxis : MonoBehaviour
{
    private Vector3 _startingPosition;
    private Vector3 _startingScale;
    private Vector3 _startingRotation;

    [SerializeField] private bool fixedUpdate;

    [Header("Position")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY, lockZ;

    [Header("Scale")]
    [SerializeField] private bool lockXScale;
    [SerializeField] private bool lockYScale, lockZScale;

    [Header("Rotation")]
    [SerializeField] private bool lockXRotation;
    [SerializeField] private bool lockYRotation, lockZRotation;

    private void Awake()
    {
        _startingPosition = transform.position;
        _startingScale = transform.localScale;
    }

    private void Update()
    {
        if (fixedUpdate) return;

        transform.position = new Vector3(
            lockX ? _startingPosition.x : transform.position.x,
            lockY ? _startingPosition.y : transform.position.y,
            lockZ ? _startingPosition.z : transform.position.z

        );

        transform.localScale = new Vector3(
            lockXScale ? _startingScale.x : transform.localScale.x,
            lockYScale ? _startingScale.y : transform.localScale.y,
            lockZScale ? _startingScale.z : transform.localScale.z

        );

        transform.eulerAngles = new Vector3(
           lockXRotation ? _startingRotation.x : transform.localRotation.x,
           lockYRotation ? _startingRotation.y : transform.localRotation.y,
           lockZRotation ? _startingRotation.z : transform.localRotation.z

       );

    }

    private void FixedUpdate()
    {
        if (!fixedUpdate) return;

        transform.position = new Vector3(
            lockX ? _startingPosition.x : transform.position.x,
            lockY ? _startingPosition.y : transform.position.y,
            lockZ ? _startingPosition.z : transform.position.z

        );

        transform.localScale = new Vector3(
            lockXScale ? _startingScale.x : transform.localScale.x,
            lockYScale ? _startingScale.y : transform.localScale.y,
            lockZScale ? _startingScale.z : transform.localScale.z

        );

        transform.eulerAngles = new Vector3(
           lockXRotation ? _startingRotation.x : transform.localRotation.x,
           lockYRotation ? _startingRotation.y : transform.localRotation.y,
           lockZRotation ? _startingRotation.z : transform.localRotation.z

       );

    }
}
