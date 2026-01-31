using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public enum RampStartingDirection
{
    Down,
    Up
}

public class Ramp : MonoBehaviour
{
    private static readonly int MaterialObjectLength = Shader.PropertyToID("_ObjectLength");
    
    
    [SerializeField] private Transform _rampParticlesTransform;
    [SerializeField] private SplineAnimate.EasingMode _particleEasing;
    [SerializeField] private float _particlePathDuration = 1, _pathAppearMultiplier = 0.3f;
    [SerializeField] private Vector3 _startPointOffset;
    [SerializeField] private float _despawnTime;
    [SerializeField] private RampStartingDirection _startingDirection;

    [SerializeField]private float length;
    
    private Ramp _connectedRamp;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private ParticleSystem _rampParticles;
    private SplineContainer _rampSpline;
    private SplineAnimate _particleSplineAnimate;
    
    private MeshRenderer _rampMeshRenderer;
    private Material _rampMaterial;
    private float _despawnTimer;

    public Vector3 StartPointOffset => _startPointOffset;
    public Vector3 StartPoint => _startPoint;
    public Vector3 EndPoint => _endPoint;
    public RampStartingDirection StartingDirection => _startingDirection;

    public Ramp ConnectedRamp{
        get => _connectedRamp;
        set => _connectedRamp = value;
    }



    private void Awake(){
        _rampParticles = _rampParticlesTransform.GetComponent<ParticleSystem>();
        _particleSplineAnimate = _rampParticles.GetComponent<SplineAnimate>();
        _rampSpline = GetComponent<SplineContainer>();
        _rampMeshRenderer = GetComponent<MeshRenderer>();
        _rampMaterial = _rampMeshRenderer.material;
        
        _particleSplineAnimate.Easing = _particleEasing;
        _particleSplineAnimate.Duration = _particlePathDuration;

        var knots = _rampSpline.Spline.ToArray();
        _startPoint = (Vector3)knots[0].Position;
        _endPoint = (Vector3)knots[^1].Position;
    }

    private void Start(){
        _rampMaterial.SetFloat(MaterialObjectLength, _rampSpline.CalculateLength());
        _rampMaterial.SetFloat("_Appear", 0);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_rampMaterial.DOFloat(1f, "_Appear", _particlePathDuration * _pathAppearMultiplier));    //animates alpha on material along spline
        sequence.Append(_rampMaterial.DOFloat(0f, "_Appear", _despawnTime));
        sequence.AppendCallback(()=>Destroy(this.gameObject));

    }

    private void Update(){
        if (Mathf.Approximately(_particleSplineAnimate.NormalizedTime, 1)){
            _rampParticles.Stop();
        }
    }

    
    private void OnDrawGizmos(){
        _rampSpline = GetComponent<SplineContainer>();
        
        var knots = _rampSpline.Spline.ToArray();
        
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere((Vector3)knots[0].Position + - _startPointOffset, 0.4f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere((Vector3)knots[^1].Position , 0.4f);

        length = _rampSpline.CalculateLength();
    }
}
