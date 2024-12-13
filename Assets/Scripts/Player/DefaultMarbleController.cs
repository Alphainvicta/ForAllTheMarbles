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
    private float marbleSpeed;
    private float marbleJumpForce;
    private float marbleSmoothSpeed;
    private PlayerInput playerInput;
    private InputAction playerMove;
    private InputAction playerJump;
    private InputAction playerPressed;
    private float previousY;
    private float targetMarbleSpeed;
    private float threshold;

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
        marbleSmoothSpeed = playerMarble.marbleSmoothSpeed;
        marbleSpeed = 1f;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        previousY = transform.position.y;
        threshold = GameObject.Find("GameManager").GetComponent<PlayerManager>().touchThreshold;
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
        Vector3 forwardForce = Vector3.forward * marbleSpeed;
        rigidbody.AddForce(forwardForce, ForceMode.Acceleration);
    }

    private void SmoothMovement()
    {
        Vector3 smoothedPosition = Vector3.Lerp(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(rigidbody.position.x, 0f, rigidbody.position.z),
            marbleSmoothSpeed
        );

        rigidbody.position = new Vector3(smoothedPosition.x, transform.position.y, smoothedPosition.z);
    }

    private void CameraFollow()
    {
        cameraRig.transform.position = new Vector3(transform.position.x, cameraRig.transform.position.y, transform.position.z);
    }

    private void SetCameraVertical()
    {
        cameraRig.transform.position = new Vector3(cameraRig.transform.position.x, transform.position.y, cameraRig.transform.position.z);
    }

    private void PlayerMovement(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>();
        if (!pivotIsSet)
        {
            centerPivot = moveValue;
            pivotIsSet = true;
        }

        if (Mathf.Abs(Mathf.Abs(centerPivot.x) - Mathf.Abs(moveValue.x)) > threshold)
        {
            if (moveValue.x > centerPivot.x)
            {
                rigidbody.AddForce(Vector3.right * 3f, ForceMode.Acceleration);
            }
            else if (moveValue.x < centerPivot.x)
            {
                rigidbody.AddForce(Vector3.left * 3f, ForceMode.Acceleration);
            }
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
        Debug.DrawRay(transform.position, Vector3.down * 0.6f, Color.red);

        if (Physics.Raycast(ray, out hit, 0.6f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                jumpAvailable = true;
                if (Mathf.Round(previousY) != Mathf.Round(transform.position.y))
                {
                    SetCameraVertical();
                    previousY = Mathf.Round(transform.position.y);
                }
            }
        }
        else
        {
            jumpAvailable = false;
        }
    }
}