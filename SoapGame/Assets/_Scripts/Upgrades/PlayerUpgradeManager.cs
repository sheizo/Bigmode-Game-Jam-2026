using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerUpgradeManager : Singleton<PlayerUpgradeManager>
{
    [Header("Float Upgrades")]
    public UpgradeFloat MaxAirSpeed;
    public UpgradeFloat MaxGroundSpeed;
    public UpgradeFloat LaunchForce;
    public UpgradeFloat TurnStrength;
    public UpgradeFloat SlamForce;

    [Header("Range Upgrades")]
    public UpgradeRange SoapRefillOnClean;
    public UpgradeRange Bounciness;
    public UpgradeRange BadRampChance;

    [Header("Int Upgrades")]
    public UpgradeInt MaxSoap;

    [Header("Vector2 Upgrades")]
    public UpgradeVector2 RampBoostSpeed;

    private const string SaveKey = "PlayerUpgrades";

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    // Automatically find all UpgradeBase fields
    public IEnumerable<FieldInfo> GetAllUpgradeFields()
    {
        var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (typeof(UpgradeBase).IsAssignableFrom(field.FieldType))
                yield return field;
        }
    }

    [ContextMenu("Save")]
    public void SaveAllUpgrades()
    {
        Dictionary<string, int> levels = new();

        foreach (var field in GetAllUpgradeFields())
        {
            var upgrade = field.GetValue(this) as UpgradeBase;
            if (upgrade == null || string.IsNullOrEmpty(upgrade.Id)) continue;

            levels[upgrade.Id] = upgrade.CurrentLevel;
        }

        string json = JsonConvert.SerializeObject(levels);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    [ContextMenu("Load Save")]
    public void LoadAllUpgrades()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;

        string json = PlayerPrefs.GetString(SaveKey);
        Dictionary<string, int> levels = JsonConvert.DeserializeObject<Dictionary<string,int>>(json);

        foreach (var field in GetAllUpgradeFields())
        {
            var upgrade = field.GetValue(this) as UpgradeBase;
            if (upgrade == null || string.IsNullOrEmpty(upgrade.Id)) continue;

            if (levels.TryGetValue(upgrade.Id, out int level))
            {
                upgrade.CurrentLevel = Mathf.Clamp(level, 0, upgrade.MaxLevel - 1);
            }
        }
    }

    [ContextMenu("Reset all")]
    public void ResetAllUpgrades()
    {
        foreach (var field in GetAllUpgradeFields())
        {
            var upgrade = field.GetValue(this) as UpgradeBase;
            if (upgrade == null) continue;

            upgrade.CurrentLevel = 0;
        }

        PlayerPrefs.DeleteKey(SaveKey);
    }
}
