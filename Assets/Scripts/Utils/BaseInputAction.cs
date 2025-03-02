using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseInputAction : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction playerPressed;
    private InputAction playerMove;
    protected Vector2 moveValue;
    protected float isPressed;

    protected void Initialize()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerMove = playerInput.actions["Move"];
        playerPressed = playerInput.actions["IsPressed"];
    }

    protected void Enable()
    {
        playerMove.performed += PlayerMovementInput;
        playerPressed.performed += PlayerPressedInput;
        playerPressed.canceled += PlayerPressedInput;
    }

    protected void Disable()
    {
        playerMove.performed -= PlayerMovementInput;
        playerPressed.performed -= PlayerPressedInput;
        playerPressed.canceled -= PlayerPressedInput;
    }

    private void PlayerMovementInput(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>();
    }

    private void PlayerPressedInput(InputAction.CallbackContext context)
    {
        isPressed = context.ReadValue<float>();
    }

    public abstract void MoveAction();
    public abstract void PressedAction();
}