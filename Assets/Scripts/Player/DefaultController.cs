using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Managers;
using System;

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
    private int currentPosition;
    private float Xposition;
    private Rigidbody playerRigidbody;
    public static Coroutine playerTransitionCoroutine;
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
        GameManager.MenuGame += StopCoroutines;
        GameManager.GameStart += SetMarblePosition;
        GameManager.GameEnd += StopCoroutines;
    }

    private void OnDisable()
    {
        Disable();
        GameManager.GameStart -= SetMarblePosition;
        GameManager.MenuGame -= StopCoroutines;
        GameManager.GameEnd -= StopCoroutines;
    }

    private void SetMarblePosition()
    {
        marblePosition = 0;
        currentPosition = marblePosition;
    }

    private void StopCoroutines()
    {
        if (playerTransitionCoroutine != null)
            StopCoroutine(playerTransitionCoroutine);
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

                    if (CameraManager.cameraTransitionCoroutine != null)
                    {
                        StopCoroutine(CameraManager.cameraTransitionCoroutine);
                    }

                    if (playerTransitionCoroutine != null)
                    {
                        StopCoroutine(playerTransitionCoroutine);
                    }

                    playerTransitionCoroutine = StartCoroutine(MarbleTransition(+2f, 0.1f));

                    CameraManager.cameraTransitionCoroutine = StartCoroutine(cameraManager.CameraTransition(
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

                    if (CameraManager.cameraTransitionCoroutine != null)
                    {
                        StopCoroutine(CameraManager.cameraTransitionCoroutine);
                    }

                    if (playerTransitionCoroutine != null)
                    {
                        StopCoroutine(playerTransitionCoroutine);
                    }

                    playerTransitionCoroutine = StartCoroutine(MarbleTransition(-2f, 0.1f));

                    CameraManager.cameraTransitionCoroutine = StartCoroutine(cameraManager.CameraTransition(
                        CameraManager.mainCameraInstance.transform.position + new Vector3(-0.5f, 0f, 0f),
                        CameraManager.mainCameraInstance.transform.rotation,
                        0.1f));
                    canMove = false;
                }
            }
            else if (angle >= -135f && angle < -45f) // Down
            {
                if (!jumpAvailable)
                {
                    playerRigidbody.linearVelocity = Vector3.zero;
                    playerRigidbody.AddForce(Vector3.down * jumpHeight * 2, ForceMode.Impulse);
                    canMove = false;
                }
            }
        }
    }

    public override void PressedAction()
    {
        return;
    }

    private IEnumerator MarbleTransition(float goalPosition, float duration)
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
            // yield return new WaitForFixedUpdate();
        }

        playerRigidbody.MovePosition(new(currentPosition.x + goalPosition, transform.position.y, transform.position.z));

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