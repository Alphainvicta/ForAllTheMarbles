using UnityEngine;
using UnityEngine.InputSystem;
using Managers;
using UnityEngine.EventSystems;

public class DefaultMarbleController : MonoBehaviour
{
    private Rigidbody rigidBody;
    private GameObject cameraRig;
    private Vector2 centerPivot;
    private bool pivotIsSet = false;
    private Vector2 moveValue;
    private float isPressed;
    private float jumpValue;
    private bool jumpIsTrigger = false;
    private bool jumpAvailable = true;
    public float marbleSpeed;
    private float marbleJumpForce;
    private float marbleSmoothDuration = 0f;
    private float cameraSmoothDuration = 0f;
    private PlayerInput playerInput;
    private InputAction playerMove;
    private InputAction playerJump;
    private InputAction playerPressed;
    private float previousY;
    private float targetMarbleSpeed;
    private float threshold;
    private float targetCameraY;

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
        PlayerMarble playerMarble = GameObject.Find("GameManager").GetComponent<PlayerManager>().playerMarble;
        targetMarbleSpeed = playerMarble.marbleSpeed;
        marbleJumpForce = playerMarble.marbleJumpForce;
        marbleSpeed = 1f;
        rigidBody = gameObject.GetComponent<Rigidbody>();
        previousY = Mathf.Round(cameraRig.transform.position.y);
        targetCameraY = Mathf.Round(transform.position.y);
        threshold = GameObject.Find("GameManager").GetComponent<PlayerManager>().touchThreshold;
    }

    private void OnEnable()
    {
        playerMove.performed += PlayerMovementInput;
        playerJump.performed += PlayerJumpInput;
        playerJump.canceled += PlayerJumpInput;
        playerPressed.performed += PlayerPressedInput;
        playerPressed.canceled += PlayerPressedInput;
    }

    private void OnDisable()
    {
        playerMove.performed -= PlayerMovementInput;
        playerJump.performed -= PlayerJumpInput;
        playerJump.canceled -= PlayerJumpInput;
        playerPressed.performed -= PlayerPressedInput;
        playerPressed.canceled -= PlayerPressedInput;
    }

    private void PlayerMovementInput(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>();
    }

    private void PlayerJumpInput(InputAction.CallbackContext context)
    {
        jumpValue = context.ReadValue<float>();
        if (jumpValue > 0)
            jumpIsTrigger = true;
    }

    private void PlayerPressedInput(InputAction.CallbackContext context)
    {
        isPressed = context.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        UpdateMarbleSpeed();
        MoveForward();
        CameraFollow();
        GroundCheck();

        PlayerMovement();
    }

    private void Update()
    {
        PlayerJump();
        PlayerPressed();
    }

    private void UpdateMarbleSpeed()
    {
        marbleSmoothDuration += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(marbleSmoothDuration / 3);
        marbleSpeed = Mathf.Lerp(1f, targetMarbleSpeed, t);
    }

    private void MoveForward()
    {
        Vector3 forwardForce = Vector3.forward * marbleSpeed;
        rigidBody.AddForce(forwardForce, ForceMode.Force);

        Vector3 currentVelocity = rigidBody.linearVelocity;
        // float clampedX = Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed);
        float clampedZ = Mathf.Clamp(currentVelocity.z, marbleSpeed, targetMarbleSpeed);
        rigidBody.linearVelocity = new Vector3(currentVelocity.x, currentVelocity.y, clampedZ);
    }

    private void CameraFollow()
    {
        float t = 1f;
        if (cameraRig.transform.position.y != targetCameraY)
        {
            cameraSmoothDuration += Time.fixedDeltaTime;
            t = Mathf.Clamp01(cameraSmoothDuration / 1f);
        }
        if (transform.position.y > 0)
            cameraRig.transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(previousY, targetCameraY, t),
                transform.position.z
            );
    }

    private void PlayerMovement()
    {
        if (isPressed == 1 && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!pivotIsSet)
            {
                centerPivot = moveValue;
                pivotIsSet = true;
            }

            if (Mathf.Abs(Mathf.Abs(centerPivot.x) - Mathf.Abs(moveValue.x)) > threshold)
            {
                if (moveValue.x > centerPivot.x)
                {
                    rigidBody.AddForce(Vector3.right * 0.3f, ForceMode.Impulse);
                }
                else if (moveValue.x < centerPivot.x)
                {
                    rigidBody.AddForce(Vector3.left * 0.3f, ForceMode.Impulse);
                }
            }
        }
    }

    private void PlayerJump()
    {
        if (jumpIsTrigger && jumpAvailable && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 upForce = new(0f, marbleJumpForce, 0f);
            rigidBody.AddForce(upForce, ForceMode.VelocityChange);
        }
        jumpIsTrigger = false;
    }

    private void PlayerPressed()
    {
        if (isPressed == 0)
        {
            pivotIsSet = false;
        }
    }

    private void GroundCheck()
    {
        float raySpacing = 0.5f;
        Vector3[] rayOffsets = new Vector3[]
        {
        new(-raySpacing, 0f, 0f),
        new(raySpacing, 0f,  0f),
        new( 0f, 0f, -raySpacing),
        new( 0f, 0f,  raySpacing),
        Vector3.zero
        };

        jumpAvailable = false;

        foreach (var offset in rayOffsets)
        {
            Vector3 rayOrigin = transform.position + offset;
            Ray ray = new Ray(rayOrigin, Vector3.down);

            Debug.DrawRay(rayOrigin, Vector3.down * 0.6f, Color.red);

            if (Physics.Raycast(ray, out RaycastHit hit, 0.6f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    jumpAvailable = true;
                    if (Mathf.Round(targetCameraY) != Mathf.Round(transform.position.y))
                    {
                        targetCameraY = Mathf.Round(transform.position.y);
                        previousY = Mathf.Round(cameraRig.transform.position.y);
                        cameraSmoothDuration = 0f;
                    }
                    break;
                }
            }
        }
    }
}
