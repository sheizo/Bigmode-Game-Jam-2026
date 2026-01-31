using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Progress Hub")]
public class PlayerProgressHubSO : ScriptableObject
{
    public PlayerStats LiveData;

    public void LoadOrInitialize()
    {
        PlayerStats saved = SaveSystem.GetExistingSave();
        
        if (saved != null)
            LiveData = saved;
        else
            ResetToDefault();
    }

    [ContextMenu("Reset Save Data")]
    public void ResetToDefault()
    {
        LiveData = new PlayerStats();
        Save();
    }

    public void Save() => SaveSystem.Save(LiveData);
}