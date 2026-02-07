using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StainSpawnPoint : MonoBehaviour
{
    [SerializeField] private List<Stain> _stainList;

    public void SpawnRandom(StainSpawnPoint stainSpawnPoint, float decalSize)
    {
        int randomStainIndex = Random.Range(0, _stainList.Count);
        Stain randomStain = _stainList[randomStainIndex];

        int randomZRotation = Random.Range(0,360);
        
        randomStain.stainDecal.size = new Vector3(decalSize, decalSize, 0.5f);
        randomStain.stainCollider.size = randomStain.stainDecal.size;
        // stainDecal.size.z / 2 => gets us on ground level (we offset to go below so we dont hit collider)
        randomStain.stainCollider.center = new Vector3(0, 0, randomStain.stainDecal.size.z / 2);

        randomStain.transform.position = new Vector3(stainSpawnPoint.transform.position.x, 0.01f, stainSpawnPoint.transform.position.z);
        randomStain.transform.rotation = Quaternion.Euler(90,0,randomZRotation);
        
        randomStain.gameObject.SetActive(true);
    }

    public void Reset()
    {
        foreach (Stain stain in _stainList)
        {
            stain.gameObject.SetActive(false);
        }
    }
}
