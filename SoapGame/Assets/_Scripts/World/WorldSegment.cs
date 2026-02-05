using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum SegmentType
{
    BATHROOM,
    LOCKER_ROOM,
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
    private float _npcSpawnRate = 0.5f;
    
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
        SegmentTypeObject randomSegmentType = _worldSegments[Random.Range(0, _worldSegments.Count)];
        _currSegmentType = randomSegmentType.SegmentType;

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
        List<NpcSpawnPoint> npcSpawnPointList = _segmentsDict[_currSegmentType].NpcSpawnPoints;

        foreach (NpcSpawnPoint npcSpawnPoint in npcSpawnPointList)
        {
            float spawnRate = Random.value;
            if (spawnRate < _npcSpawnRate)
            {
                npcSpawnPoint.SpawnRandom();
            }
        }
    }

    public void SpawnStain()
    {
        List<StainSpawnPoint> stainSpawnList = _segmentsDict[_currSegmentType].StainSpawnPoints;

        foreach (StainSpawnPoint stainSpawnPoint in stainSpawnList)
        {
            float spawnRate = Random.value;
            if (spawnRate < _stainSpawnRate)
            {
                stainSpawnPoint.SpawnRandom(stainSpawnPoint, decalSize);
            }
        }
        
    }
}
