using System;
using System.Collections.Generic;
using UnityEngine;



// Base class with common fields and level logic
[Serializable]
public abstract class UpgradeBase 
{
    [Tooltip("Unique ID used for saving/loading")]
    public string Id;
    public string Name;

    public int CurrentLevel = 0;


    public abstract int NextLevelCost();
    public abstract int MaxLevel { get; }

    public bool CanUpgrade => CurrentLevel < MaxLevel-1 ;

    public bool Upgrade()
    {
        if (CanUpgrade){
            CurrentLevel++;
            return true;
        }
        
        return false;
    }
}

// Standard float upgrade
[Serializable]
public class UpgradeFloat : UpgradeBase
{
    [Serializable]
    public class UpgradeLevel
    {
        public float Value;
        public int Cost;
    }

    public List<UpgradeLevel> Levels = new();

    public float CurrentValue =>
        Levels.Count == 0 ? 0f : Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)].Value;

    public override int NextLevelCost() =>
        CanUpgrade ? Levels[CurrentLevel + 1].Cost : 0;

    public override int MaxLevel => Levels.Count;
    
}

// Float upgrade constrained 0–1
[Serializable]
public class UpgradeRange : UpgradeBase
{
    [Serializable]
    public class UpgradeLevel
    {
        [Range(0f, 1f)]
        public float Value;
        public int Cost;
    }

    public List<UpgradeLevel> Levels = new();

    public float CurrentValue =>
        Levels.Count == 0 ? 0f : Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)].Value;

    public override int NextLevelCost() =>
        CanUpgrade ? Levels[CurrentLevel + 1].Cost : 0;

    public override int MaxLevel => Levels.Count;
}

// Int upgrade
[Serializable]
public class UpgradeInt : UpgradeBase
{
    [Serializable]
    public class UpgradeLevel
    {
        public int Value;
        public int Cost;
    }

    public List<UpgradeLevel> Levels = new();

    public int CurrentValue =>
        Levels.Count == 0 ? 0 : Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)].Value;

    public override int NextLevelCost() =>
        CanUpgrade ? Levels[CurrentLevel + 1].Cost : 0;

    public override int MaxLevel => Levels.Count;
}

// Vector2 upgrade
[Serializable]
public class UpgradeVector2 : UpgradeBase
{
    [Serializable]
    public class UpgradeLevel
    {
        public Vector2 Value;
        public int Cost;
    }

    public List<UpgradeLevel> Levels = new();

    public Vector2 CurrentValue =>
        Levels.Count == 0 ? Vector2.zero : Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)].Value;

    public override int NextLevelCost() =>
        CanUpgrade ? Levels[CurrentLevel + 1].Cost : 0;

    public override int MaxLevel => Levels.Count;
}
