using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;

namespace SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "New scene group";
        public List<SceneData> Scenes;
        
        public string FindSceneNameByType(SceneType sceneGroup) => Scenes.FirstOrDefault(scene => scene.sceneGroup == sceneGroup)?.Reference.Name;
    }
    
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public string Name => Reference.Name;
        public SceneType sceneGroup;
        
    }
    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling}
}