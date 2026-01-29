using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private Vector3 _moveVector;

    private void FixedUpdate()
    {
        transform.position += _moveVector * Time.deltaTime;
    }
}
