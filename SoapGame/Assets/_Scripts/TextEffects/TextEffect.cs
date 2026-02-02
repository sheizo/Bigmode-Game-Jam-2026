using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

public abstract class TextEffect : ScriptableObject
{
    public bool applyColor = true;
    public Color32 topLeft = Color.white, topRight = Color.white, bottomLeft = Color.white, bottomRight = Color.white;

    public virtual void ApplyEffect(TMP_Text textMesh, int textStart, int textEnd) {}
    public virtual void ApplyEffect(TMP_Text textMesh)
    {
        ApplyEffect(textMesh, 0, textMesh.text.Length);
    }

    public virtual void ApplyColor(TMP_Text textMesh, int textStart, int textEnd) {
        for (int i = textStart; i < textEnd; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
            int index = c.vertexIndex;

            // Skip invisible characters
            if (!c.isVisible)
            {
                continue;
            }

            Color32[] colorVertices = textMesh.textInfo.meshInfo[textMesh.textInfo.characterInfo[i].materialReferenceIndex].colors32;

            colorVertices[index] = bottomLeft;
            colorVertices[index + 1] = topLeft;
            colorVertices[index + 2] = topRight;
            colorVertices[index + 3] = bottomRight;
        }
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextEffect))]
[CanEditMultipleObjects]
class TextEffectEditor : Editor
{
    SerializedProperty applyColor;
    SerializedProperty topLeft, topRight, bottomLeft, bottomRight;

    void OnEnable()
    {


        // Fetch the objects from the GameObject script to display in the inspector
        applyColor = serializedObject.FindProperty("applyColor");
        
        topLeft = serializedObject.FindProperty("topLeft");
        topRight = serializedObject.FindProperty("topRight");
        bottomLeft = serializedObject.FindProperty("bottomLeft");
        bottomRight = serializedObject.FindProperty("bottomRight");

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var textEffect = (TextEffect)target;
        if (textEffect == null) return;

        // Update the serialized object
        serializedObject.Update();

        // Display the boolean property
        EditorGUILayout.PropertyField(applyColor);


        if (applyColor.boolValue)
        {
            EditorGUILayout.PropertyField(topLeft);
            EditorGUILayout.PropertyField(topRight);
            EditorGUILayout.PropertyField(bottomLeft);
            EditorGUILayout.PropertyField(bottomRight);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
