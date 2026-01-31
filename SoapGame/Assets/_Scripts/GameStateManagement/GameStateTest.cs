using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateTest : MonoBehaviour
{
    [SerializeField] private GameStateManager _gameStateManager;

    [ContextMenu("Switch to LAUNCH")]
    public void SwitchToLaunch()
    {
        _gameStateManager.SwitchGameState(GameState.LAUNCH);
    }

    [ContextMenu("Switch to Shop")]
    public void SwitchToShop()
    {
        _gameStateManager.SwitchGameState(GameState.SHOP);
    }


    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            SwitchToLaunch();
        }
        
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            SwitchToShop();
        }
    }
}
