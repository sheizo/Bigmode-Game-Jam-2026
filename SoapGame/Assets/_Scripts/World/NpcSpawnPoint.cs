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
    public NpcRarity rarity;
    public GameObject npcObject;
    public bool npcRotates;
}

public class NpcSpawnPoint : MonoBehaviour
{
    
    [SerializeField] private List<Npc> _npcList;
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
    }

    public void SpawnRandom(){
        if (_weightedNpcList == null) return;
        
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
