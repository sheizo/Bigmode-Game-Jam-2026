using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class World : MonoBehaviour
{
    [SerializeField] private Level _levelPrefab;
    [SerializeField] private GameObject _backWallPrefab;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _playerVisual;

    
    [SerializeField] private World _worldPrefab;

    private World _currentWorld;

    [SerializeField] private int _copyAmount = 3;
    [SerializeField] private int _segmentDistanceDisappearance = 2;

    private List<Level> _levelInstances;
    private GameObject _backWall;
    private float _backWallPosZ;
    private float _backWallPosY;
    private float _zPos;
    private LayerMask _stainMask;

    private Bounds _levelBounds;
    public Bounds LevelBounds => _levelBounds;

    void Awake()
    {
        _stainMask  = LayerMask.GetMask("Stain");

        _levelInstances = new List<Level>();
        GameObject container = new GameObject("LevelCopies");
        
        //Store all _levelPrefab children components renderers
        Renderer[] renderers = _levelPrefab.GetComponentsInChildren<Renderer>();
        
        //define level maximum bounds
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (renderers[i] == null) continue;
            _levelBounds.Encapsulate(renderers[i].bounds);
        }

        //Create x copies of _levelPrefab 
        for (int i = 0; i < _copyAmount; i++)
        {
            _zPos = i * _levelBounds.size.z;
            Level level = Instantiate(_levelPrefab, new Vector3(0, 0, _zPos), Quaternion.identity, container.transform);
            level.SpawnNpc();
            level.SpawnStains();
            _levelInstances.Add(level);
        }
        
        // offset from centers 
        _backWallPosZ = -(_levelBounds.size.z / 2);
        _backWallPosY = _backWallPrefab.transform.localScale.y / 2;

        _backWall = Instantiate(_backWallPrefab, new Vector3(0, _backWallPosY, _backWallPosZ), Quaternion.identity);

        //Disable original objects
        _levelPrefab.gameObject.SetActive(false);
        _backWallPrefab.SetActive(false);
    }

    void Start()
    {
        //Set all level objects as static
        foreach (Level instance in _levelInstances)
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
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            return;
        }
        
        IsPlayerOnStain();
        ChangeSegmentPos();
    }

    private void ChangeSegmentPos()
    {
        for (int i = 0; i < _copyAmount; i++)
        {
            if (_player.transform.position.z >= _levelInstances[i].transform.position.z + (_levelBounds.size.z * _segmentDistanceDisappearance))
            {
                // increase zPos by bounds so we know where to transition next level copy
                _zPos += _levelBounds.size.z;
                _backWallPosZ += _levelBounds.size.z;
                _backWall.transform.position = new Vector3(0, _backWallPosY, _backWallPosZ);
                _levelInstances[i].transform.position = new Vector3(0,0, _zPos);
                _levelInstances[i].SpawnNpc();
                _levelInstances[i].SpawnStains();
            } 
        }
    }

    public bool IsPlayerOnStain() 
    {
        RaycastHit hit;

        if (Physics.Raycast(_player.transform.position, Vector3.down, out hit, 1f, _stainMask))
        {
            DecalProjector stainHit = hit.collider.gameObject.GetComponent<DecalProjector>();
            DOTween.To(() => stainHit.fadeFactor, x => stainHit.fadeFactor = x, 0f, 0.5f); 
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SpawnLevelAtPos(Vector3 pos)
    {
        Level level = Instantiate(_levelPrefab, pos, Quaternion.identity);
        level.gameObject.SetActive(true);
    }
}
