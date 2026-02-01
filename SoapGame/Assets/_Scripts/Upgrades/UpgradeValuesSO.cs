
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade Values Asset")]
public class UpgradeValuesSO : ScriptableObject
{
    public List<float> _moneyCosts;
    
    public List<float> MaxAirSpeed, MaxGroundSpeed, LaunchForce, TurnStrength;
    public List<int> MaxSoap;
    [Range(0,1)] public List<float> SoapRefillOnClean;
    [Tooltip("X: Down, Y: Forwards")] public List<Vector2> RampBoostSpeed;    
    public List<float> SlamForce;    
    [Range(0,1)] public List<float> Bounciness;
    [Range(0,1)] public List<float> BadRampChance;
    
}
