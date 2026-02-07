using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private WorldSegment _levelPrefab;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private int _segmentPoolCount;
    [SerializeField] private float _segmentSpacing;

    [Range(2f, 5f)]
    [SerializeField] private int _minimumSegmentsAhead = 2;

    private Transform _playerTransform;
    private MoveToEndFIFO<WorldSegment> _worldSegmentPool;

    private float _segmentInitialZPos;
    private float _backWallInitialZPos;
    private float _maxZPos;

    private Bounds _levelBounds;

    private float _zPosThreshold => _maxZPos - ((_segmentPoolCount - _minimumSegmentsAhead) * _levelBounds.size.z) + _levelBounds.size.z/2;

    public void Init()
    {
        _playerTransform = GameManager.PlayerTransform;

        _worldSegmentPool = new MoveToEndFIFO<WorldSegment>();
        //Store all _levelPrefab children components renderers
        Renderer[] renderers = _levelPrefab.GetComponentsInChildren<Renderer>();
        
        //define level maximum bounds
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (renderers[i] == null) continue;
            _levelBounds.Encapsulate(renderers[i].bounds);
        }

        _segmentInitialZPos = _levelPrefab.transform.position.z;
        _backWallInitialZPos = _backWall.transform.position.z;

        _worldSegmentPool.Enqueue(_levelPrefab);

        //Create x copies of _levelPrefab 
        for (int i = 1; i < _segmentPoolCount; i++)
        {
            WorldSegment level = Instantiate(_levelPrefab, Vector3.zero, Quaternion.identity, _levelPrefab.transform.parent);
            _worldSegmentPool.Enqueue(level);
        }

        ResetWorld();
    }

    void Update()
    {
        HandleSegmentPositions();
    }

    private void HandleSegmentPositions()
    {
        if (_playerTransform.position.z >=_zPosThreshold)
        {
            WorldSegment worldSegment = _worldSegmentPool.Dequeue();
            // increase zPos by bounds so we know where to transition next level copy
            worldSegment.transform.position = new Vector3(worldSegment.transform.position.x, worldSegment.transform.position.y, _maxZPos);
            worldSegment.Reset();

            //Move the back wall
            Vector3 currWallPos = _backWall.transform.position;
            _backWall.transform.position = new Vector3(currWallPos.x, currWallPos.y, currWallPos.z + _levelBounds.size.z);

            _maxZPos += _levelBounds.size.z + _segmentSpacing;
        } 
    }

    public void ResetWorld()
    {
        //Reset the back wall
        Vector3 currWallPos = _backWall.transform.position;
        _backWall.transform.position = new Vector3(currWallPos.x, currWallPos.y, _backWallInitialZPos);

        _maxZPos = _segmentInitialZPos;

        foreach(WorldSegment worldSegment in _worldSegmentPool)
        {
            worldSegment.Reset();

            worldSegment.transform.position = new Vector3(worldSegment.transform.position.x, worldSegment.transform.position.y, _maxZPos);
            _maxZPos += _levelBounds.size.z + _segmentSpacing;
        }
    }

    private void OnValidate(){
        Renderer[] renderers = _levelPrefab.GetComponentsInChildren<Renderer>();
        
        //define level maximum bounds
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (renderers[i] == null) continue;
            _levelBounds.Encapsulate(renderers[i].bounds);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0,0, _zPosThreshold + transform.position.z), new Vector3(100f, 100f, 0.1f));

        Gizmos.color = Color.green;
        Vector3 localCenter = transform.InverseTransformPoint(_levelBounds.center);
        float zOffset = (_levelBounds.size.z / 2f) + _segmentSpacing;

        Vector3 levelBounds = _levelBounds.size;
        Gizmos.DrawWireCube(localCenter + new Vector3(0, 0, zOffset), new Vector3(levelBounds.x, levelBounds.y, 0.01f));
        Gizmos.DrawWireCube(localCenter - new Vector3(0, 0, zOffset), new Vector3(levelBounds.x, levelBounds.y, 0.01f));
    }
}
