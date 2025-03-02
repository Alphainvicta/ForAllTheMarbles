using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle", menuName = "ScriptableObjects/Obstacle")]
public class Obstacle : ScriptableObject
{
    public GameObject obstaclePrefab;
    public ObstacleWidth obstacleWidth;
    public int ObstacleLength = 1;
    public ObstaclePosition obstaclePosition = ObstaclePosition.Both;
    public bool needFloor = true;

    public enum ObstacleWidth
    {
        Small,
        Medium,
        Large
    }

    public enum ObstaclePosition
    {
        Edges,
        Center,
        Both
    }
}
