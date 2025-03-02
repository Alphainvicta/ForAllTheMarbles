using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public Obstacle[] obstacles;
    public int baseLevelLength = 1;

    public GameObject levelFloor;
}
