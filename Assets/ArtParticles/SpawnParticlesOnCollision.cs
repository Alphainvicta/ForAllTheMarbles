using UnityEngine;

public class SpawnParticlesOnCollision : MonoBehaviour
{
    public GameObject particlePrefab; // Prefab del sistema de partículas

    private void OnCollisionEnter(Collision collision)
    {
        // Solo generar partículas si el objeto tiene el tag "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (particlePrefab != null && collision.contacts.Length > 0)
            {
                ContactPoint contact = collision.contacts[0];
                Quaternion rot = Quaternion.LookRotation(contact.normal);
                Vector3 pos = contact.point;

                Instantiate(particlePrefab, pos, rot);
            }
        }
    }
}
