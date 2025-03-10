using UnityEngine;
using UnityEngine.InputSystem;
using Managers;
using System.Collections;

public abstract class BaseInputAction : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction playerPressed;
    private InputAction playerMove;
    protected Vector2 moveValue;
    protected bool jumpAvailable;
    protected bool hittedObstacle;
    protected float isPressed;
    protected bool isTransitioning;
    protected Rigidbody playerRigidbody;
    public int marbleXPosition = 0;

    protected void Initialize()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerMove = playerInput.actions["Move"];
        playerPressed = playerInput.actions["IsPressed"];
        isTransitioning = false;
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
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

    private void FixedUpdate()
    {
        GroundCheck();
        if (!PlayerManager.isObstacleHitted)
            ObstacleCheck();
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

    private void GroundCheck()
    {
        float raySpacing = 0.5f;
        Vector3[] rayOffsets = new Vector3[]
        {
        Vector3.zero,
        new(-raySpacing, 0f, 0f),
        new(raySpacing, 0f,  0f),
        new( 0f, 0f, -raySpacing),
        new( 0f, 0f,  raySpacing),
        };

        jumpAvailable = false;

        foreach (var offset in rayOffsets)
        {
            Vector3 rayOrigin = transform.position + offset;
            Ray ray = new(rayOrigin, Vector3.down);

            Debug.DrawRay(rayOrigin, Vector3.down * 0.6f, Color.red);

            if (Physics.Raycast(ray, out RaycastHit hit, 0.6f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    jumpAvailable = true;
                    break;
                }
            }
        }
    }

    private void ObstacleCheck()
    {
        float raySpacing = 0.5f;
        Vector3[] rayOffsets = new Vector3[]
        {
        Vector3.zero,
        new(-raySpacing, 0f, 0f),
        new(raySpacing, 0f,  0f),
        new( 0f, -raySpacing, 0f),
        new( 0f, raySpacing, 0f)
        };

        foreach (var offset in rayOffsets)
        {
            Vector3 rayOrigin = transform.position + offset;
            Ray ray = new(rayOrigin, Vector3.forward);

            Debug.DrawRay(rayOrigin, Vector3.forward, Color.blue);

            if (Physics.Raycast(ray, out RaycastHit hit, 1f))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    GameManager.EndGame();
                    break;
                }
            }
        }
    }

    public IEnumerator MarbleXTransition(float goalPosition, float duration)
    {
        isTransitioning = true;

        float timeElapsed = 0f;
        Vector3 currentPosition = transform.position;
        while (timeElapsed < duration)
        {
            yield return new WaitUntil(() => !GameManager.isPaused);
            float t = timeElapsed / duration;
            float newX = Mathf.Lerp(currentPosition.x, currentPosition.x + goalPosition, t);
            playerRigidbody.MovePosition(new Vector3(newX, transform.position.y, transform.position.z));
            timeElapsed += Time.deltaTime;
        }

        playerRigidbody.MovePosition(new(currentPosition.x + goalPosition, transform.position.y, transform.position.z));

        isTransitioning = false;
        yield return null;
    }
}