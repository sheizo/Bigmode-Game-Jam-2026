using System;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    [SerializeField] private float _timeToDespawn;

    private float _timer;
    private void Update(){
        _timer += Time.deltaTime;
        if (_timer >= _timeToDespawn){
            Destroy(this.gameObject);
        }
    }
}
