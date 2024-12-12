using UnityEngine;
using Managers;

public class PlayerController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.GameStart += AllowPlayerController;
        GameManager.GamePaused += BanPlayerController;
        GameManager.GameEnd += BanPlayerController;
    }
    private void OnDisable()
    {
        GameManager.GameStart -= AllowPlayerController;
        GameManager.GamePaused -= BanPlayerController;
        GameManager.GameEnd -= BanPlayerController;
    }
    private void AllowPlayerController()
    {

    }
    private void BanPlayerController()
    {

    }
}
