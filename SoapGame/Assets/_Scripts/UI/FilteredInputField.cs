using TMPro;
using UnityEngine;

public class FilteredInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();

        // Subscribe to the value changed event
        inputField.onValueChanged.AddListener(HandleInputChange);
    }

    private void HandleInputChange(string text)
    {
        // Filter out non-alphanumeric characters
        string filteredText = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z0-9]", "");
        
        // Update the input field with the filtered text
        if (filteredText != text)
        {
            inputField.text = filteredText;
        }

        //Limit the length of the input to 12 characters
        if (filteredText.Length > 12)
        {
            inputField.text = filteredText.Substring(0, 12);
        }
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(HandleInputChange);
    }
}