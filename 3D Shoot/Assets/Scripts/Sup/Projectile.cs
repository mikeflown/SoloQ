using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Damage & Lifetime")]
    public float damage = 25f;
    public float lifetime = 6f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Debug.Log("Projectile hit: " + other.name + " (Damage: " + damage + ")");
        Destroy(gameObject);
    }
}