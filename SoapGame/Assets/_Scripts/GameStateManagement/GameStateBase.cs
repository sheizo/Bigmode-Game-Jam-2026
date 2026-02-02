using Unity.Cinemachine;
using UnityEngine;

public class GameStateBase : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    public CinemachineCamera _camera;

    public GameState GameState => _gameState;

    public virtual void OnEntered() 
    {
        if(_camera) _camera.enabled = true;
    }

    public virtual void OnExited()
    {
        if(_camera) _camera.enabled = false;
    }
}
