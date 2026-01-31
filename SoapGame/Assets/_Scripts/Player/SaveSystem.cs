using UnityEngine;

public static class SaveSystem
{
    private const string PrefName = "PlayerStats";


    public static PlayerStats GetExistingSave(){
        if (PlayerPrefs.HasKey(PrefName)){
            string json = PlayerPrefs.GetString(PrefName);
            return JsonUtility.FromJson<PlayerStats>(json);
        }

        return null;
    }
    
    public static void Save(PlayerStats stats)
    {
        string json = JsonUtility.ToJson(stats);
        PlayerPrefs.SetString(PrefName, json);
        PlayerPrefs.Save();
        Debug.Log("Progress Saved.");
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(PrefName);
        PlayerPrefs.Save();
    }
}