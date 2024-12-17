using UnityEngine;

public class Obstacle_3 : MonoBehaviour
{
    [SerializeField] private Transform[] platformList;
    private void OnEnable()
    {
        gameObject.GetComponent<ObstacleEndPosition>().obstacleEndPosition = null;
        Transform lastPlatform = gameObject.transform;
        foreach (Transform platform in platformList)
        {
            //Set random size on the z axis
            int randomSize = Random.Range(5, 10);
            platform.localScale = new Vector3(1f, 1f, randomSize);

            //Set correct snap point for the platform
            float positionZ = lastPlatform.position.z + (lastPlatform.localScale.z / 2) + (platform.localScale.z / 2);

            //Set the random side to set the next platform left or right
            int randomSide = Random.Range(0, 2);

            //Set random arrange condition for more diversity
            int randomArrange = Random.Range(0, 2);
            float positionX;
            if (randomSide > 0)
            {
                positionX = lastPlatform.position.x + Random.Range(0, 4);
            }
            else
            {
                positionX = lastPlatform.position.x - Random.Range(0, 4);
            }

            //Set a random height to the platform
            float positionY = 0;
            while (true)
            {
                positionY = Random.Range(0, 4);
                if (randomSide > 0)
                {
                    positionY = lastPlatform.position.y + positionY;
                }
                else
                {
                    positionY = lastPlatform.position.y - positionY;
                }

                if (positionY >= 0)
                    break;
            }

            //Set the random offset of the platform
            int randomOffset = Random.Range(0, 4);
            positionZ += randomOffset;

            platform.position = new Vector3(positionX, positionY, positionZ);

            lastPlatform = platform.transform;
        }
        gameObject.GetComponent<ObstacleEndPosition>().obstacleEndPosition = lastPlatform;
    }
}
