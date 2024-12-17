using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    [Header("Level obstacles")]
    public GameObject[] obstacleList;
    [Header("Obstacle count")]
    public int obstacleCount;
}
