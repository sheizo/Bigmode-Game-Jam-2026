using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FollowObject : MonoBehaviour
{
    public Transform Target { get; set; }
    public Vector3 Offset { get; set; }

    [SerializeField] private bool _xPosition;
    [SerializeField] private bool _yPosition;
    [SerializeField] private bool _zPosition;

    private void LateUpdate(){
        if (!Target) return;
        
        transform.position = (new Vector3(_xPosition ? Target.position.x : transform.position.x,
                                         _yPosition ? Target.position.y : transform.position.y,
                                         _zPosition ? Target.position.z : transform.position.z)) + Offset;
    }
    
    
}
