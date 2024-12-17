using TMPro;
using UnityEngine;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject gameMenu;
        public TextMeshProUGUI selectedMarble;
        private void OnEnable()
        {
            GameManager.GameMenu += HandleGameMenu;
            GameManager.GameStart += HandleGameStart;
        }

        private void OnDisable()
        {
            GameManager.GameMenu -= HandleGameMenu;
            GameManager.GameStart -= HandleGameStart;
        }

        private void HandleGameMenu() => GameMenu(1);
        private void HandleGameStart() => GameMenu(2);
        public void GameMenu(int option)
        {
            switch (option)
            {
                case 1:
                    gameObject.GetComponent<PlayerManager>().ModifyPlayerValues();
                    SelectedMarble();
                    gameMenu.SetActive(true);
                    break;
                case 2:
                    gameMenu.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void SelectedMarble()
        {
            string currentMarbleName = gameObject.GetComponent<PlayerManager>().currentMarble.name;
            if (selectedMarble.text != currentMarbleName)
                selectedMarble.text = currentMarbleName;
        }

        public void NextMarble()
        {
            string currentMarbleName = gameObject.GetComponent<PlayerManager>().currentMarble.name;
            PlayerManager playerManager = gameObject.GetComponent<PlayerManager>();
            PlayerMarble[] playerMarbles = gameObject.GetComponent<GameManager>().playerMarbles;
            for (int i = 0; i < playerMarbles.Length; i++)
            {
                if (currentMarbleName == playerMarbles[i].marbleName)
                {
                    if (i + 1 >= playerMarbles.Length)
                    {
                        playerManager.playerMarble = playerMarbles[0];
                    }
                    else
                    {
                        playerManager.playerMarble = playerMarbles[i + 1];

                    }
                    playerManager.ModifyPlayerValues();
                    SelectedMarble();
                    break;
                }
            }
        }

        public void PreviousMarble()
        {
            string currentMarbleName = gameObject.GetComponent<PlayerManager>().currentMarble.name;
            PlayerManager playerManager = gameObject.GetComponent<PlayerManager>();
            PlayerMarble[] playerMarbles = gameObject.GetComponent<GameManager>().playerMarbles;
            for (int i = 0; i < playerMarbles.Length; i++)
            {
                if (currentMarbleName == playerMarbles[i].marbleName)
                {
                    if (i - 1 < 0)
                    {
                        playerManager.playerMarble = playerMarbles[playerMarbles.Length - 1];
                    }
                    else
                    {
                        playerManager.playerMarble = playerMarbles[i - 1];
                    }
                    playerManager.ModifyPlayerValues();
                    SelectedMarble();
                    break;
                }
            }
        }
    }
}

