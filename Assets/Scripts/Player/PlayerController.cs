using UnityEngine;
using Managers;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction playerMove;
    private InputAction playerJump;

    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerMove = playerInput.actions["Move"];
        playerJump = playerInput.actions["Jump"];
    }
    private void OnEnable()
    {
        GameManager.GameStart += AllowPlayerController;
        GameManager.GamePaused += BanPlayerController;
        GameManager.GameEnd += BanPlayerController;
        playerMove.performed += PlayerMovement;
        playerJump.performed += PlayerJump;
    }
    private void OnDisable()
    {
        GameManager.GameStart -= AllowPlayerController;
        GameManager.GamePaused -= BanPlayerController;
        GameManager.GameEnd -= BanPlayerController;
        playerMove.performed -= PlayerMovement;
        playerJump.performed -= PlayerJump;
    }

    private void PlayerMovement(InputAction.CallbackContext context)
    {
        Vector2 moveValue = context.ReadValue<Vector2>();
        Debug.Log(moveValue);
    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        float jumpValue = context.ReadValue<float>();
        Debug.Log(jumpValue);
    }
    private void AllowPlayerController()
    {
        string marbleName = GameObject.FindWithTag("Player").name;
        switch (marbleName)
        {
            case ("Default"):
                {
                    gameObject.AddComponent<DefaultMarbleController>();
                    break;
                }
            case ("Cube"):
                {
                    print("pepe2");
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    private void BanPlayerController()
    {
        string marbleName = GameObject.FindWithTag("Player").name;
        switch (marbleName)
        {
            case ("Default"):
                {
                    DefaultMarbleController marbleController = gameObject.GetComponent<DefaultMarbleController>();
                    if (marbleController != null)
                        Destroy(marbleController);
                    break;
                }
            case ("Cube"):
                {
                    print("pepe2");
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}
