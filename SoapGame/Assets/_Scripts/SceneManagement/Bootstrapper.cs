using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Bootstrapper : PersistentSingleton<Bootstrapper>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static async void Init(){
#if UNITY_EDITOR
            // check if bootstrapper is active
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName != "Bootstrapper") return;
#endif
            
            Debug.Log("Bootstrapper...");
            await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
        }
    }
}