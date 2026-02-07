using UnityEngine;

public class PrintLevelBounds : MonoBehaviour
{
    [ContextMenu("Print Level Bounds")]
    public void PrintBounds()
    {
        Bounds bounds = new Bounds();
        //Store all _levelPrefab children components renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        //define level maximum bounds
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (renderers[i] == null) continue;
            bounds.Encapsulate(renderers[i].bounds);
        }

        Debug.Log($"Level Bounds: {bounds.size}");
    }
}
