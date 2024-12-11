using UnityEngine;
using GManager;
using System;

public class PlayerController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.GameStart += allowPlayerController;
        GameManager.GamePaused += banPlayerController;
        GameManager.GameEnd += banPlayerController;
    }
    private void OnDisable()
    {
        GameManager.GameStart -= allowPlayerController;
        GameManager.GamePaused -= banPlayerController;
        GameManager.GameEnd -= banPlayerController;
    }
    private void allowPlayerController()
    {
        throw new NotImplementedException();
    }
    private void banPlayerController()
    {
        throw new NotImplementedException();
    }
}
