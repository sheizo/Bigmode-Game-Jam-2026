
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrade Values Asset")]
public class UpgradeValuesSO : ScriptableObject
{
    public List<float> MaxSpeed, LaunchSpeed, TurnSpeed;
    public List<float> SoapRefillOnClean;
    public List<float> RampBoostSpeed;    
    public List<float> SlamForce;    
    public List<float> Bounciness;    
}
