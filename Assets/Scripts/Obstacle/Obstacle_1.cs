using System.Linq;
using UnityEngine;

public class Obstacle_1 : MonoBehaviour
{
    [SerializeField] private Transform[] platformList;
    private void OnEnable()
    {
        Transform lastPlatform = gameObject.transform;
        foreach (Transform platform in platformList)
        {
            //Set random size on the z axis
            int randomSize = Random.Range(3, 10);
            platform.localScale = new Vector3(1f, 1f, randomSize);

            //Set correct snap point for the platform
            float positionZ = lastPlatform.position.z + (lastPlatform.localScale.z / 2) + (platform.localScale.z / 2);

            //Set random arrange condition for more diversity
            int randomArrange = Random.Range(0, 1);
            switch (randomArrange)
            {
                case 0:
                    {
                        //Set the random side to set the next platform left or right
                        int randomSide = Random.Range(0, 2);
                        float positionX;
                        if (randomSide > 0)
                        {
                            positionX = lastPlatform.position.x + 1f;
                        }
                        else
                        {
                            positionX = lastPlatform.position.x - 1f;
                        }

                        //Set the random offset of the platform
                        int randomOffset = Random.Range(-1, 1);
                        positionZ += randomOffset;

                        platform.position = new Vector3(positionX, platform.position.y, positionZ);
                        break;
                    }
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            lastPlatform = platform.transform;
        }
    }
}
