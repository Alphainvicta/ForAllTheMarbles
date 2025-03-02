using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Managers;

public class DefaultController : BaseInputAction
{
    private bool pivotIsSet = false;
    private bool canMove = true;
    private Vector2 centerPivot;
    private readonly float threshold = 50f;
    private readonly float jumpHeight = 9f;
    private bool isTransitioning = false;
    private bool jumpAvailable = false;
    private int marblePosition = 0;
    private Rigidbody playerRigidbody;
    CameraManager cameraManager;

    private void Awake()
    {
        Initialize();
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
        cameraManager = FindFirstObjectByType<CameraManager>();
    }

    private void OnEnable()
    {
        Enable();
    }

    private void OnDisable()
    {
        Disable();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (isPressed == 0)
        {
            pivotIsSet = false;
            canMove = true;
            return;
        }
        else
        {
            MoveAction();
            PressedAction();
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    public override void MoveAction()
    {
        if (isTransitioning || !canMove) return;

        if (!pivotIsSet)
        {
            centerPivot = moveValue;
            pivotIsSet = true;
        }

        Vector2 movementDirection = moveValue - centerPivot;
        float angle = Vector2.SignedAngle(Vector2.right, movementDirection);

        if (Vector2.Distance(centerPivot, moveValue) > threshold)
        {
            if (angle >= -45f && angle < 45f) // Right
            {
                if (marblePosition < 1)
                {
                    marblePosition++;
                    StartCoroutine(MarbleTransition(new Vector3(transform.position.x + 2f, 0f, 0f), 0.1f));
                    StartCoroutine(cameraManager.CameraTransition(
                        CameraManager.mainCameraInstance.transform.position + new Vector3(0.5f, 0f, 0f),
                        CameraManager.mainCameraInstance.transform.rotation,
                        0.1f));
                    canMove = false;
                }
            }
            else if (angle >= 45f && angle < 135f) // Up
            {
                if (jumpAvailable)
                {
                    playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                    canMove = false;
                }
            }
            else if (angle >= 135f || angle < -135f) // Left
            {
                if (marblePosition > -1)
                {
                    marblePosition--;
                    StartCoroutine(MarbleTransition(new Vector3(transform.position.x - 2f, 0f, 0f), 0.1f));
                    StartCoroutine(cameraManager.CameraTransition(
                        CameraManager.mainCameraInstance.transform.position + new Vector3(-0.5f, 0f, 0f),
                        CameraManager.mainCameraInstance.transform.rotation,
                        0.1f));
                    canMove = false;
                }
            }
            else if (angle >= -135f && angle < -45f) // Down
            {
                playerRigidbody.linearVelocity = Vector3.zero;
                playerRigidbody.AddForce(Vector3.down * jumpHeight * 2, ForceMode.Impulse);
                canMove = false;
            }
        }
    }

    public override void PressedAction()
    {
        return;
    }

    private IEnumerator MarbleTransition(Vector3 goalPosition, float duration)
    {
        isTransitioning = true;

        Vector3 currentPosition = gameObject.transform.position;

        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float newX = Mathf.Lerp(currentPosition.x, goalPosition.x, t);
            gameObject.transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = new Vector3(goalPosition.x, currentPosition.y, currentPosition.z);

        isTransitioning = false;
        yield return null;
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
}