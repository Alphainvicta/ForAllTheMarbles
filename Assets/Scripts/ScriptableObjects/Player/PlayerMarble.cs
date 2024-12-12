using UnityEditor.EditorTools;
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

    [Header("Marble identifier")]
    public string marbleName;
}
