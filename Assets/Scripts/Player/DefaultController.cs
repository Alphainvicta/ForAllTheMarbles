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

    public static Coroutine playerTransitionCoroutine;
    CameraManager cameraManager;

    private void Awake()
    {
        Initialize();
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
        marbleXPosition = 0;

    }

    private void StopCoroutines()
    {
        if (playerTransitionCoroutine != null)
        {
            StopCoroutine(playerTransitionCoroutine);
        }
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
                if (marbleXPosition < 1)
                {
                    marbleXPosition++;

                    if (CameraManager.cameraTransitionCoroutine != null)
                    {
                        StopCoroutine(CameraManager.cameraTransitionCoroutine);
                    }

                    if (playerTransitionCoroutine != null)
                    {
                        StopCoroutine(playerTransitionCoroutine);
                    }

                    playerTransitionCoroutine = StartCoroutine(MarbleXTransition(+2f, 0.1f));

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

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.Play("Jump");
                    }

                    canMove = false;
                }
            }
            else if (angle >= 135f || angle < -135f) // Left
            {
                if (marbleXPosition > -1)
                {
                    marbleXPosition--;

                    if (CameraManager.cameraTransitionCoroutine != null)
                    {
                        StopCoroutine(CameraManager.cameraTransitionCoroutine);
                    }

                    if (playerTransitionCoroutine != null)
                    {
                        StopCoroutine(playerTransitionCoroutine);
                    }

                    playerTransitionCoroutine = StartCoroutine(MarbleXTransition(-2f, 0.1f));

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
}