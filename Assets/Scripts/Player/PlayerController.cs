using UnityEngine;
using Managers;
using System;

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
