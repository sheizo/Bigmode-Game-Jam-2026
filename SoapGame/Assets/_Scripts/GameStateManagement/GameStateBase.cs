using Unity.Cinemachine;
using UnityEngine;

public class GameStateBase : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    public CinemachineCamera _camera;

    public GameState GameState => _gameState;

    public virtual void OnEntered() 
    {
        _camera.enabled = true;
    }

    public virtual void OnExited()
    {
        _camera.enabled = false;
    }
}
