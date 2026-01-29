using UnityEngine;
using UnityEngine.InputSystem;

public class SoapController : MonoBehaviour
{
    private InputAction _moveAction;

    public float moveSpeed = 5f;      // Speed at which the soap moves
    public float rotateSpeed = 2f;    // Speed at which the soap rotates when slipping
    public float lowFriction = 0.1f;  // Friction value to simulate soap-like slipperiness
    public Camera playerCamera;       // Reference to the player's camera

    private Rigidbody rb;

    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Set very low friction values to simulate slipperiness
        var friction = new PhysicsMaterial();
        friction.dynamicFriction = lowFriction;
        friction.staticFriction = lowFriction;
        rb.GetComponent<Collider>().material = friction;
    }

    void Update()
    {
        HandleMoveInput();
    }

    void HandleMoveInput()
    {

        Vector2 moveValue = _moveAction.ReadValue<Vector2>();

        // Get the input for forward movement (W key)
        float moveInput = moveValue.y;

        // Calculate movement direction relative to the camera
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;  // Keep the movement on the XZ plane

        // Move the soap in the direction of the camera's forward
        rb.AddForce(forward.normalized * moveInput * moveSpeed, ForceMode.VelocityChange);



        // Get input for rotation (A and D keys)
        float rotationInput = moveValue.x;

        // Apply torque to simulate slipping
        rb.AddTorque(Vector3.up * rotationInput * rotateSpeed, ForceMode.VelocityChange);
    }
}
