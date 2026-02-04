using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]

public class Stain
{
    public GameObject stainObject;
}

public class StainSpawnPoint : MonoBehaviour
{
    [SerializeField] private List<Stain> _stainList;

    private BoxCollider stainCollider;
    private DecalProjector stainDecal;
    private Transform stainTransform;

    public void SpawnRandom()
    {
        int randomStain = Random.Range(0, _stainList.Count);
        _stainList[randomStain].stainObject.SetActive(true);
        
        stainTransform = _stainList[randomStain].stainObject.transform;
        
        stainCollider = stainTransform.GetComponent<BoxCollider>();
        stainDecal = stainTransform.GetComponent<DecalProjector>();
    }
    
    public BoxCollider GetStainCollider()
    {
        return stainCollider;
    }

    public DecalProjector GetStainDecalProjector()
    {
        return stainDecal;
    }

    public Transform GetStainTransform()
    {
        return stainTransform;
    }

    public void Reset()
    {
        foreach (Stain stain in _stainList)
        {
            stain.stainObject.SetActive(false);
        }
    }
}
