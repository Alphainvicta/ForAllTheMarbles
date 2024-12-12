using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject gameMenu;
        public TextMeshProUGUI selectedMarble;
        private void OnEnable()
        {
            GameManager.GameMenu += GameMenu;
        }

        private void OnDisable()
        {
            GameManager.GameMenu -= GameMenu;
        }

        public void GameMenu()
        {
            gameObject.GetComponent<PlayerManager>().ModifyPlayerValues();
            SelectedMarble();
            gameMenu.SetActive(true);
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

        public void PlayGame()
        {
            Debug.Log("PEPEEEEEEEEEEEEEEEEEEEEEEEEE");
        }
    }
}

