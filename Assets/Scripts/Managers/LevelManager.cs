using System;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public Level[] levelList;
        private void OnEnable()
        {
            GameManager.GameStart += HandleGameStart;
            GameManager.GameEnd += HandleGameEnd;
            GameManager.GameMenu += HandleGameEnd;
        }

        private void OnDisable()
        {
            GameManager.GameStart -= HandleGameStart;
            GameManager.GameEnd -= HandleGameEnd;
            GameManager.GameMenu -= HandleGameEnd;
        }

        private void HandleGameStart() => GameLevel(1);
        private void HandleGameEnd() => GameLevel(2);

        private void GameLevel(int option)
        {
            switch (option)
            {
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }
    }
}