using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.EventSystems;

public static class Helpers
{
    //Cache first reference to Camera.Main
    /*private static Camera _camera;
    public static Camera Camera{
        get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }*/
    //---Not expensive anymore

    //Cache WaitForSeconds //called by yield return Helpers.GetWait(x);
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time)
    {
        //if already has wait return said wait
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait; 
        
        //if not create and add to dictionary
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    //Destroy all children from transform
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }
    public static void ReleaseChildren<T>(this Transform t, ObjectPool<T> pool) where T : Component
    {
        foreach (Transform child in t)
        {
            if(t.TryGetComponent(out T poolItem))
                pool.Release(poolItem);
        }
    }

    public static Color RandomColor(this Color color)
    {
        return new Color(Random.Range(0, 1f),
                         Random.Range(0, 1f),
                         Random.Range(0, 1f));
    }

    //UI

    //Is over UI
    /*private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    public static bool IsOverUi()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }*/
    //https://docs.unity3d.com/2018.1/Documentation/ScriptReference/EventSystems.EventSystem.IsPointerOverGameObject.html


    //Gets the world position of an ui element
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(element, element.position, Camera.main, out var result);
        return result;
    }
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element, Camera camera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(element, element.position, camera, out var result);
        return result;
    }
    
    public static Vector2 WorldToCanvasPoint(this RectTransform This, Vector3 WorldPosition, Camera WorldCamera, out bool Success, Camera CanvasCamera = null)
    {
        Success = RectTransformUtility.ScreenPointToLocalPointInRectangle(This, WorldCamera.WorldToScreenPoint(WorldPosition), CanvasCamera, out var CanvasPoint);

        return CanvasPoint;
    }

}
