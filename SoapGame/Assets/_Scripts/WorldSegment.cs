using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldSegment : MonoBehaviour
{
    [SerializeField] private GameObject _npcSpawnPoints;
    [SerializeField] private GameObject _npcs;
    [SerializeField] private GameObject _stainSpawnPoints;
    [SerializeField] private GameObject _stains;
    [SerializeField] private float decalSize = 5;
    
    private float _stainSpawnRate = 0.6f;
    private float _npcSpawnRate = 0.75f;

    private float _uncommonNpcRate = 0.75f;
    private float _rareNpcRate = 0.25f;

    private float _colliderGroundOffset = 0.1f;
    

    public void Reset()
    {
        //disable all npcs
        foreach (Transform npcChild in _npcs.transform)
        {
            npcChild.gameObject.SetActive(false);
        }

        //disable all stains
        foreach (Transform stainChild in _stains.transform)
        {
            stainChild.gameObject.SetActive(false);
        }

        SpawnNpc();
        SpawnStains();
    }

    public void SpawnNpc()
    {
        float spawnRate = Random.value;
        if (spawnRate < _npcSpawnRate)
        {
            // get random spawn point
            int randomNpcSpawnPointIndex = Random.Range(0, _npcSpawnPoints.transform.childCount);
            Transform npcSpawnPoint = _npcSpawnPoints.transform.GetChild(randomNpcSpawnPointIndex);

            Transform npc = _npcs.transform.GetChild(0);
            float randomValue = Random.value;

            if (randomValue < _rareNpcRate)
            {
                npc = _npcs.transform.GetChild(2);
            }
            else if (randomValue < _uncommonNpcRate)
            {
                npc = _npcs.transform.GetChild(1);
            }
            
            npc.transform.position = npcSpawnPoint.transform.position;
            npc.gameObject.SetActive(true);
        }
    }

    public void SpawnStains()
    {
        float spawnRate = Random.value;
        if (spawnRate < _stainSpawnRate)
        {
            int randomStainSpawnPointIndex = Random.Range(0, _stainSpawnPoints.transform.childCount);
            Transform stainSpawnPoint = _stainSpawnPoints.transform.GetChild(randomStainSpawnPointIndex);

            int randomStain = Random.Range(0, _stains.transform.childCount);

            Transform stainTransform = _stains.transform.GetChild(randomStain);

            // get collider/decal to change sizes & offset collider pos so it doesnt get above ground
            BoxCollider stainCollider = stainTransform.GetComponent<BoxCollider>();
            DecalProjector stainDecal = stainTransform.GetComponent<DecalProjector>();

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
