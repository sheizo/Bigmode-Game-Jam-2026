using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private AudioMixer _audioMixer;
    
    [SerializeField] private PlayerProgressHubSO _playerProgressHubSo;
    
    [SerializeField]private PlayerStats _playerStats;
    
    protected override void Awake(){
        base.Awake();
        
        _playerProgressHubSo.LoadOrInitialize();
    }
    
    private void Start(){
        PlayerStats existingStats = SaveSystem.GetExistingSave();
        _playerProgressHubSo.LiveData = existingStats ?? new PlayerStats();
        
        _playerStats = _playerProgressHubSo.LiveData;
    }

    private void SaveGame() {
        SaveSystem.Save(_playerStats);
    }

    [ContextMenu("Reset Player Values")]
    private void ResetPlayerValues(){
        _playerProgressHubSo.ResetToDefault();
        _playerStats = _playerProgressHubSo.LiveData;
    }

    private void OnValidate(){
        _playerStats = _playerProgressHubSo.LiveData;
        SaveGame();
    }

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();
        SaveGame(); 
    }
}
