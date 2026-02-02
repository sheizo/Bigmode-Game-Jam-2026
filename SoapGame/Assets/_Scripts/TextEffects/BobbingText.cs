
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "BobbingText", menuName = "ScriptableObjects/TextEffects/BobbingText")]
public class BobbingText : TextEffect
{
    public bool cos, sin = true;
    public float speed, height;


    private Mesh mesh;
    private Vector3[] vertices;


    public override void ApplyEffect(TMP_Text textMesh, int textStart, int textEnd)
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;


        for (int i = textStart; i < textEnd; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
            int index = c.vertexIndex;

            // Skip invisible characters
            if (!c.isVisible)
            {
                continue;
            }

            Vector3 offset = new Vector2((cos ? Mathf.Cos(Time.time * speed ) * height : 0), (sin ? Mathf.Sin(Time.time * speed ) * height : 0));

            //4 vertices of a character
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;



        }

        if(applyColor) ApplyColor(textMesh, textStart, textEnd);

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

}
