using System.Collections.Generic;
using UnityEngine;

public enum NpcRarity
{
    COMMON,
    UNCOMMON,
    RARE
}

[System.Serializable]
public class Npc
{
    public CleanedType CleanedType;
    public NpcRarity rarity;
    public GameObject npcObject;
    public bool npcRotates;
}

public class NpcSpawnPoint : MonoBehaviour
{
    
    [SerializeField] private List<Npc> _npcList;
    [SerializeField][Range(0,1)] private float _npcSpawnRate = 0.5f;
    private List<Npc> _weightedNpcList;

    void Awake()
    {
        _weightedNpcList = new();

        foreach (Npc npc in _npcList)
        {
            int weight = GetRarityWeight(npc.rarity);
            for (int i = 0; i < weight; i++)
            {
                _weightedNpcList.Add(npc);
            }
        }

        Reset();
    }

    public void SpawnRandom(){
        if (_weightedNpcList == null) return;

        float spawnRate = Random.value;
        if (spawnRate > _npcSpawnRate)
        {
            return;
        }
        
        int randomNpcIndex = Random.Range(0, _weightedNpcList.Count);
        Npc randomNpc = _weightedNpcList[randomNpcIndex];
        randomNpc.npcObject.SetActive(true);
        
        if (randomNpc.npcRotates)
        {
            int randomZRotation = Random.Range(0,360);
            randomNpc.npcObject.transform.rotation = Quaternion.Euler(0, randomZRotation, 0);
        }
    }

    public void Reset()
    {
        foreach (Npc npc in _npcList)
        {
            npc.npcObject.SetActive(false);
            if (npc.npcObject.TryGetComponent(out PlayerInteractable playerInteractable)) playerInteractable.Reset();
        }
    }

    private int GetRarityWeight(NpcRarity rarity)
    {
        switch (rarity)
        {
            case NpcRarity.COMMON:
                return 50;
            case NpcRarity.UNCOMMON:
                return 10; 
            case NpcRarity.RARE:
                return 1; 
            default:
                return 0;
        }
    }
}
