using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 40f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, Vector2 hitPoint)
    {
        currentHealth -= amount;
        Debug.Log($"{name} recibe {amount} de daño. HP = {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // Después podés disparar animación, partículas, etc.
        Destroy(gameObject);
    }
}
