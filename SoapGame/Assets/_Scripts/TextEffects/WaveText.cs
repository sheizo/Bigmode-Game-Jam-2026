
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

[CreateAssetMenu(fileName = "WaveText", menuName = "ScriptableObjects/TextEffects/WaveText")]
public class WaveText : TextEffect
{
    public float amplitude = 0.1f;
    public bool byWord = false, everything = false;
    public bool wiggly = false;
    public float speed, height;

    private Mesh mesh;
    private Vector3[] vertices;
    private int wordCount = 0;



    public override void ApplyEffect(TMP_Text textMesh, int textStart, int textEnd)
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        wordCount = 0;

        
        for (int i = textStart; i < textEnd; i++)
        {
            TMP_CharacterInfo charInfo = textMesh.textInfo.characterInfo[i];
            int index = charInfo.vertexIndex;


            if (charInfo.character == ' ')
            {
                wordCount++;
            }

            if (!charInfo.isVisible)
            {
                continue;
            }


            int characterModifier = i;
            if (byWord)
                characterModifier = wordCount;
            if (everything)
                characterModifier = 0;

            float sinFunc = Mathf.Sin((-Time.time * speed + characterModifier) * amplitude) * height;

            Vector3 offset = new Vector2(0, sinFunc);

            if (sinFunc > 0)
            {
                for (int j = 0; j < 4; j++)
                {
                    vertices[index + j] += offset * (wiggly ? (j + 1) : 1);
                }
            }
            
        }

        if (applyColor) ApplyColor(textMesh, textStart, textEnd);

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

}
