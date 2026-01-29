using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };
        
        SceneGroup ActiveSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false){
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            // unload scenes then load them 1 by 1
            await UnloadScenes();
            
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++){
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }
            
            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;
            var operationsGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (int i = 0; i < totalScenesToLoad; i++){
                var sceneData = group.Scenes[i];
                // if already loaded skip
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name)) continue; 
                
                var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                operationsGroup.Operations.Add(operation);

                if (operation != null) operation.completed += _ => OnSceneLoaded.Invoke(sceneData.Name);

                OnSceneLoaded.Invoke(sceneData.Name);
            }

            while (!operationsGroup.IsDone){
                progress?.Report(operationsGroup.Progress);
                await Task.Delay(100);
            }
            
            Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
            if (activeScene.IsValid()){
                SceneManager.SetActiveScene(activeScene);
            }
            
            OnSceneGroupLoaded.Invoke();
        }

        public async Task UnloadScenes(){
            //DG.Tweening.DOTween.KillAll();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Bootstrapper"));
            
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;
            
            int sceneCount = SceneManager.sceneCount;

            for (int i = sceneCount - 1; i > 0; i--){
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;
                
                var sceneName = sceneAt.name;
                //dont unload active
                //if(sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue; 
                if(sceneName == "Bootstrapper") continue; 
                
                scenes.Add(sceneName);
            }
            
            var operationGroup = new AsyncOperationGroup(scenes.Count);
            foreach (var scene in scenes){
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;
                
                operationGroup.Operations.Add(operation);
                
                OnSceneUnloaded.Invoke(scene);
            }
            
            // wait until all AsyncOperation in the group are done
            while (!operationGroup.IsDone){
                await Task.Delay(100);
            }
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;
        
        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone); //checks if all operations are done

        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}