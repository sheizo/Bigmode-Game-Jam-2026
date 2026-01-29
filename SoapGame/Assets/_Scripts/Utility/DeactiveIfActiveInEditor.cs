using UnityEngine;

public class DeactiveIfActiveInEditor : MonoBehaviour
{
    private bool _wasActiveInEditor;
    
    void Awake(){
        if(_wasActiveInEditor)
            gameObject.SetActive(false);
    }
    
#if UNITY_EDITOR
    // This ensures the active state is stored when the scene is saved or scripts are recompiled
    private void OnValidate()
    {
        _wasActiveInEditor = gameObject.activeSelf;
    }
#endif
}
