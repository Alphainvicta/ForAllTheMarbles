using System;
using UnityEngine;


namespace GManager
{
    public class GameManager : MonoBehaviour
    {
        public static event Action GameMenu;
        public static event Action GameStart;
        public static event Action GamePaused;
        public static event Action GameEnd;
        public static event Action GameStore;
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

