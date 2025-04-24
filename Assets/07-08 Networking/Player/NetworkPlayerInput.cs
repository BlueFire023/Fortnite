using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerInput : MonoBehaviour
{
    public bool canMove = true;
    public bool isSprinting;

    [Header("Move")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 9.81f;

    private CharacterController characterController;
    private Vector3 moveVelocity;
    private Vector2 moveInput;

    [Header("Camera/Rotation")]
    public Camera fpsCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector2 lookInput;
    private float rotationX;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Move
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isSprinting ? runSpeed : walkSpeed) * moveInput.y : 0;
        float curSpeedY = canMove ? (isSprinting ? runSpeed : walkSpeed) * moveInput.x : 0;
        float movementVelocityY = moveVelocity.y;
        moveVelocity = (forward * curSpeedX) + (right * curSpeedY);

        // Jump
        moveVelocity.y = movementVelocityY;
        if (!characterController.isGrounded)
            moveVelocity.y -= gravity * Time.deltaTime;

        // Apply Movement
        characterController.Move(moveVelocity * Time.deltaTime);

        // Rotation
        if (canMove)
        {
            rotationX += -lookInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            fpsCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }
    }

    public void HandleMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void HandleSprintInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            isSprinting = true;
        else if (ctx.canceled)
            isSprinting = false;
    }

    public void HandleLookInput(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canMove && characterController.isGrounded)
            moveVelocity.y = jumpPower;
    }
}
