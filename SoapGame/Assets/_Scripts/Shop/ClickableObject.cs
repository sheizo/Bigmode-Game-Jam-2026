using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickableObject : MonoBehaviour
{

    [SerializeField] private InputActionAsset _inputActionAsset;  // The object to be highlighted and clicked.
    [SerializeField] private Outline _outline;
    [SerializeField] private GameObject _objectToHighlight;  // The object to be highlighted and clicked.


    private InputAction _clickAction;
    private bool _isMouseHovered = false;


    void Awake()
    {
        _clickAction = _inputActionAsset.FindActionMap("Player").FindAction("Click");
        _clickAction.Enable();

        _outline.enabled = false;
    }

    private void Update()
    {
        // Perform raycast to detect mouse over the object.
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        _isMouseHovered = false;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject == _objectToHighlight)
            {
                _isMouseHovered = true;
                if (_clickAction.triggered)
                {
                    OnObjectClicked();
                }
            }
        }

        _outline.enabled = _isMouseHovered;
    }

    // Custom method to handle the click event
    private void OnObjectClicked()
    {
        // Perform the desired action when the object is clicked
        Debug.Log("Object clicked: " + _objectToHighlight.name);
    }
}
