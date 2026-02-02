
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "WavyText", menuName = "ScriptableObjects/TextEffects/WavyText")]
public class WavyText : TextEffect
{
    public float amplitude = 1;
    public bool cos, sin = true;
    public bool byWord = false;
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
            if(charInfo.character == ' ')
            {
                wordCount++;
            }

            // Skip invisible characters
            if (!charInfo.isVisible)
            {
                continue;
            }

            float cosFunc = Mathf.Cos((-Time.time * speed + (byWord ? wordCount : i))*amplitude) * height;
            float sinFunc = Mathf.Sin((-Time.time * speed + (byWord ? wordCount : i))*amplitude) * height;

            Vector3 offset = new Vector2((cos ? cosFunc : 0), (sin ? sinFunc : 0));

            for (int j = 0; j < 4; j++)
            {
                vertices[index+j] += offset * (wiggly ? (j+1) : 1);
            }

        }

        if(applyColor) ApplyColor(textMesh, textStart, textEnd);

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    
}
