using UnityEngine;
using UnityEngine.InputSystem;
using Managers;

public class DefaultMarbleController : MonoBehaviour
{
    private Rigidbody rigidbody;
    private GameObject cameraRig;
    private Vector2 centerPivot;
    private bool pivotIsSet = false;
    private Vector2 moveValue;
    private float isPressed;
    private float jumpValue;
    public bool jumpAvailable = true;
    public float marbleSpeed;
    public float marbleJumpForce;
    private Vector3 targetPosition;
    private float marbleSmoothSpeed;
    private PlayerInput playerInput;
    public InputAction playerMove;
    public InputAction playerJump;
    public InputAction playerPressed;

    private float targetMarbleSpeed;

    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerMove = playerInput.actions["Move"];
        playerJump = playerInput.actions["Jump"];
        playerPressed = playerInput.actions["IsPressed"];
    }

    private void Start()
    {
        cameraRig = GameObject.Find("CameraRig");
        targetMarbleSpeed = GameObject.Find("GameManager").GetComponent<PlayerManager>().playerMarble.marbleSpeed;
        marbleJumpForce = GameObject.Find("GameManager").GetComponent<PlayerManager>().playerMarble.marbleJumpForce;
        marbleSmoothSpeed = GameObject.Find("GameManager").GetComponent<PlayerManager>().playerMarble.marbleSmoothSpeed;
        marbleSpeed = 1f;
        targetPosition = transform.position;
        rigidbody = gameObject.GetComponent<Rigidbody>();
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
        UpdateMarbleSpeed();
        MoveForward();
        SmoothMovement();
        CameraFollow();
        GroundCheck();
    }

    private void UpdateMarbleSpeed()
    {
        marbleSpeed = Mathf.Lerp(marbleSpeed, targetMarbleSpeed, Time.fixedDeltaTime * 0.5f);
    }

    private void MoveForward()
    {
        targetPosition.z += marbleSpeed * Time.fixedDeltaTime;
    }

    private void SmoothMovement()
    {
        Vector3 smoothedPosition = Vector3.Lerp(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(targetPosition.x, 0, targetPosition.z),
            marbleSmoothSpeed
        );

        transform.position = new Vector3(smoothedPosition.x, transform.position.y, smoothedPosition.z);
    }

    private void CameraFollow()
    {
        cameraRig.transform.position = new Vector3(transform.position.x, cameraRig.transform.position.y, transform.position.z);
    }

    private void PlayerMovement(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>();
        if (!pivotIsSet)
        {
            centerPivot = moveValue;
            pivotIsSet = true;
        }

        if (moveValue.x > centerPivot.x)
        {
            targetPosition.x += 3 * Time.fixedDeltaTime;
        }
        else if (moveValue.x < centerPivot.x)
        {
            targetPosition.x -= 3 * Time.fixedDeltaTime;
        }
    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        jumpValue = context.ReadValue<float>();
        if (jumpValue > 0 && jumpAvailable)
        {
            Vector3 upForce = new Vector3(0f, marbleJumpForce, 0f);
            rigidbody.AddForce(upForce, ForceMode.VelocityChange);
        }
    }

    private void PlayerPressed(InputAction.CallbackContext context)
    {
        isPressed = context.ReadValue<float>();
        if (isPressed == 0)
        {
            pivotIsSet = false;
        }
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down * 0.5f, Color.red);

        if (Physics.Raycast(ray, out hit, 0.5f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                jumpAvailable = true;
            }
        }
        else
        {
            jumpAvailable = false;
        }
    }
}