using UnityEngine;

public class Obstacle_2 : MonoBehaviour
{
    [SerializeField] private Transform[] platformList;
    private int positionPlatform;
    private void OnEnable()
    {
        gameObject.GetComponent<ObstacleEndPosition>().obstacleEndPosition = null;
        Transform lastPlatform = gameObject.transform;
        positionPlatform = 0;
        float positionZ = 0;

        //Set random size on the z axis
        int randomSize = Random.Range(5, 15);

        foreach (Transform platform in platformList)
        {
            platform.localScale = new Vector3(1f, 1f, randomSize);

            //Set correct snap point for the platform
            if (positionPlatform == 0)
            {
                positionZ = lastPlatform.position.z + (lastPlatform.localScale.z / 2) + (platform.localScale.z / 2);
                positionZ += Random.Range(2, 4);
            }

            switch (positionPlatform)
            {
                case 0:
                    {
                        float positionX = lastPlatform.position.x - 3f;
                        platform.position = new Vector3(positionX, platform.position.y, positionZ);
                        break;
                    }
                case 1:
                    {
                        float positionX = lastPlatform.position.x + 3f;
                        platform.position = new Vector3(positionX, platform.position.y, positionZ);
                        break;
                    }
                case 2:
                    {
                        float positionX = lastPlatform.position.x + 3f;
                        platform.position = new Vector3(positionX, platform.position.y, positionZ);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            lastPlatform = platform.transform;
            positionPlatform += 1;
        }
        int randomEndPosition = Random.Range(0, platformList.Length);
        gameObject.GetComponent<ObstacleEndPosition>().obstacleEndPosition = platformList[randomEndPosition];
    }
}
