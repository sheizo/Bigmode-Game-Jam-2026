using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public enum GameState
{
    NONE = -1,
    LAUNCH = 0,
    SHOP = 1,
    GAMEPLAY = 2,
    LOSSSCREEN = 3,
    MENU = 4,
}

public class GameStateManager : MonoBehaviour
{
    [SerializeField] List<GameStateBase> _gameStates;
    [SerializeField] GameState _initialGameState;

    private Dictionary<GameState, GameStateBase> _gameStateDict;

    private GameState _currentGameState;

    public GameState CurrentGameState =>  _currentGameState;

    public Action<GameState> OnGameStateChange;
    
    
    public void Init()
    {
        _currentGameState = _initialGameState;

        _gameStateDict = new Dictionary<GameState, GameStateBase>();
        foreach(GameStateBase gameState in _gameStates)
        {
            if (gameState.GameState != _currentGameState && gameState._camera)
                gameState._camera.enabled = false;

            _gameStateDict.Add(gameState.GameState, gameState);
        }
    }

    public void SwitchGameState(GameState gameState)
    {
        //disable current gameState
        if (_gameStateDict.ContainsKey(_currentGameState))
        {
            _gameStateDict[_currentGameState].OnExited();
        }
        else
        {
            Debug.LogError("Trying to exit invalid game state. (current gameState not present in dict)");
        }

        _currentGameState = gameState;
        if (_gameStateDict.ContainsKey(_currentGameState))
        {
            _gameStateDict[_currentGameState].OnEntered();
        }
        else
        {
            Debug.LogError("Trying to enter invalid game state. (new gameState not present in dict)");
        }

        OnGameStateChange?.Invoke(_currentGameState);
    }
    

}
