using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldSegment : MonoBehaviour
{
    [SerializeField] private List<NpcSpawnPoint> _npcSpawnPoints;
    [SerializeField] private List<StainSpawnPoint> _stainSpawnPoints;
    [SerializeField] private float decalSize = 5;
    
    private float _stainSpawnRate = 0.6f;
    private float _npcSpawnRate = 0.75f;

    private float _colliderGroundOffset = 0.1f;
    

    public void Reset()
    {
        //disable all npcs
        foreach (NpcSpawnPoint npcSpawnPoint in _npcSpawnPoints)
        {
            npcSpawnPoint.Reset();
        }

        //disable all stains
        foreach (StainSpawnPoint stainSpawnPoint in _stainSpawnPoints)
        {
            stainSpawnPoint.Reset();
        }

        SpawnNpc();
        SpawnStain();
    }

    public void SpawnNpc()
    {
        float spawnRate = Random.value;
        if (spawnRate < _npcSpawnRate)
        {
            int randomSpawnPoint = Random.Range(0, _npcSpawnPoints.Count);
            _npcSpawnPoints[randomSpawnPoint].SpawnRandom();
        }

        foreach (NpcSpawnPoint npcSpawnPoint in _npcSpawnPoints)
        {
            if (spawnRate < _npcSpawnRate)
            {
                npcSpawnPoint.SpawnRandom();
            }
        }
        
    }

    public void SpawnStain()
    {
        float spawnRate = Random.value;
        if (spawnRate < _stainSpawnRate)
        {
            int randomStainSpawnPoint = Random.Range(0, _stainSpawnPoints.Count);
            StainSpawnPoint stainSpawnPoint = _stainSpawnPoints[randomStainSpawnPoint];
            stainSpawnPoint.SpawnRandom();

            Transform stainTransform = stainSpawnPoint.GetStainTransform();
            BoxCollider stainCollider = stainSpawnPoint.GetStainCollider();
            DecalProjector stainDecal = stainSpawnPoint.GetStainDecalProjector();

            int randomZRotation = Random.Range(0,360);
            
            stainDecal.size = new Vector3(decalSize, decalSize, 0.5f);
            stainCollider.size = stainDecal.size;
            // stainDecal.size.z / 2 => gets us on ground level (we offset to go below so we dont hit collider)
            stainCollider.center = new Vector3(0, 0, stainDecal.size.z / 2 + _colliderGroundOffset);

            stainTransform.transform.position = new Vector3(stainSpawnPoint.transform.position.x, 0.01f, stainSpawnPoint.transform.position.z);
            stainTransform.transform.rotation = Quaternion.Euler(90,0,randomZRotation);
            stainTransform.gameObject.SetActive(true);
        }
    }
}
