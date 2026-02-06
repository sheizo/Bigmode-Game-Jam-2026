using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeTexture : MonoBehaviour
{
    [SerializeField] private string _texturePropertyId = "_MainTex";
    [SerializeField] private Texture2D[] _textures;

    private void Awake(){
        if (_textures == null || !TryGetComponent(out Renderer r)) return;
            r.material.SetTexture(_texturePropertyId, _textures[Random.Range(0,_textures.Length)]);
        
    }
}
