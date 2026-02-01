
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade Values Asset")]
public class UpgradeValuesSO : ScriptableObject
{
    public List<float> MaxSpeed, LaunchForce, TurnStrength;
    public List<int> MaxSoap;
    public List<float> SoapRefillOnClean;
    [Tooltip("X: Down, Y: Forwards")] public List<Vector2> RampBoostSpeed;    
    public List<float> SlamForce;    
    public List<float> Bounciness;    
}
