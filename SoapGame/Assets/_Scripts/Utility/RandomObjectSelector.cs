using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSelector : MonoBehaviour
{
    private List<Transform> _objectList;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            _objectList.Add(child);
        }
    }

    private void GetRandomObject()
    {
        Transform randomObject = _objectList[Random.Range(0, _objectList.Count)];
        randomObject.gameObject.SetActive(true);
    }
}
