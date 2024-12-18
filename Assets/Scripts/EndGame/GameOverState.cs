using UnityEngine;
using Managers;

public class GameOverState : MonoBehaviour
{
    public bool isVictory;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameOverManager.isVictory == null)
            {
                GameManager.EndGame();
                if (isVictory)
                {
                    GameOverManager.isVictory = true;
                }
                else
                {
                    GameOverManager.isVictory = false;
                }
            }
        }
    }
}
