using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private bool _isGameSceneActive = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load the game scene additively but don't activate it yet
        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive).completed += (op) =>
        {
            Scene gameScene = SceneManager.GetSceneByName("MainScene");
            foreach (var go in gameScene.GetRootGameObjects())
                go.SetActive(false); // hide game scene
        };
    }

    public void ShowGame()
    {
        Scene gameScene = SceneManager.GetSceneByName("MainScene");
        Scene menuScene = SceneManager.GetSceneByName("StartScene");

        foreach (var go in menuScene.GetRootGameObjects())
            go.SetActive(false); // hide menu

        foreach (var go in gameScene.GetRootGameObjects())
            go.SetActive(true); // show game

        _isGameSceneActive = true;
    }

    public void ShowMenu()
    {
        Scene gameScene = SceneManager.GetSceneByName("MainScene");
        Scene menuScene = SceneManager.GetSceneByName("StartScene");

        foreach (var go in gameScene.GetRootGameObjects())
            go.SetActive(false); // hide game

        foreach (var go in menuScene.GetRootGameObjects())
            go.SetActive(true); // show menu

        _isGameSceneActive = false;
    }

    void Update()
    {
        if (!_isGameSceneActive)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ShowMenu();
        }
    }
}
