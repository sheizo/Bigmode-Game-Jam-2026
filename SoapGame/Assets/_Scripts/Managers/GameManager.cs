using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static readonly int MasterMaterialDirtAmount  = Shader.PropertyToID("_Dirt_Amount");
    public static readonly int MasterMaterialFresnelAmount = Shader.PropertyToID("_Fresnel_Clean_Amount");
    
    public const string InteractableTag = "PlayerInteractable";
    
    [SerializeField] private CinemachineBrain _cinemachineBrain;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Launcher _launcher;
    [SerializeField] private Shop _shop;


    [Header("Managers/Services")]
    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private PlayerUpgradeManager _playerUpgradeManager;
    [SerializeField] private WorldManager _worldManager;


    [Header("Money")] 
    [Tooltip("Money per meter")] [SerializeField] private float _distanceMValue; 
    [SerializeField] private int _npcValue, _objectValue, _stainValue;
    
    private RunStats _runStats;
    
    public static GameStateManager GameStateManager => Instance._gameStateManager;
    public static UIManager UIManager => Instance._uiManager;
    public static PlayerUpgradeManager PlayerUpgradeManager => Instance._playerUpgradeManager;
    public static PlayerStats PlayerStats => Instance._playerStats;
    public static WorldManager World => Instance._worldManager;
    public static Shop Shop => Instance._shop;

    public static float DistanceMValue => Instance._distanceMValue;
    public static int NpcValue => Instance._npcValue;
    public static int ObjectValue => Instance._objectValue;
    public static int StainValue => Instance._stainValue;

    public static Transform PlayerTransform => Instance._playerController.transform;

    public static GameState CurrentGameState => Instance._cinemachineBrain.IsBlending ? GameState.NONE : Instance._gameStateManager.CurrentGameState;


    private bool _isInitialized = false;

    //giga spaghetti
    private void OnEnable(){
        if (_isInitialized) return;

        //Careful changing the order of inits/functions. Spaghett.
        _playerUpgradeManager.Init();
        LoadGame();

        _gameStateManager.Init();
        _uiManager.Init();
        _worldManager.Init();
        _shop.Init();
        
        _launcher.OnLaunched += PlayerGotoGameplay;
        _playerController.OnSoapDeplete += PlayerEndRun;
        
        _uiManager.OnExitShopClick += PlayerRestart;
        _uiManager.OnRestartClick += PlayerRestart;
        _uiManager.OnShopClick += PlayerGotoShop;
        

        _uiManager.UpdateGameStateCanvas(_gameStateManager.CurrentGameState);
        _gameStateManager.OnGameStateChange += _uiManager.UpdateGameStateCanvas;


#if DEVELOPMENT_BUILD
        PlayerPrefs.DeleteAll();
#endif
        

        _isInitialized = true;
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
        UpdatePlayerStats();
        
        _gameStateManager.SwitchGameState(GameState.LOSSSCREEN);
        _uiManager.ResetSoapMeter();
        _uiManager.SetRunStats(runStats);

        if (!string.IsNullOrEmpty(PlayerName.Name))
        {
            LeaderboardManager.Instance.SubmitScore(PlayerName.Name, (int) runStats.DistanceTravelled);
        }
        
    }

    private void PlayerGotoGameplay(float strength){
        print("gameplay");
        
        _gameStateManager.SwitchGameState(GameState.GAMEPLAY);
    }

    public void PlayerRestart() { 
        print("restart");
        
        _worldManager.ResetWorld();
        _launcher.ResetLauncher();
        _playerController.ResetPlayer();
        _gameStateManager.SwitchGameState(GameState.LAUNCH);
    }

    private void UpdatePlayerStats(){
        _playerStats.Money += _runStats.TotalMoneyEarned;
        SaveGame();
    }

    public static void SaveGame() {
        SaveSystem.Save(Instance._playerStats);
    }

    private void LoadGame(){
        _playerStats = SaveSystem.LoadGame() ?? _playerStats;
    }

    public void HitStop(float time){
        Time.timeScale = 1;
        StartCoroutine(HitStopRoutine(time));
    }

    private IEnumerator HitStopRoutine(float time){
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
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

    private void OnDestroy() {
        _launcher.OnLaunched -= PlayerGotoGameplay;
        _playerController.OnSoapDeplete -= PlayerEndRun;
        
        _uiManager.OnExitShopClick -= PlayerRestart;
        _uiManager.OnRestartClick -= PlayerRestart;
        _uiManager.OnShopClick -= PlayerGotoShop;
    }
}
