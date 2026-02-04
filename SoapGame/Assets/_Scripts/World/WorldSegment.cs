using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum SegmentType
{
    BATHROOM,
    SCHOOL,
    GARDEN
}

[System.Serializable]
public class SegmentTypeObject
{
    public SegmentType SegmentType;
    public GameObject GameObject;
    public List<NpcSpawnPoint> NpcSpawnPoints;
    public List<StainSpawnPoint> StainSpawnPoints;
}


public class WorldSegment : MonoBehaviour
{
    [SerializeField] private List<SegmentTypeObject> _worldSegments;
    [SerializeField] private float decalSize = 5;
    
    private Dictionary<SegmentType, SegmentTypeObject> _segmentsDict;
    
    private SegmentType _currSegmentType;

    private float _stainSpawnRate = 0.6f;
    private float _npcSpawnRate = 0.75f;

    private float _colliderGroundOffset = 0.1f;
    
    void CreateDictIfNotExist()
    {
        if (_segmentsDict == null)
        {
            _segmentsDict = new();
            foreach(var a in _worldSegments)
            {
                _segmentsDict[a.SegmentType] = a;
            }
        }
    }

    public void Reset()
    {
        CreateDictIfNotExist();

        //Get a random room type.
        var values = (SegmentType[])System.Enum.GetValues(typeof(SegmentType));
        _currSegmentType = values[Random.Range(0, values.Length)];

        print("Random segment created: " + _currSegmentType.ToString());

        //Disable all segment objects, enable the current one only
        foreach(SegmentTypeObject seg in _worldSegments)
        {
            seg.GameObject.SetActive(seg.SegmentType == _currSegmentType);
        }

        //disable all npcs
        List<NpcSpawnPoint> npcSpawnPoints = _segmentsDict[_currSegmentType].NpcSpawnPoints;
        foreach (NpcSpawnPoint npcSpawnPoint in npcSpawnPoints)
        {
            npcSpawnPoint.Reset();
        }

        //disable all stains
        List<StainSpawnPoint> stainSpawnPoints = _segmentsDict[_currSegmentType].StainSpawnPoints;
        foreach (StainSpawnPoint stainSpawnPoint in stainSpawnPoints)
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
            List<NpcSpawnPoint> npcList = _segmentsDict[_currSegmentType].NpcSpawnPoints;

            int randomSpawnPoint = Random.Range(0, npcList.Count);
            npcList[randomSpawnPoint].SpawnRandom();
        }
    }

    public void SpawnStain()
    {
        float spawnRate = Random.value;
        if (spawnRate < _stainSpawnRate)
        {
            List<StainSpawnPoint> stainSpawnList = _segmentsDict[_currSegmentType].StainSpawnPoints;
            int randomStainSpawnPoint = Random.Range(0, stainSpawnList.Count);
            StainSpawnPoint stainSpawnPoint = stainSpawnList[randomStainSpawnPoint];
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
