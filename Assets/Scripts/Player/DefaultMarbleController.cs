using UnityEngine;
using UnityEngine.InputSystem;

public class DefaultMarbleController : MonoBehaviour
{
    private Rigidbody rigidbody;
    private GameObject cameraRig;
    private Vector2 centerPivot;
    private Vector2 moveValue;
    private float isPressed;
    private float jumpValue;
    private bool jumpAvailable = true;
    public float marbleSpeed = 1f;
    public float jumpForce = 10f;

    private PlayerInput playerInput;
    public InputAction playerMove;
    public InputAction playerJump;
    public InputAction playerPressed;

    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerMove = playerInput.actions["Move"];
        playerJump = playerInput.actions["Jump"];
        playerPressed = playerInput.actions["IsPressed"];
    }

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        cameraRig = GameObject.Find("CameraRig");
    }

    private void OnEnable()
    {
        playerMove.performed += PlayerMovement;
        playerJump.performed += PlayerJump;
        playerJump.canceled += PlayerJump;
        playerPressed.performed += PlayerPressed;
        playerPressed.canceled += PlayerPressed;
    }
    private void OnDisable()
    {
        playerMove.performed -= PlayerMovement;
        playerJump.performed -= PlayerJump;
        playerJump.canceled -= PlayerJump;
        playerPressed.performed -= PlayerPressed;
        playerPressed.canceled -= PlayerPressed;
    }

    private void FixedUpdate()
    {
        MoveForward();
        CameraFollow();
    }

    private void MoveForward()
    {
        Vector3 forwardVelocity = new Vector3(0f, 0f, marbleSpeed);
        rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, rigidbody.linearVelocity.y, forwardVelocity.z);
    }

    private void CameraFollow()
    {
        cameraRig.transform.position = new Vector3(transform.position.x, cameraRig.transform.position.y, transform.position.z);
    }

    private void PlayerMovement(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>();
        if (isPressed > 0)
        {
            centerPivot = moveValue;
        }

        if (moveValue.x > centerPivot.x)
        {
            Vector3 rightVelocity = new Vector3(marbleSpeed, 0f, 0f);
            rigidbody.AddForce(rightVelocity, ForceMode.Force);
        }
        else if (moveValue.x < centerPivot.x)
        {
            Vector3 leftVelocity = new Vector3(-marbleSpeed, 0f, 0f);
            rigidbody.AddForce(leftVelocity, ForceMode.Force);
        }
    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        jumpValue = context.ReadValue<float>();
        if (jumpValue > 0 && jumpAvailable)
        {
            Vector3 upForce = new Vector3(0f, jumpForce, 0f);
            rigidbody.AddForce(upForce, ForceMode.VelocityChange);
            jumpAvailable = false;
        }
    }

    private void PlayerPressed(InputAction.CallbackContext context)
    {
        isPressed = context.ReadValue<float>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpAvailable = true;
        }
    }
}