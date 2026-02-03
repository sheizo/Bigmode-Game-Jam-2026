using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] CinemachineBrain _cinemachineBrain;
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Launcher _launcher;
    [SerializeField] private Shop _shop;
    [SerializeField] private World _worldPrefab;

    private World _currentWorld;
    private UIManager _UIManager;
    
    private RunStats _runStats;
    
    public GameState CurrentGameState => _cinemachineBrain.IsBlending ? GameState.NONE : _gameStateManager.CurrentGameState;

    public PlayerStats PlayerStats => _playerStats;
    
    

    //giga spaghetti
    private void OnEnable(){
        _UIManager = UIManager.Instance;
        
        _launcher.OnLaunched += PlayerGotoGameplay;
        _playerController.OnSoapDeplete += PlayerEndRun;
        
        _UIManager.OnPlayerExitShop += PlayerRestart;
        _UIManager.OnPlayerRestart += PlayerRestart;
        _UIManager.OnPlayerGotoShop += PlayerGotoShop;
    }

    protected override void Awake(){
        base.Awake();
        

    }

    private void Start(){
        LoadGame();
        SaveGame(); 
        
    }

    private void PlayerGotoShop(){
        print("shop");
        
        _gameStateManager.SwitchGameState(GameState.SHOP);
    }
    private void PlayerEndRun(RunStats runStats){
        print("end run");
        _runStats = runStats;
        
        _gameStateManager.SwitchGameState(GameState.LOSSSCREEN);
        _UIManager.ResetSoapMeter();
        
    }
    private void PlayerGotoGameplay(float strength){
        print("gameplay");
        
        _gameStateManager.SwitchGameState(GameState.GAMEPLAY);
    }
    private void PlayerRestart(){
        print("restart");
        
        ResetWorld();
        _playerController.ResetPlayer();
        _launcher.ResetLauncher();
        
        
        _gameStateManager.SwitchGameState(GameState.LAUNCH);
    }

    private void StartWorld()
    {
        _currentWorld = Instantiate(_worldPrefab);
    }

    private void ResetWorld()
    {
        if (_currentWorld != null)
        {
            Destroy(_currentWorld);
        }

        _currentWorld = Instantiate(_worldPrefab);
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
        _launcher.OnLaunched -= PlayerGotoGameplay;
        _playerController.OnSoapDeplete -= PlayerEndRun;
        
        _UIManager.OnPlayerExitShop -= PlayerRestart;
        _UIManager.OnPlayerRestart -= PlayerRestart;
        _UIManager.OnPlayerGotoShop -= PlayerGotoShop;
        
        
    }
}
