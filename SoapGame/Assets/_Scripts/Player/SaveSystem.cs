using System;
using Newtonsoft.Json;
using UnityEngine;


[Serializable]
public class PlayerStats
{
    public int Money;
}

public static class SaveSystem
{
    private const string SavePlayerStatsKey = "PlayerStats";
    
    public static void Save(PlayerStats playerStats)
    {
        GameManager.PlayerUpgradeManager.SaveAllUpgrades();
    
        string json = JsonConvert.SerializeObject(playerStats);
        PlayerPrefs.SetString(SavePlayerStatsKey, json);
        PlayerPrefs.Save();
        
        Debug.Log("Progress Saved.");
    }

    public static PlayerStats LoadGame(){
        GameManager.PlayerUpgradeManager.LoadAllUpgrades();
        string json = PlayerPrefs.GetString(SavePlayerStatsKey);
        PlayerStats stats = JsonConvert.DeserializeObject<PlayerStats>(json);
        
        Debug.Log("Progress Loaded.");
        
        return stats ?? new PlayerStats();
    }

    public static void SavePlayerStats(PlayerStats playerStats){
        string json = JsonConvert.SerializeObject(playerStats);
        PlayerPrefs.SetString(SavePlayerStatsKey, json);
        PlayerPrefs.Save();
    }

}