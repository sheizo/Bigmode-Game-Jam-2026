using System;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    [SerializeField] private float _timeToDespawn;

    private float _timer;

    public float TimeToDespawn => _timeToDespawn;

    private void Update(){
        _timer += Time.deltaTime;
        if (_timer >= TimeToDespawn){
            Destroy(this.gameObject);
        }
    }
}
