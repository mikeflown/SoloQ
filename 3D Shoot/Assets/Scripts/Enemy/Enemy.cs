using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Points Modifier")]
    public int pointsOnDeath = 15;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(name + " HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        SpawnManager spawner = FindFirstObjectByType<SpawnManager>();
        if (spawner != null)
        {
            spawner.AddPoints(pointsOnDeath);
            Debug.Log("Enemy killed! +" + pointsOnDeath + " points");
        }

        Destroy(gameObject);
    }
}