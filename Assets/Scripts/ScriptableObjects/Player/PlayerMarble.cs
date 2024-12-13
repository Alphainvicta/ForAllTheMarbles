using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMarble", menuName = "Scriptable Objects/PlayerMarble")]
public class PlayerMarble : ScriptableObject
{
    [Header("Marble appearance")]
    public Mesh mesh;
    public Material material;

    [Header("Marble rigidbody")]
    public float mass = 1f;
    public float linearDamping;
    public float angularDamping = 0.05f;

    [Header("Marble forces")]
    public float marbleSpeed;
    public float marbleJumpForce;
    public float marbleSmoothSpeed;

    [Header("Marble identifier")]
    public string marbleName;

    [Header("MarbleScript")]
    public MonoScript playerControllerScripts;
}
