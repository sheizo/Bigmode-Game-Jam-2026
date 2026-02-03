using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private CinemachineBrain _cinemachineBrain;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Launcher _launcher;
    [SerializeField] private Shop _shop;


    [Header("Managers/Services")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private PlayerUpgradeManager _playerUpgradeManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private WorldManager _worldManager;

    public static GameStateManager GameStateManager => Instance._gameStateManager;
    public static UIManager UIManager => Instance._uiManager;
    public static PlayerUpgradeManager PlayerUpgradeManager => Instance._playerUpgradeManager;
    public static AudioManager AudioManager => Instance._audioManager;
    public static PlayerStats PlayerStats => Instance._playerStats;
    public static WorldManager World => Instance._worldManager;

    public static Transform PlayerTransform => Instance._playerController.transform;

    private RunStats _runStats;
    public static GameState CurrentGameState => Instance._cinemachineBrain.IsBlending ? GameState.NONE : Instance._gameStateManager.CurrentGameState;
    

    //giga spaghetti
    private void OnEnable(){
        _gameStateManager.Init();
        _uiManager.Init();
        _playerUpgradeManager.Init();
        _audioManager.Init();
        _worldManager.Init();
        
        _launcher.OnLaunched += PlayerGotoGameplay;
        _playerController.OnSoapDeplete += PlayerEndRun;
        
        _uiManager.OnExitShopClick += PlayerRestart;
        _uiManager.OnRestartClick += PlayerRestart;
        _uiManager.OnShopClick += PlayerGotoShop;

        _uiManager.UpdateGameStateCanvas(_gameStateManager.CurrentGameState);
        _gameStateManager.OnGameStateChange += _uiManager.UpdateGameStateCanvas;
    }

    protected override void Awake(){
        base.Awake();
#if DEVELOPMENT_BUILD
        PlayerPrefs.DeleteAll();
#endif
        
        LoadGame();
        
    }
    
    private void Start(){
        _uiManager.UpdateMoney(_playerStats.Money);
    }

    private void PlayerGotoShop(){
        print("shop");
        
        _uiManager.UpdateMoney(_playerStats.Money);
        _gameStateManager.SwitchGameState(GameState.SHOP);
    }
    private void PlayerEndRun(RunStats runStats){
        print("end run");
        _runStats = runStats;
        
        _gameStateManager.SwitchGameState(GameState.LOSSSCREEN);
        _uiManager.ResetSoapMeter();
        
    }

    private void PlayerGotoGameplay(float strength){
        print("gameplay");
        
        _gameStateManager.SwitchGameState(GameState.GAMEPLAY);
    }

    public void PlayerRestart() { 
        print("restart");
        
        _playerController.ResetPlayer();
        _worldManager.ResetWorld();
        _launcher.ResetLauncher();
        _gameStateManager.SwitchGameState(GameState.LAUNCH);
    }

    public static void SaveGame() {
        SaveSystem.Save(Instance._playerStats);
    }

    private void LoadGame(){
        _playerStats = SaveSystem.LoadGame() ?? _playerStats;
        
        //_playerUpgradeManager.LoadAllUpgrades();
    }

    
    
    [ContextMenu("Save Stats")]
    private void SaveGameContext(){
        SaveSystem.SavePlayerStats(_playerStats);
    }
    

    [ContextMenu("Nuke PlayerPrefs")]
    private void AddMoney(){
        PlayerPrefs.DeleteAll();
    }

    
    // private void OnValidate(){
    //     if (!Application.isPlaying){
    //         SaveSystem.SavePlayerStats(_playerStats);
    //     }
    // }

    private void OnDisable(){
        _launcher.OnLaunched -= PlayerGotoGameplay;
        _playerController.OnSoapDeplete -= PlayerEndRun;
        
        _uiManager.OnExitShopClick -= PlayerRestart;
        _uiManager.OnRestartClick -= PlayerRestart;
        _uiManager.OnShopClick -= PlayerGotoShop;
        
        
    }
    
    
}
