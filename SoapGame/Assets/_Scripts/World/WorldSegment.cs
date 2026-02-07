using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum SegmentType
{
    BATH_ROOM,
    LOCKER_ROOM,
    SHOWER_ROOM,
    BATH_HOUSE
}

public class WorldSegment : MonoBehaviour
{
    [SerializeField] private List<WorldSegmentType> _worldSegments;
    [SerializeField] private float decalSize = 5;
    
    private Dictionary<SegmentType, WorldSegmentType> _segmentsDict;
    
    private SegmentType _currSegmentType;

    private float _stainSpawnRate = 0.6f;
    
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
        WorldSegmentType randomSegmentType = _worldSegments[Random.Range(0, _worldSegments.Count)];
        _currSegmentType = randomSegmentType.SegmentType;

        print("Random segment created: " + _currSegmentType.ToString());

        //Disable all segment objects, enable the current one only
        foreach(WorldSegmentType seg in _worldSegments)
        {
            seg.GameObject.SetActive(seg.SegmentType == _currSegmentType);
        }

        //disable all npcs
        foreach (NpcSpawnPoint npcSpawnPoint in _segmentsDict[_currSegmentType].NpcSpawnPoints)
        {
            npcSpawnPoint.Reset();
        }

        //disable all stains
        foreach (StainSpawnPoint stainSpawnPoint in _segmentsDict[_currSegmentType].StainSpawnPoints)
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
            npcSpawnPoint.SpawnRandom();
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
