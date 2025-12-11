using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;

    public float CurrentHealth => currentHealth;  // propiedad de solo lectura

    float currentHealth;
    bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, Vector2 hitPoint)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"Player recibe {amount} de da�o. HP = {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // cortar movimiento
        var controller = GetComponent<TopDownPlayerController2D>();
        if (controller) controller.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        // avisar al GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
    }

}
