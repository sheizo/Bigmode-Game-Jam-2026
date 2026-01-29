using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveOnAwake : MonoBehaviour
{
    private void Awake()
    {
        if(gameObject) gameObject.SetActive(false);
    }
}
