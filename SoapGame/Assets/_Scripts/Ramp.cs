using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public enum RampDirection
{
    Down,
    Straight,
    Up
}

public class Ramp : MonoBehaviour
{
    private static readonly int MaterialObjectLength = Shader.PropertyToID("_ObjectLength");
    
    
    [SerializeField] private Transform _rampParticlesTransform;
    [SerializeField] private SplineAnimate.EasingMode _particleEasing;
    [SerializeField] private float _particlePathDuration = 1, _pathAppearMultiplier = 0.3f;
    [SerializeField] private RampDirection _startingDirection, endingDirection;
    [SerializeField] private Sprite _rampSprite;
    
    [SerializeField]private float length;

    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private ParticleSystem _rampParticles;
    private SplineContainer _rampSpline;
    private SplineAnimate _particleSplineAnimate;
    
    private MeshRenderer _rampMeshRenderer;
    private Material _rampMaterial;

    public Vector3 StartPoint => _startPoint;
    public Vector3 EndPoint => _endPoint;
    public RampDirection StartingDirection => _startingDirection;
    public RampDirection EndingDirection => endingDirection;

    public Ramp ConnectedRamp{ get; set; }

    public Sprite RampSprite => _rampSprite;


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

        float despawnTime = _rampParticles.main.startLifetime.constantMax;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_rampMaterial.DOFloat(1f, "_Appear", _particlePathDuration * _pathAppearMultiplier));    //animates alpha on material along spline
        //sequence.AppendInterval(despawnTime / 2);
        sequence.Append(_rampMaterial.DOFloat(0f, "_Appear", despawnTime ));
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
        Gizmos.DrawSphere((Vector3)knots[0].Position, 0.4f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere((Vector3)knots[^1].Position , 0.4f);

        length = _rampSpline.CalculateLength();
    }
}
