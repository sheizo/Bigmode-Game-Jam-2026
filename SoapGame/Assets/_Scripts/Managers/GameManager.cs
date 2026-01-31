using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    
    [SerializeField] private AudioMixer _audioMixer;
    
    [SerializeField] private PlayerProgressHubSO playerProgressHubSo;
    
    [SerializeField]private PlayerStats _playerStats;
    
    protected override void Awake(){
        base.Awake();
        
        playerProgressHubSo.LoadOrInitialize();
    }
    
    private void Start(){
        PlayerStats existingStats = SaveSystem.GetExistingSave();
        playerProgressHubSo.LiveData = existingStats ?? new PlayerStats();
        
        _playerStats = playerProgressHubSo.LiveData;
    }


    private void Update(){
        if (Keyboard.current.rKey.wasPressedThisFrame){
            _playerStats.LaunchForce++;
        }
        SaveSystem.Save(_playerStats);
        
    }


    private void SaveGame() {
        SaveSystem.Save(_playerStats);
    }

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();
        SaveGame(); 
    }
}
