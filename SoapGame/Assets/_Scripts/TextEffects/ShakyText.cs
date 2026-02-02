using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "ShakyText", menuName = "ScriptableObjects/TextEffects/ShakyText")]
public class ShakyText : TextEffect
{
    public bool byVertices = false;
    public float magnitude = 1;
    [Range(0,1)] public float shakeInterval = 0.05f;

    private Mesh mesh;
    private Vector3[] vertices;

    private float timer;
    public override void ApplyEffect(TMP_Text textMesh, int textStart, int textEnd)
    {
        timer += Time.deltaTime;
        if (timer < shakeInterval) return;
        timer = 0;

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

            Vector3 offset = new Vector2(Random.Range(-1f, 1f) * magnitude, Random.Range(-1f, 1f) * magnitude);

            //4 vertices of a character
            for (int j = 0; j < 4; j++)
            {
                vertices[index+j] += offset;
            }



        }

        if(applyColor) ApplyColor(textMesh, textStart, textEnd);

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

}