using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerMarble playerMarble;
        public GameObject currentMarble;
        public float touchThreshold;

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

        private void HandleGameStart() => ManagePlayerController(1);
        private void HandleGameEnd() => ManagePlayerController(2);

        private void ManagePlayerController(int option)
        {
            System.Type marbleScript = playerMarble.playerControllerScripts.GetClass();
            var currentMarbleScript = currentMarble.GetComponent(marbleScript);

            switch (option)
            {
                case 1:
                    if (currentMarbleScript == null)
                        currentMarble.AddComponent(marbleScript);
                    break;
                case 2:
                    if (currentMarbleScript != null)
                        Destroy(currentMarbleScript);
                    break;
                default:
                    break;
            }
        }
        public void ModifyPlayerValues()
        {
            string currentName = currentMarble.name;
            if (currentName != playerMarble.marbleName)
            {
                currentMarble.name = playerMarble.marbleName;
                currentMarble.GetComponent<MeshFilter>().mesh = playerMarble.mesh;
                if (playerMarble.material != null)
                    currentMarble.GetComponent<MeshRenderer>().material = playerMarble.material;

                Rigidbody marbleRigid = currentMarble.GetComponent<Rigidbody>();
                marbleRigid.mass = playerMarble.mass;
                marbleRigid.linearDamping = playerMarble.linearDamping;
                marbleRigid.angularDamping = playerMarble.angularDamping;
            }
        }
    }

}