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

    public void SpawnRandom(StainSpawnPoint stainSpawnPoint, float decalSize)
    {
        int randomStainIndex = Random.Range(0, _stainList.Count);
        Stain randomStain = _stainList[randomStainIndex];
        
        DecalProjector stainDecal = randomStain.stainObject.GetComponent<DecalProjector>();
        BoxCollider stainCollider = randomStain.stainObject.GetComponent<BoxCollider>();
        int randomZRotation = Random.Range(0,360);
        
        stainDecal.size = new Vector3(decalSize, decalSize, 0.5f);
        stainCollider.size = stainDecal.size;
        // stainDecal.size.z / 2 => gets us on ground level (we offset to go below so we dont hit collider)
        stainCollider.center = new Vector3(0, 0, stainDecal.size.z / 2);

        randomStain.stainObject.transform.position = new Vector3(stainSpawnPoint.transform.position.x, 0.01f, stainSpawnPoint.transform.position.z);
        randomStain.stainObject.transform.rotation = Quaternion.Euler(90,0,randomZRotation);
        
        randomStain.stainObject.SetActive(true);
    }

    public void Reset()
    {
        foreach (Stain stain in _stainList)
        {
            stain.stainObject.SetActive(false);
        }
    }
}
