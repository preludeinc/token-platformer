using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 2;
    int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void enemyHit(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            Debug.Log("Hit enemy");
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (gameObject != null)
        {
            Debug.Log("Enemy died");
            Destroy(gameObject);
        }
    }
}
