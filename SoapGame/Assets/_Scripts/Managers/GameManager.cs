using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Launcher _launcher;

    public GameState CurrentGameState => _gameStateManager.CurrentGameState;

    public PlayerStats PlayerStats => _playerStats;

    private void OnEnable(){
        _launcher.OnLaunched += SwitchToGameplay;
    }

    protected override void Awake(){
        base.Awake();
        

    }

    private void Start(){
        LoadGame();
        SaveGame(); 
        
    }

    private void SwitchToGameplay(){
        _gameStateManager.SwitchGameState(GameState.GAMEPLAY);
    }

    public void SaveGame() {
        SaveSystem.Save(_playerStats);
    }

    private void LoadGame(){
        _playerStats = SaveSystem.LoadGame();
    }

    [ContextMenu("Save Game")]
    public void SaveGameContext(){
        SaveGame();
    }

    [ContextMenu("Add Money")]
    public void AddMoney(){
        _playerStats.Money+=50;
        SaveGame();
    }

    private void OnValidate(){
        SaveSystem.SavePlayerStats(_playerStats);
    }

    private void OnDisable(){
        _launcher.OnLaunched -= SwitchToGameplay;
    }
}
