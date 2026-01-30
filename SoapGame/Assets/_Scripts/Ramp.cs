using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class Ramp : MonoBehaviour
{
    [SerializeField] private Transform _rampParticlesTransform;
    [SerializeField] private SplineAnimate.EasingMode _particleEasing;
    [SerializeField] private float _particlePathDuration = 1;
    [SerializeField] private Vector3 _startPointOffset;


    [SerializeField] private Ramp _connectedRamp;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private ParticleSystem _rampParticles;
    private SplineContainer _rampSpline;
    private SplineAnimate _particleSplineAnimate;

    public Vector3 StartPointOffset => _startPointOffset;
    public Vector3 StartPoint => _startPoint;
    public Vector3 EndPoint => _endPoint;

    public Ramp ConnectedRamp{
        get => _connectedRamp;
        set => _connectedRamp = value;
    }


    private void Awake(){
        _rampParticles = _rampParticlesTransform.GetComponent<ParticleSystem>();
        _particleSplineAnimate = _rampParticles.GetComponent<SplineAnimate>();
        _rampSpline = GetComponent<SplineContainer>();
        
        _particleSplineAnimate.Easing = _particleEasing;
        _particleSplineAnimate.Duration = _particlePathDuration;

        var knots = _rampSpline.Spline.ToArray();
        _startPoint = (Vector3)knots[0].Position;
        _endPoint = (Vector3)knots[^1].Position;
    }

    private void Start(){
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
    }
}
