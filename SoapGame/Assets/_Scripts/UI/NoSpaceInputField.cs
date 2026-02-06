using TMPro;
using UnityEngine;

public class NoSpacesInputField : MonoBehaviour
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
        // Remove any spaces from the input
        if (text.Contains(" "))
        {
            string newText = text.Replace(" ", "");
            inputField.text = newText;

            // Optional: move caret to the end
            inputField.caretPosition = newText.Length;
        }

        // trim the name to 10 characters
        if (text.Length > 10)
        {
            string newText = text.Substring(0, 10);
            inputField.text = newText;
            inputField.caretPosition = newText.Length;
        }
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(HandleInputChange);
    }
}