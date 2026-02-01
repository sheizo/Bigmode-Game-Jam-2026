using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private Level _levelPrefab;
    [SerializeField] private GameObject player;

    [SerializeField] private int copyAmount = 3;
    [SerializeField] private int segmentDistanceDisappearance = 2;

    private List<Level> levelInstances;

    private float zPos;

    private Bounds _levelBounds;
    public Bounds LevelBounds => _levelBounds;

    void Awake()
    {
        levelInstances = new List<Level>();
        GameObject container = new GameObject("LevelCopies");
        
        //Save all _levelPrefab children components renderers
        Renderer[] renderers = _levelPrefab.GetComponentsInChildren<Renderer>();
        
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (renderers[i] == null) continue;
            _levelBounds.Encapsulate(renderers[i].bounds);
        }

        //Create x copies of _levelPrefab 
        for (int i = 0; i < copyAmount; i++)
        {
            zPos = i * _levelBounds.size.z;
            Level level = Instantiate(_levelPrefab, new Vector3(0, 0, zPos), Quaternion.identity, container.transform);
            levelInstances.Add(level);
        }

        //Disable original object
        _levelPrefab.gameObject.SetActive(false);
    }

    void Start()
    {
        //Set all level objects as static
        foreach (Level instance in levelInstances)
        {
            instance.gameObject.isStatic = true;

            foreach (Transform child in instance.transform)
            {
                child.gameObject.isStatic = true;
            }
        }
    }

    void Update()
    {
        ChangeSegmentPos();
    }

    private void ChangeSegmentPos()
    {
        for (int i = 0; i < copyAmount; i++)
        {
            if (player.transform.position.z >= levelInstances[i].transform.position.z + (_levelBounds.size.z * segmentDistanceDisappearance))
            {
                // increase zPos by bounds so we know where to "spawn" next level copy
                zPos += _levelBounds.size.z;
                levelInstances[i].transform.position = new Vector3(0,0, zPos);
                levelInstances[i].SpawnNpc();
            } 
        }
    }
}
