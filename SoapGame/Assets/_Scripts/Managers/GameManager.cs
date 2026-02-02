using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Launcher _launcher;
    [SerializeField] private Shop _shop;

    private UIManager _UIManager;
    
    private RunStats _runStats;
    
    public GameState CurrentGameState => _gameStateManager.CurrentGameState;

    public PlayerStats PlayerStats => _playerStats;
    
    

    //giga spaghetti
    private void OnEnable(){
        _UIManager = UIManager.Instance;
        
        _launcher.OnLaunched += PlayerLaunched;
        _playerController.OnSoapDeplete += PlayerEndRun;
        
        _UIManager.OnShopExit += PlayerRestart;
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
        
        _gameStateManager.SwitchGameState(GameState.SHOP);
    }
    private void PlayerEndRun(RunStats runStats){
        _runStats = runStats;
        
        _gameStateManager.SwitchGameState(GameState.LOSSSCREEN);
        
    }
    private void PlayerLaunched(float strength){
        _launcher.ResetLauncher();
        
        _gameStateManager.SwitchGameState(GameState.GAMEPLAY);
    }
    private void PlayerRestart(){
        _playerController.ResetPlayer();
        
        _gameStateManager.SwitchGameState(GameState.LAUNCH);
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
        _launcher.OnLaunched -= PlayerLaunched;
        _playerController.OnSoapDeplete -= PlayerEndRun;
        
        _UIManager.OnShopExit -= PlayerRestart;
        _UIManager.OnPlayerRestart -= PlayerRestart;
        
    }
}
