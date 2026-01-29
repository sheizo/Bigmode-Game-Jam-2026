using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Image loadingBar;
        [SerializeField] private SceneGroup[] _sceneGroups;
        
        
        int _currentIndex = -1;
        SceneGroup _currentSceneGroup;
        
        
        float _targetProgress;
        bool _isLoading;
        
        public readonly SceneGroupManager manager = new SceneGroupManager();
        public int SceneGroupCount => _sceneGroups.Length;

        private void Awake(){
            manager.OnSceneLoaded += sceneName => Debug.Log($"Loaded {sceneName}");
            manager.OnSceneUnloaded += sceneName => Debug.Log($"Unloaded {sceneName}");
            manager.OnSceneGroupLoaded += OnSceneGroupLoaded;
        }

        async void Start(){
            await LoadSceneGroup(0);
        }

        private void Update(){
            if(!_isLoading) return;
        }
        
        public async Task LoadNextSceneGroup()
        {
            int nextIndex = _currentIndex + 1;
            if (nextIndex >= _sceneGroups.Length)
            {
                Debug.LogWarning("Already at last scene group.");
                return;
            }

            await LoadSceneGroup(nextIndex);
        }

        public async Task LoadSceneGroup(int index){
            if (index < 0 || index >= _sceneGroups.Length){
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }
            
            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => _targetProgress = Mathf.Max(target, _targetProgress);

            _isLoading = true;
            
            _currentIndex = index;
            _currentSceneGroup = _sceneGroups[index];
            await manager.LoadScenes(_sceneGroups[index], progress);
            //loaded here
            _isLoading = false;
            
            
        }

        private void OnSceneGroupLoaded(){
            Debug.Log("Scene group loaded");
            //if gameplay manager, there is a level, start it
            //if(GameplayManager.Instance) GameplayManager.Instance.InitLevel();
        }
        
        private void OnDestroy()
        {
            manager.OnSceneGroupLoaded -= OnSceneGroupLoaded;
        }
    }
    
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;
        private const float ratio = 1f;
        
        public void Report(float value)
        {
            Progressed?.Invoke(value/ratio);
        }
    }
}