using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    [Header("New Upgrades")]
    public SoapUpgrade SoapUpgrade;
    public SpeedUpgrade SpeedUpgrade;
    public RampUpgrade RampUpgrade;
    public CleanUpgrade CleanUpgrade;
    public FloatUpgrade LaunchForce;
    public FloatUpgrade TurnStrength;
    public FloatUpgrade SlamForce;
    public RangeUpgrade Bounciness;

    private const string SaveKey = "PlayerUpgrades";

    private List<IUpgrade> _allUpgrades;

    public List<IUpgrade> AllUpgrades => _allUpgrades;

    public void Init()
    {
        _allUpgrades = new()
        {
            SoapUpgrade,
            SpeedUpgrade,
            RampUpgrade,
            CleanUpgrade,
            LaunchForce,
            TurnStrength,
            SlamForce,
            Bounciness
        };
    }

    [ContextMenu("Save")]
    public void SaveAllUpgrades()
    {
        Dictionary<string, int> levels = new();

        foreach (var upgrade in _allUpgrades)
        {
            if (string.IsNullOrEmpty(upgrade.Id)) continue;

            levels[upgrade.Id] = upgrade.CurrentLevel;
        }

        string json = JsonConvert.SerializeObject(levels);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    [ContextMenu("Load Save")]
    public void LoadAllUpgrades()
    {
        foreach (var upgrade in _allUpgrades)
        {
            upgrade.CurrentLevel = 0;
        };

        if (!PlayerPrefs.HasKey(SaveKey)) return;

        string json = PlayerPrefs.GetString(SaveKey);
        Dictionary<string, int> levels = JsonConvert.DeserializeObject<Dictionary<string,int>>(json);

        foreach (var upgrade in _allUpgrades)
        {
            if (string.IsNullOrEmpty(upgrade.Id)) continue;

            if (levels.TryGetValue(upgrade.Id, out int level))
            {
                upgrade.CurrentLevel = level;
            }
        }
    }

    [ContextMenu("Reset all")]
    public void ResetAllUpgrades()
    {
        foreach (var upgrade in _allUpgrades)
        {
            upgrade.CurrentLevel = 0;
        }

        PlayerPrefs.DeleteKey(SaveKey);

        GameManager.Shop.Refresh();
    }

    [ContextMenu("Max all")]
    public void MaxAllUpgrades()
    {
        foreach (var upgrade in _allUpgrades)
        {
            upgrade.CurrentLevel = upgrade.MaxLevel - 1;
        }

        SaveAllUpgrades();
    }

    // private void OnValidate(){
    //     if(!Application.isPlaying) SaveAllUpgrades();
    // }
}
