using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerMarble playerMarble;
        public GameObject currentMarble;
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