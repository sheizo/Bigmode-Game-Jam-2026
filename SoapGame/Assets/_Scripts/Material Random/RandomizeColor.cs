using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    [SerializeField] private string _colorPropertyId = "_Color";

    [SerializeField] private Vector2 _saturation, _value;
    private void Awake(){
        SetRandomColor();
    }

    private void SetRandomColor(){
        if (!TryGetComponent(out Renderer r)) return;
        r.material.SetColor(_colorPropertyId, Random.ColorHSV(0f, 1f, _saturation.x, _saturation.y, _value.x, _value.y));        

    }

    [ContextMenu("Random Color")]
    public void RandomColor(){
        SetRandomColor();
    }
}
