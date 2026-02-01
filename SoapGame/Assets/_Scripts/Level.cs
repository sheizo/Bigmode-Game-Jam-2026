using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoints;
    [SerializeField] private GameObject npcs;
    
    private float npcSpawnRate = 0.75f;

    private float uncommonNpcRate = 0.75f;
    private float rareNpcRate = 0.25f;
    

    public void SpawnNpc()
    {
        //disable previous npc
        foreach (Transform npcChild in npcs.transform)
        {
            npcChild.gameObject.SetActive(false);
        }

        float spawnRate = Random.value;

        if (spawnRate < npcSpawnRate)
        {
            // get random spawn point
            int randomSpawnPointIndex = Random.Range(0, spawnPoints.transform.childCount);
            Transform spawnPoint = spawnPoints.transform.GetChild(randomSpawnPointIndex);

            Transform npc = npcs.transform.GetChild(0);
            float randomValue = Random.value;

            if (randomValue < rareNpcRate)
            {
                npc = npcs.transform.GetChild(2);
            }
            else if (randomValue < uncommonNpcRate)
            {
                npc = npcs.transform.GetChild(1);
            }
            
            npc.transform.position = spawnPoint.transform.position;
            npc.gameObject.SetActive(true);
        }
    }
}
