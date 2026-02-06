using System.Collections.Generic;
using UnityEngine;

public interface IUpgrade
{
    string Id { get; }
    string Name { get; }
    int CurrentLevel { get; set; }
    bool CanUpgrade { get; }

    int MaxLevel { get; }
    int NextLevelCost();
    bool Upgrade();
}

public interface IUpgradeLevel
{
    int Cost { get; }
}

public abstract class UpgradeBase<TLevel> : ScriptableObject, IUpgrade
    where TLevel : IUpgradeLevel
{
    [SerializeField] protected string _id;
    [SerializeField] protected string _name;
    [SerializeField] protected List<TLevel> _levels;

    protected int _currentLevel = 0;

    public string Id => _id;
    public string Name => _name;

    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = Mathf.Clamp(value, 0, _levels.Count - 1);
    }

    public int MaxLevel => _levels.Count;
    public bool CanUpgrade => CurrentLevel < MaxLevel - 1;

    public int NextLevelCost()
    {
        return CanUpgrade ? _levels[CurrentLevel + 1].Cost : 0;
    }

    public bool Upgrade()
    {
        if (!CanUpgrade)
            return false;

        _currentLevel++;
        OnUpgraded(_levels[CurrentLevel]);
        return true;
    }

    protected virtual void OnUpgraded(TLevel newLevel) { }

    public TLevel CurrentLevelData => _levels[CurrentLevel];
}

[CreateAssetMenu(menuName = "Upgrades/Soap Upgrade")]
public class SoapUpgrade : UpgradeBase<SoapUpgradeLevel> { }

[CreateAssetMenu(menuName = "Upgrades/Speed Upgrade")]
public class SpeedUpgrade : UpgradeBase<SpeedUpgradeLevel> { }

[CreateAssetMenu(menuName = "Upgrades/Ramp Upgrade")]
public class RampUpgrade : UpgradeBase<RampUpgradeLevel> { }

[CreateAssetMenu(menuName = "Upgrades/Clean Upgrade")]
public class CleanUpgrade : UpgradeBase<CleanUpgradeLevel> { }

[CreateAssetMenu(menuName = "Upgrades/Float Upgrade")]
public class FloatUpgrade : UpgradeBase<FloatUpgradeLevel> { }

[CreateAssetMenu(menuName = "Upgrades/Range Upgrade")]
public class RangeUpgrade : UpgradeBase<RangeUpgradeLevel> { }

[System.Serializable]
public class SoapUpgradeLevel : IUpgradeLevel
{
    public int MaxSoap;
    [Range(0f, 1f)] public float GroundSoapUsage;

    [SerializeField] private int cost;
    public int Cost => cost;
}

[System.Serializable]
public class SpeedUpgradeLevel : IUpgradeLevel
{
    public float MaxAirSpeed;
    public float MaxGroundSpeed;

    [SerializeField] private int cost;
    public int Cost => cost;
}

[System.Serializable]
public class RampUpgradeLevel : IUpgradeLevel
{
    [Range(0f, 1f)] public float BadRampChance;
    public Vector2 RampSpeedBoost;

    [SerializeField] private int cost;
    public int Cost => cost;
}

[System.Serializable]
public class CleanUpgradeLevel : IUpgradeLevel
{
    [Range(0f, 1f)] public float SoapRefillOnClean;
    [Range(0f, 1f)] public float TimeToClean;

    [SerializeField] private int cost;
    public int Cost => cost;
}

[System.Serializable]
public class IntUpgradeLevel : IUpgradeLevel
{
    public int Value;

    [SerializeField] private int cost;
    public int Cost => cost;
}

[System.Serializable]
public class FloatUpgradeLevel : IUpgradeLevel
{
    public float Value;

    [SerializeField] private int cost;
    public int Cost => cost;
}

[System.Serializable]
public class RangeUpgradeLevel : IUpgradeLevel
{
    [Range(0f, 1f)] public float Value;

    [SerializeField] private int cost;
    public int Cost => cost;
}

