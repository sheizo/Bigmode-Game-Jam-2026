
using System;
using UnityEngine;

[System.Serializable]
public class PlayerStats : ICloneable
{
    public int Money;
    
    //upgrade counts
    public int MaxAirSpeed, MaxGroundSpeed, LaunchForce, TurnStrength;
    public int MaxSoap, SoapRefillOnClean;
    public int RampBoostSpeed;
    public int SlamForce;    
    public int Bounciness;
    
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}